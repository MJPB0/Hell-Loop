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

    protected UnityAction weaponWasUsed;

    protected Player player;
    protected PlayerController controller;

    [Space]
    [Header("Weapon info")]
    [SerializeField] protected WeaponName weaponName;
    [SerializeField] protected PassiveName passiveNeededToEvolve;
    [SerializeField] protected bool isEvolved;

    [Space]
    [SerializeField] protected GameObject evolvedWeapon;

    [Space]
    [Header("Weapon level")]
    [SerializeField] protected int currentLevel = 1;
    [SerializeField] protected int maxLevel;

    [Space]
    [Header("Weapon usage")]
    [SerializeField] protected bool canAttack = true;
    [SerializeField] protected float timeToNextAttack = 0f;
    [SerializeField] protected float timeBetweenAttacks = 1f;

    [Space]
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float criticalChance;

    [Space]
    [SerializeField] protected WeaponEffect primaryEffect;
    [SerializeField] protected WeaponEffect secondaryEffect;

    [Space]
    [Header("Weapon sprite")]
    [SerializeField] private Sprite sprite;

    [Space]
    [SerializeField] protected List<Enemy> enemiesInRange;

    [Space]
    [Header("Colliders")]
    [SerializeField] private CircleCollider2D pickupCollider;
    [SerializeField] protected Collider2D rangeCollider;

    [Space]
    [Header("Projectiles")]
    [SerializeField] protected Transform projectilesParent;

    [Space]
    [Header("On hit animations")]
    [SerializeField] protected GameObject onHitEffect;
    [SerializeField] protected AnimationClip onHitEffectAnimation;

    public bool CanAttack { get { return canAttack; } }
    public WeaponName WeaponName { get { return weaponName; } }
    public PassiveName PassiveNeededToEvolve { get { return passiveNeededToEvolve; } }
    public float AttackSpeed { get { return attackSpeed;} }
    public float AttackRange { get { return attackRange;} }

    public int CurrentLevel { get { return currentLevel; } }
    public int MaxLevel { get { return maxLevel; } }

    public bool IsEvolved { get { return isEvolved; } }
    public GameObject EvolvedWeapon { get { return evolvedWeapon; } }

    public WeaponEffect PrimaryEffect { get { return primaryEffect; } }
    public WeaponEffect SecondaryEffect { get { return secondaryEffect; } }

    public List<Enemy> Enemies { get { return enemiesInRange; } }

    public GameObject OnHitEffect { get { return onHitEffect; } }
    public AnimationClip OnHitEffectAnimation { get { return onHitEffectAnimation; } }

    void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    protected void Start()
    {
        gameObject.transform.localScale = new Vector3(.8f, .8f, .8f);
        spriteRenderer.sprite = sprite;

        if (pickupCollider)
            pickupCollider.enabled = true;
        if (rangeCollider && !isEvolved)
            rangeCollider.enabled = false;

        playerInventory = FindObjectOfType<PlayerInventory>();
        player = FindObjectOfType<Player>();
        controller = player.gameObject.GetComponent<PlayerController>();

        enemiesInRange = new List<Enemy>();

        projectilesParent = GameObject.FindGameObjectWithTag(PROJECTILES_PARENT).transform;
    }

    void Update()
    {
        if (timeToNextAttack > 0)
            timeToNextAttack -= Time.deltaTime * attackSpeed * player.AttackSpeedMultiplier;
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
    }

    public void LevelUp()
    {
        if (currentLevel < maxLevel)
            currentLevel++;
    }

    public abstract void Use();
    public abstract void SetCurrentlyUsed();
    public abstract void SetNotCurrentlyUsed();
}
