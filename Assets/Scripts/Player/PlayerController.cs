using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private const string DASH_TRIGGER = "Dash";
    private const string IS_RUNNING = "IsRunning";
    private const string DEATH_TRIGGER = "Death";
    private const string HIT_TRIGGER = "Hit";

    public PlayerActions Controls { get; private set; }
    private InputActionMap currentActions;

    private Vector2 movementInput;

    private Player player;
    private ObjectInteraction playerInteraction;
    private PlayerInventory playerInventory;
    private GameObject playerBody;

    private Animator animator;

    private float pixelsPerUnit;

    public bool IsFlipped { get; private set; }

    public int IndexSelected { get; set; }

    void Awake()
    {
        Controls = new PlayerActions();

        player = GetComponent<Player>();
        playerInventory = GetComponent<PlayerInventory>();
        playerInteraction = GetComponentInChildren<ObjectInteraction>();
        playerBody = GetComponentInChildren<SpriteRenderer>().gameObject;

        animator = GetComponentInChildren<Animator>();

        InputSetup();
    }

    private void Start()
    {
        pixelsPerUnit = GetComponentInChildren<SpriteRenderer>().sprite.pixelsPerUnit;

        GameplayManager.Instance.OnGameEnd += DisableControls;

        player.OnPlayerDash += () => animator.SetTrigger(DASH_TRIGGER);
        player.OnPlayerDeath += () => animator.SetTrigger(DEATH_TRIGGER);
        player.OnPlayerTakeDamage += () => animator.SetTrigger(HIT_TRIGGER);
    }

    private void FixedUpdate()
    {
        if (player.CanMove) 
            Move();
        else 
            animator.SetBool(IS_RUNNING, false);

        if (player.TimeToNextDash > 0f) player.TimeToNextDash -= Time.deltaTime;
        else if (!player.CanDash)
        {
            //Debug.Log("Dash ready!");
            player.CanDash = true;
            player.TimeToNextDash = 0f;
        }
    }

    private Vector2 PixelPerfectClamp(Vector2 move)
    {
        Vector2 moveInPixels = new(
            Mathf.RoundToInt(move.x * pixelsPerUnit),
            Mathf.RoundToInt(move.y * pixelsPerUnit)
        );

        return moveInPixels / pixelsPerUnit;
    }

    private void Move()
    {
        if (player.IsDashing || movementInput.magnitude < .1f)
        {
            animator.SetBool(IS_RUNNING, false);
            return;
        }

        animator.SetBool(IS_RUNNING, true);

        if (movementInput.x < 0f)
            FlipBody(false);
        else if (movementInput.x > 0f)
            FlipBody(true);

        Vector2 newPos = player.MovementSpeed * Time.deltaTime * movementInput;
        newPos = PixelPerfectClamp(newPos);

        transform.Translate(new Vector3(newPos.x, newPos.y, 0));
    }

    private void Dash()
    {
        if (!player.CanDash) return;
        player.IsDashing = true;
        player.CanMove = false;
        player.CanDash = false;
        player.TimeToNextDash = player.DashCooldown;

        player.OnPlayerDash?.Invoke();

        StartCoroutine(PerformDash());
        //Debug.Log("Dash performed!");
    }

    private void FlipBody(bool facingRight)
    {
        IsFlipped = !facingRight;
        float rotation = facingRight ? 0 : 180;
        playerBody.transform.rotation = new Quaternion(0, rotation, 0, 0);
    }

    public Vector2 MouseDirection()
    {
        Vector2 playerPos = new(transform.position.x, transform.position.y);
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Controls.Gameplay.MousePosition.ReadValue<Vector2>());
        return (mousePos - playerPos).normalized;
    }

    public Vector2 MousePosition() => Camera.main.ScreenToWorldPoint(Controls.Gameplay.MousePosition.ReadValue<Vector2>());

    IEnumerator PerformDash()
    {
        player.CanTakeDamage = false;

        Vector2 dir = MouseDirection();
        if (dir.x < 0)
            FlipBody(false);
        else
            FlipBody(true);

        float startTime = Time.time;
        while (Time.time < startTime + player.DashDuration)
        {
            Vector2 newPos = player.DashStrength * Time.deltaTime * dir;
            newPos = PixelPerfectClamp(newPos);

            transform.Translate(new Vector3(newPos.x, newPos.y, 0));
            yield return null;
        }

        player.CanTakeDamage = true;
        player.IsDashing = false;

        yield return new WaitForSeconds(player.DashDaze);
        player.CanMove = true;
    }

    public void SwitchActionMap(PlayerActionTypes type)
    {
        if (type.ToString() == currentActions.name) return;

        currentActions.Disable();

        switch (type)
        {
            case PlayerActionTypes.Gameplay:
                currentActions = Controls.Gameplay;
                currentActions.Enable();
                break;
            case PlayerActionTypes.PassiveSelect:
                currentActions = Controls.PassiveSelect;
                currentActions.Enable();
                break;
            case PlayerActionTypes.WeaponReplace:
                currentActions = Controls.WeaponReplace;
                currentActions.Enable();
                break;
        }
    }

    private void InputSetup()
    {
        GameplayInputsSetup();
        PassiveSelectInputsSetup();
        WeaponReplaceInputsSetup();

        currentActions = Controls.Gameplay;
        currentActions.Enable();
    }

    private void GameplayInputsSetup()
    {
        Controls.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        Controls.Gameplay.Movement.canceled += cts => movementInput = Vector2.zero;

        Controls.Gameplay.Dash.performed += ctx => Dash();

        Controls.Gameplay.Interact.performed += ctx => Interact();

        Controls.Gameplay.UseWeaponEffect.performed += ctx => UseWeaponEffect();

        Controls.Gameplay.NextWeapon.performed += ctx => NextWeapon();
        Controls.Gameplay.PreviousWeapon.performed += ctx => PreviousWeapon();
    }

    private void PassiveSelectInputsSetup()
    {
        Controls.PassiveSelect.OneClicked.performed += ctx => IndexSelected = 0;
        Controls.PassiveSelect.TwoClicked.performed += ctx => IndexSelected = 1;
        Controls.PassiveSelect.ThreeClicked.performed += ctx => IndexSelected = 2;
    }

    private void WeaponReplaceInputsSetup()
    {
        Controls.WeaponReplace.OneClicked.performed += ctx => IndexSelected = 0;
        Controls.WeaponReplace.TwoClicked.performed += ctx => IndexSelected = 1;
        Controls.WeaponReplace.ThreeClicked.performed += ctx => IndexSelected = 2;
    }

    private void Interact() => playerInteraction.Interact();

    private void UseWeaponEffect() => playerInventory.UseWeaponEffect();

    private void NextWeapon() => playerInventory.NextWeapon();
    private void PreviousWeapon() => playerInventory.PreviousWeapon();

    public void DisableControls() => currentActions.Disable();
}
