using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Merchant : InteractableObject
{
    protected const string INTERACTABLES_TAG = "Interactables Parent";
    protected const string PICKABLES_TAG = "Pickables Parent";

    private const string ENEMY_RUN_BOOLEAN = "IsRunning";

    [Header("Loot")]
    [SerializeField] private Transform interactablesParent;
    [SerializeField] private Transform pickablesParent;

    [Space]
    [SerializeField] private int minTimesToInteract = 1;
    [SerializeField] private int maxTimesToInteract = 5;
    [SerializeField] private int timesToInteract = 1;
    [SerializeField] private GameObject[] possibleLoot;

    [Space]
    [SerializeField] private float timeBetweenInteracts = 60f;
    [SerializeField] private float timeToNextInteraction = 0f;

    [Header("Movement")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float moveSpeed = 3;

    [Space]
    [SerializeField] private float timeLeftToStopMoving = 0f;
    [SerializeField] private float minTimeToMove = 1f;
    [SerializeField] private float maxTimeToMove = 10f;

    [Space]
    [SerializeField] private float timeLeftToStartMoving = 0f;
    [SerializeField] private float minTimeToWait = 1f;
    [SerializeField] private float maxTimeToWait = 10f;

    [Space]
    [SerializeField] private Vector2 moveDirection;

    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        interactablesParent = GameObject.FindGameObjectWithTag(INTERACTABLES_TAG).transform;
        pickablesParent = GameObject.FindGameObjectWithTag(PICKABLES_TAG).transform;

        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        timesToInteract = Random.Range(minTimesToInteract, maxTimesToInteract);
    }

    private void Update()
    {
        if (canMove) Move();
        else if (timeLeftToStartMoving <= 0f) DecideToMove();

        if (timeLeftToStartMoving > 0f) timeLeftToStartMoving -= Time.deltaTime;
        if (timeToNextInteraction > 0f) timeToNextInteraction -= Time.deltaTime;
    }

    private void Move()
    {
        anim.SetBool(ENEMY_RUN_BOOLEAN, true);

        Vector3 step = moveSpeed * Time.deltaTime * moveDirection;

        if (moveDirection.x < 0f)
            spriteRenderer.flipX = true;
        else if (moveDirection.x > 0f)
            spriteRenderer.flipX = false;

        transform.Translate(step);

        timeLeftToStopMoving -= Time.deltaTime;
        if (timeLeftToStopMoving <= 0f)
        {
            canMove = false;
            timeLeftToStartMoving = Random.Range(minTimeToWait, maxTimeToWait);
            anim.SetBool(ENEMY_RUN_BOOLEAN, false);
        }
    }

    private void DecideToMove()
    {
        if (Random.Range(0,2) == 1)
        {
            canMove = true;
            moveDirection = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            timeLeftToStopMoving = Random.Range(minTimeToMove, maxTimeToMove);
        }
        else
            canMove = false;
    }

    public override void Interact()
    {
        if (!IsInteractable) return;
        if (timeToNextInteraction > 0f) return;

        timesToInteract -= 1;
        if (timesToInteract <= 0)
            IsInteractable = false;

        timeToNextInteraction = timeBetweenInteracts;

        GameObject objToSpawn = possibleLoot[Random.Range(0, possibleLoot.Length)];
        bool isAutomaticallyPickable = objToSpawn.GetComponent<InteractableObject>().AutomaticInteraction;
        Instantiate(objToSpawn, transform.position, Quaternion.identity, isAutomaticallyPickable ? pickablesParent : interactablesParent);
    }
}
