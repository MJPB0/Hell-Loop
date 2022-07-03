using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Enemy : MonoBehaviour
{
    protected const string PICKABLES_PARENT_TAG = "Pickables Parent";
    protected const string INTERACTABLES_TAG = "Interactables Parent";

    protected const string ENEMY_DEATH_TRIGGER = "Death";
    protected const string ENEMY_GET_HIT_TRIGGER = "WasHit";
    protected const string ENEMY_ATTACK_TRIGGER = "Attack";

    private const string ENEMY_RUN_BOOLEAN = "IsRunning";

    public UnityAction OnEnemyTakeDamage;
    public UnityAction OnEnemyDeath;

    public UnityAction OnEnemyIsMovingChange;

    public EnemyEffects EnemyEffectsController { get; private set; }

    [Header("Level")]
    [SerializeField] protected int level = 1;
    [SerializeField] protected float levelupInterval = 60f;

    [Space]
    [SerializeField] protected float movementIncreasePerLevel = .2f;
    [SerializeField] protected float spawnChanceIncreasePerLevel = .2f;

    [SerializeField] protected float healthIncreasePerLevel = 5f;

    [Space]
    [SerializeField] protected float damageIncreasePerLevel = .2f;
    [SerializeField] protected float attackRangeIncreasePerLevel = .1f;
    [SerializeField] protected float attackSpeedIncreasePerLevel = .05f;
    [SerializeField] protected float timeBetweenAttacksDecreasePerLevel = .01f;

    [Space]
    [SerializeField] protected float dropRateIncreasePerLevel = .02f;
    [SerializeField] protected float dropAmountIncreasePerLevel = .2f;

    [Header("Health")]
    [SerializeField] protected int health = 100;

    [Space]
    public bool isAlive = true;
    [SerializeField] protected bool canTakeDamage = true;

    [Header("Movement")]
    [SerializeField] protected bool canMove = true;
    [SerializeField] protected bool isMoving = false;
    [SerializeField] protected float knockbackDaze = 1f;
    [SerializeField] protected float damageDaze = .2f;

    [Space]
    [SerializeField] protected float slowness = 1f;
    [SerializeField] protected float moveSpeed = 3;

    [Header("Spawn")]
    [SerializeField] [Range(0,1)] protected float spawnChance = 1f;
    [SerializeField] protected float spawnChanceMultiplier = 1f;

    [Header("Combat")]
    [SerializeField] protected bool canBeKnockedBack = false;
    [SerializeField] protected bool playerInRange = false;

    [Space]
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float attackRange = 1f;
    [SerializeField] protected float attackTimeReduction = 0f;

    [Space]
    [SerializeField] protected bool isFrozen = false;
    [SerializeField] protected bool canAttack = false;
    [SerializeField] protected float timeBetweenAttacks = 2f;
    [SerializeField] protected float timeToNextAttack = 0f;

    [Space]
    [SerializeField] protected AnimationClip attack;
    [SerializeField] protected AnimationClip death;
    [SerializeField] protected AnimationClip getHit;

    [Header("Drops")]
    [SerializeField] protected bool dropsItems;
    [SerializeField] protected Transform interactablesParent;
    [SerializeField] protected Transform pickablesParent;

    [Space]
    [SerializeField] protected bool isSpawningLoot;
    [SerializeField] protected float timeBetweenDropSpawns = .2f;
    [SerializeField] protected float minSpawnRadius = .5f;
    [SerializeField] protected float maxSpawnRadius = 1f;

    [Space]
    [SerializeField] protected GameObject[] loot;
    [SerializeField] protected int[] amounts;
    [SerializeField] [Range(0,1)] protected float[] chances;

    protected Player player;

    protected SpriteRenderer body;
    protected Animator anim;

    protected BoxCollider2D enemyCollider;

    public float SpawnChance { get { return spawnChance * spawnChanceMultiplier + spawnChanceIncreasePerLevel * level; } set { spawnChance = value; } }
    public float AttackRange { get { return attackRange + attackRangeIncreasePerLevel * level; } }
    public int Damage { get { return Mathf.RoundToInt(damage + damageIncreasePerLevel * level); } }
    public int Level { get { return level; } set { level = value; } }

    protected void Start()
    {
        player = FindObjectOfType<Player>();

        body = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        enemyCollider = GetComponent<BoxCollider2D>();

        EnemyEffectsController = FindObjectOfType<EnemyEffects>();

        pickablesParent = GameObject.FindGameObjectWithTag(PICKABLES_PARENT_TAG).transform;
        interactablesParent = GameObject.FindGameObjectWithTag(INTERACTABLES_TAG).transform;

        OnEnemyDeath += DropItems;
        OnEnemyDeath += () => StartCoroutine(WaitAndDestroyEnemy());

        OnEnemyTakeDamage += () => SoundManager.PlayEnemyDamageSound();
    }

    protected void Update()
    {
        IsPlayerInRange();
        CanAttack();
        if (canMove && isAlive && !playerInRange)
            Move();
        if (playerInRange && isAlive && canAttack && !isFrozen)
            Attack();
        FacePlayer();
    }

    private void IsPlayerInRange()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange + attackRangeIncreasePerLevel * level)
        {
            playerInRange = true;
            isMoving = false;
            anim.SetBool(ENEMY_RUN_BOOLEAN, isMoving);
        }
        else
            playerInRange = false;
    }

    private void CanAttack()
    {
        if (timeToNextAttack > 0f)
            timeToNextAttack -= Time.deltaTime;
        else
        {
            canAttack = true;
            timeToNextAttack = 0f;
        }
    }

    private void Move()
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Vector3 step = (moveSpeed + movementIncreasePerLevel * level) * Time.deltaTime * dir / slowness;

        if (step.magnitude == 0f) 
            isMoving = false;
        else 
            isMoving = true;

        anim.SetBool(ENEMY_RUN_BOOLEAN, isMoving);

        transform.Translate(step);
    }

    private void FacePlayer()
    {
        if ((player.transform.position - transform.position).x < 0)
        {
            body.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else
        {
            body.transform.rotation = new Quaternion(0, 180, 0, 0);
        }
    }

    public void ApplyDamageOverTime(int damage, float duration, float damageIntervals, WeaponName weaponName)
    {
        if (!canTakeDamage || !isAlive) return;

        StartCoroutine(DamageOverTime(damage, duration, damageIntervals, weaponName));
    }

    public void ApplySlow(float value, float duration)
    {
        if (!isAlive || isFrozen) return;

        StartCoroutine(Slowness(value, duration));
    }

    public void KnockBack(float value)
    {
        if (!canBeKnockedBack) return;

        Vector3 dir = (transform.position - player.transform.position).normalized;
        transform.Translate(value * dir);

        StartCoroutine(Daze(knockbackDaze));
    }

    private IEnumerator Daze(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public IEnumerator DamageOverTime(int damage, float duration, float damageIntervals, WeaponName weaponName)
    {
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            TakeDamage(damage, weaponName);

            Vector3 positionVector;
            positionVector = new Vector3(transform.position.x + 9.8f, transform.position.y - 1.5f, transform.position.z);
            DamagePopup.Create(positionVector, damage.ToString(), DamagePopupOwner.ENEMY_HIT);

            SoundManager.PlayEnemyDamageSound();
            yield return new WaitForSeconds(damageIntervals);
        }
    }

    public IEnumerator Slowness(float value, float duration)
    {
        slowness += value;

        if (slowness > 50)
            isFrozen = true;

        yield return new WaitForSeconds(duration);
        slowness -= value;

        if (slowness < 50)
            isFrozen = false;
    }

    public void ScaleEnemyWithGameTime(float currentTime)
    {
        level = Mathf.FloorToInt(currentTime / levelupInterval);
        health = Mathf.RoundToInt(health + healthIncreasePerLevel * level);
        for (int i = 0; i < amounts.Length; i++)
            amounts[i] += Mathf.FloorToInt(dropAmountIncreasePerLevel * level);
        for (int i = 0; i < chances.Length; i++)
            chances[i] += Mathf.FloorToInt(dropRateIncreasePerLevel * level);
    }

    public void DropItems()
    {
        if (!dropsItems) return;

        isSpawningLoot = true;
        StartCoroutine(SpawnDrops());
    }

    private IEnumerator SpawnDrops()
    {
        for (int i = 0; i < loot.Length; i++)
        {
            if (Random.Range(0f, 1f) > chances[i])
                continue;

            GameObject dropToSpawn = loot[i];
            bool isAutomaticallyPickable = dropToSpawn.GetComponent<InteractableObject>().AutomaticInteraction;
            for (int j = 0; j < amounts[i]; j++)
            {
                GameObject obj = Instantiate(dropToSpawn, transform.position, Quaternion.identity, isAutomaticallyPickable ? pickablesParent : interactablesParent);

                if (obj.GetComponent<ExperienceMineral>())
                    obj.transform.position = RandomizePosition(obj.transform.position);

                yield return new WaitForSeconds(timeBetweenDropSpawns);
            }
        }
        isSpawningLoot = false;
    }

    private Vector3 RandomizePosition(Vector3 pos)
    {
        int xMult = Random.Range(-1, 1) >= 0 ? 1 : -1;
        int yMult = Random.Range(-1, 1) >= 0 ? 1 : -1;

        float x = Random.Range(minSpawnRadius, maxSpawnRadius) * xMult;
        float y = Random.Range(minSpawnRadius, maxSpawnRadius) * yMult;

        return new Vector3(
            pos.x + x,
            pos.y + y,
            pos.z
        );
    }

    protected IEnumerator WaitAndEnableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    private IEnumerator WaitAndDestroyEnemy()
    {
        float startTime = Time.time;
        yield return new WaitUntil(() => !isSpawningLoot && startTime + death.length <= Time.time);
        Destroy(gameObject);
    }

    public void Die()
    {
        health = 0;
        canTakeDamage = false;
        canMove = false;
        isAlive = false;

        anim.SetTrigger(ENEMY_DEATH_TRIGGER);
        OnEnemyDeath?.Invoke();
    }

    public abstract void TakeDamage(int damage, WeaponName weaponName);
    protected abstract void Attack();
    public abstract void DealDamage(bool isSpecial);
}
