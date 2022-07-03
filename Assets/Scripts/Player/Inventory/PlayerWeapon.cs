using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlayerWeapon : InteractableObject
{
    private const string PROJECTILES_PARENT = "Player Projectiles Parent";

    protected const string TAG_AFTER_PICKUP = "Untagged";
    protected const string LAYER_AFTER_PICKUP = "Player projectile";

    protected SpriteRenderer spriteRenderer;
    protected PlayerInventory playerInventory;

    public UnityAction WeaponIsUsed;

    protected Player player;
    protected PlayerController controller;

    [Header("Weapon info")]
    [SerializeField] protected WeaponName weaponName;
    [SerializeField] protected PassiveName passiveNeededToEvolve;
    [SerializeField] protected bool isEvolved;

    [Space]
    [SerializeField] protected GameObject evolvedWeapon;

    [Header("Weapon level")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected int maxLevel;

    [SerializeField] protected WeaponLevelUpgradeSO[] levelUpgrades;

    [Header("Weapon usage")]
    [SerializeField] protected bool canAttack = true;
    [SerializeField] protected float timeToNextAttack = 0f;
    [SerializeField] protected float timeBetweenAttacks = 1f;

    [Space]
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float additionalAttackSpeed;

    [SerializeField] protected float attackRange;
    [SerializeField] protected float additionalAttackRange;

    [Space]
    [SerializeField] protected WeaponEffect primaryEffect;
    [SerializeField] protected WeaponEffect secondaryEffect;

    [Header("Weapon sprite")]
    [SerializeField] private Sprite sprite;

    [Space]
    [SerializeField] protected List<Enemy> enemiesInRange;

    [Header("Colliders")]
    [SerializeField] private CircleCollider2D pickupCollider;
    [SerializeField] protected Collider2D rangeCollider;

    [Header("Projectiles")]
    [SerializeField] protected Transform projectilesParent;

    [Header("VFX")]
    [SerializeField] protected GameObject onHitEffect;
    [SerializeField] protected AnimationClip onHitEffectAnimation;

    public bool CanAttack { get { return canAttack; } }

    public WeaponName WeaponName { get { return weaponName; } }
    public PassiveName PassiveNeededToEvolve { get { return passiveNeededToEvolve; } }

    public float AttackSpeed { get { return attackSpeed * player.AttackSpeedMultiplier + additionalAttackSpeed; } }
    public float AttackRange { get { return attackRange * player.AttackRangeMultiplier + additionalAttackRange; } }
    public float TimeToNextAttack { get { return timeToNextAttack; } }

    public int CurrentLevel { get { return currentLevel; } }
    public int MaxLevel { get { return maxLevel; } }
    public string NextWeaponStatUpgrade 
    { 
        get
        {
            WeaponLevelUpgradeSO nextUpg = levelUpgrades[CurrentLevel - 1];
            string decreases = nextUpg.StatType == WeaponStatType.DAMAGE_INTERVAL ? "-" : "+";
            string text = "";
            switch (nextUpg.StatType)
            {
                case WeaponStatType.DAMAGE_INTERVAL:
                    text = "damage interval";
                    break;
                case WeaponStatType.ENEMY_HIT_CAP:
                    text = "enemy hit cap";
                    break;
                case WeaponStatType.ATTACK_RANGE:
                    text = "attack range";
                    break;
                case WeaponStatType.ATTACK_SPEED:
                    text = "attack speed";
                    break;
                case WeaponStatType.PROJECTILE_SPEED:
                    text = "projectile speed";
                    break;
            }

            return $"{decreases}{nextUpg.Value * 100}% {text}";
        } 
    }
    public string NextWeaponEffectUpgrade
    {
        get
        {
            WeaponLevelUpgradeSO nextUpg = levelUpgrades[CurrentLevel - 1];

            if (nextUpg.PrimaryEffectUpgrade)
                return "Primary effect upgrade!";
            else if (nextUpg.SecondaryEffectUpgrade)
                return "Secondary effect upgrade!";

            return "";
        }
    }

    public bool IsEvolved { get { return isEvolved; } }
    public GameObject EvolvedWeapon { get { return evolvedWeapon; } }

    public WeaponEffect PrimaryEffect { get { return primaryEffect; } }
    public WeaponEffect SecondaryEffect { get { return secondaryEffect; } }

    public List<Enemy> Enemies { get { return enemiesInRange; } }

    public GameObject OnHitEffect { get { return onHitEffect; } }
    public AnimationClip OnHitEffectAnimation { get { return onHitEffectAnimation; } }

    public Sprite WeaponSprite { get { return sprite; } }

    protected void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        playerInventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<Player>();
        controller = player.gameObject.GetComponent<PlayerController>();
    }

    protected void Start()
    {
        gameObject.transform.localScale = new Vector3(.8f, .8f, .8f);
        if (!isEvolved)
            spriteRenderer.sprite = sprite;

        if (pickupCollider)
            pickupCollider.enabled = true;
        if (rangeCollider && !isEvolved)
            rangeCollider.enabled = false;

        if (levelUpgrades.Length != maxLevel - 1)
        {
            Debug.LogError($"Item: {gameObject.name} has less/more upgrades than required");
        }

        enemiesInRange = new List<Enemy>();

        projectilesParent = GameObject.FindGameObjectWithTag(PROJECTILES_PARENT).transform;
    }

    void Update()
    {
        if (timeToNextAttack > 0)
            timeToNextAttack -= Time.deltaTime;
        else
        {
            timeToNextAttack = 0f;
            canAttack = true;
        }
    }

    public override void Interact()
    {
        playerInventory.PickUpWeapon(this);
    }

    public void PickUp(Transform parent)
    {
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);

        pickupCollider.enabled = !pickupCollider.enabled;

        gameObject.tag = TAG_AFTER_PICKUP;
        gameObject.layer = LayerMask.NameToLayer(LAYER_AFTER_PICKUP);

        spriteRenderer.sprite = null;

        UpdateAttackRange(WeaponName == WeaponName.KNIFE || WeaponName == WeaponName.BLOODY_KNIFE);
    }

    public void LevelUp()
    {
        if (currentLevel < maxLevel)
        {
            DisplayFloatingText(NextWeaponStatUpgrade, DamagePopupOwner.PLAYER_HEAL);
            StartCoroutine(WaitAndDisplayFloatingText(.25f, NextWeaponEffectUpgrade, DamagePopupOwner.PLAYER_HEAL));

            currentLevel++;
            if (CheckIfUpgradeIsAvailable())
                StartCoroutine(WaitAndDisplayFloatingText(.5f, "Upgrade available!", DamagePopupOwner.ENEMY_CRITICAL_HIT));

            ApplyUpgrades();
        }
    }

    private bool CheckIfUpgradeIsAvailable()
    {
        var passive = playerInventory.PlayerPassivesList.Find(passive => passive.PassiveName == passiveNeededToEvolve);
        return passive && currentLevel == maxLevel;
    }

    private void DisplayFloatingText(string text, DamagePopupOwner color)
    {
        Vector3 positionVector = new(transform.position.x + 8.2f, transform.position.y - 1.5f, transform.position.z);

        DamagePopup.Create(positionVector, text, color);
    }

    IEnumerator WaitAndDisplayFloatingText(float time, string weaponEffect, DamagePopupOwner color)
    {
        yield return new WaitForSeconds(time);
        DisplayFloatingText(weaponEffect, color);
    }

    public void UpdateAttackRange(bool onlyHoriozontal)
    {
        if (rangeCollider.GetType() == typeof(BoxCollider2D))
        {
            (rangeCollider as BoxCollider2D).size = new(AttackRange, onlyHoriozontal ? (rangeCollider as BoxCollider2D).size.y : AttackRange);
            return;
        }

        if (rangeCollider.GetType() == typeof(CircleCollider2D))
        {
            (rangeCollider as CircleCollider2D).radius = AttackRange;
        }
    }

    protected abstract void ApplyUpgrades();

    public abstract void Use();
    public abstract void SetCurrentlyUsed();
    public abstract void SetNotCurrentlyUsed();
}
