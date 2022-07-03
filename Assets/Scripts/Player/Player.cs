using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityAction OnPlayerDash;

    public UnityAction OnPlayerTakeDamage;
    public UnityAction OnPlayerHeal;
    public UnityAction OnPlayerDeath;

    public UnityAction OnPlayerExperiencePickup;
    public UnityAction OnPlayerGoldPickup;

    public UnityAction OnPlayerMaxHealthChange;
    public UnityAction OnPlayerLevelUp;

    public UnityAction OnPlayerMove;

    public UnityAction<Vector3> OnPlayerDoubleAttack;

    #region Player stats

    [Header("Health")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Space]
    [SerializeField] private int healthRestoreOnLevelUp = 15;
    [SerializeField] private int healthIncreasePerLevelUp = 5;

    [Space]
    [SerializeField] private float baseHealthMultiplier = 1;
    [SerializeField] private float healthMultiplier = 0;

    [SerializeField] private int temporaryArmor = 0;

    [Header("Combat")]
    [SerializeField] private float attackSpeedMultiplier = 0f;

    [Space]
    public bool CanDoubleAttack = false;
    [SerializeField] private float doubleAttackChance = 0f;

    [Space]
    [SerializeField] private float attackRangeMultiplier = 0f;

    [Space]
    [SerializeField] private float baseCriticalChance = .1f;
    [SerializeField] private float criticalChanceMultiplier = 1f;

    [Space]
    [SerializeField] private float baseCriticalDamage = 2f;
    [SerializeField] private float criticalDamageMultiplier = 1f;

    [Space]
    [SerializeField] private float effectDurationMultiplier = 1f;

    [Space]
    [SerializeField] private float effectPowerMultiplier = 1f;

    [Space]
    [SerializeField] private float bulletSpeedMultiplier = 1f;

    [Header("Movement")]
    [SerializeField] private float baseMovementSpeed = 4f;

    [Space]
    public float DashStrength = 30f;
    public float DashDuration = 5f;
    public float DashDaze = .5f;

    [Space]
    public float TimeToNextDash = 0f;
    [SerializeField] private float baseDashCooldown = 4f;
    [SerializeField] private float dashCooldownMultiplier = 1f;

    [Header("Other")]
    [SerializeField] private float baseLuck = .1f;
    [SerializeField] private float luckMultiplier = 1f;

    [Space]
    [SerializeField] private float basePickupRange = 1f;
    [SerializeField] private float pickupRangeMultiplier = 1f;

    [Header("Experience")]
    [SerializeField] private int baseExperienceIncrease = 100;

    [Space]
    [SerializeField] private float experienceGainMultiplier = 1;

    [Space]
    [SerializeField] private int currentExperience = 0;
    [SerializeField] private int experienceToNextLevel = 100;
    [SerializeField] private int currentLevel = 1;

    [Header("Gold")]
    [SerializeField] private float goldGainMultiplier = 1;

    public int acquiredGold = 0;

    #endregion

    [Header("Checks")]
    public bool IsAlive = true;
    public bool CanTakeDamage = true;
    public bool CanPickupExperience = true;

    [Space]
    public bool IsDashing = false;

    [Space]
    public bool IsStunned = false;
    public bool CanMove = true;
    public bool CanDash = true;

    [Space]
    public bool IsAbleToEvolveWeapon = false;

    #region Accessors

    public float MovementSpeed { get { return baseMovementSpeed; } }
    public float DashCooldown { get { return baseDashCooldown * DashCooldownMultiplier; } }
    public float Luck { get { return baseLuck + baseLuck * luckMultiplier; } }

    public int MaxHealth { get { return Mathf.RoundToInt(baseHealth * HealthMultiplier + healthIncreasePerLevelUp * currentLevel); } }
    public int Health { get { return currentHealth; } }
    public int Armor { get { return temporaryArmor; } }

    public float AttackSpeedMultiplier { get { return attackSpeedMultiplier; } }

    public float AttackRangeMultiplier { get { return 1 + attackRangeMultiplier; } }
    public float EffectDurationMultiplier { get { return 1 + effectDurationMultiplier; } }
    public float EffectPowerMultiplier { get { return 1 + effectPowerMultiplier; } }
    public float BulletSpeedMultiplier { get { return 1 + bulletSpeedMultiplier; } }
    public float DashCooldownMultiplier { get { return 1 + dashCooldownMultiplier; } }
    public float HealthMultiplier { get { return 1 + healthMultiplier; } }

    public float ExperienceGainMultiplier {  get { return 1 + experienceGainMultiplier; } }
    public float GoldGainMultiplier {  get { return 1 + goldGainMultiplier; } }

    public float DoubleAttackChance { get { return doubleAttackChance; } }
    public float CriticalChance { get { return baseCriticalChance + baseCriticalChance * criticalChanceMultiplier; } }
    public float CriticalDamageMultiplier { get { return baseCriticalDamage + baseCriticalDamage * criticalDamageMultiplier; } }
    public float PickupRange { get { return basePickupRange + basePickupRange * pickupRangeMultiplier; } }

    public int CurrentExp { get { return currentExperience; } }
    public int CurrentLvl { get { return currentLevel; } }
    public int ExpToNextLvl { get { return experienceToNextLevel; } }

    #endregion

    private ObjectInteraction objectInteraction;
    private PlayerInventory playerInventory;

    private void Start()
    {
        currentHealth = MaxHealth;

        objectInteraction = GetComponentInChildren<ObjectInteraction>();
        playerInventory = GetComponent<PlayerInventory>();

    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        int amountToHeal = currentHealth + amount < MaxHealth ? amount : MaxHealth - currentHealth;
        currentHealth += amountToHeal;
        GameplayManager.Instance.CountHealingDone(amountToHeal);

        Vector3 positionVector;
        positionVector = new Vector3(transform.position.x + 9.8f, transform.position.y - 1.5f, transform.position.z);
        DamagePopup.Create(positionVector, amountToHeal.ToString(), DamagePopupOwner.PLAYER_HEAL);
        OnPlayerHeal?.Invoke();
    }

    public void TakeDamage(int damage)
    { 
        if (!CanTakeDamage || !IsAlive) return;

        int damageDealt = damage;
        if (temporaryArmor > 0)
            damageDealt = AbsorbDamage(damageDealt);

        if (damageDealt <= 0)
            return;
        
        currentHealth -= damageDealt;
        GameplayManager.Instance.CountDamageTaken(damageDealt);

        Vector3 positionVector;
        positionVector = new Vector3(transform.position.x + 9.8f, transform.position.y - 1.5f, transform.position.z);
        DamagePopup.Create(positionVector, damageDealt.ToString(), DamagePopupOwner.PLAYER_HIT);

        OnPlayerTakeDamage?.Invoke();

        if (currentHealth <= 0f)
        {
            currentHealth = 0;
            OnPlayerDeath?.Invoke();
            GameplayManager.Instance.EndGame();
            DeathScreenDisplay.Create();
        }
    }

    private int AbsorbDamage(int damage)
    {
        int absorbedDamage = damage <= temporaryArmor ? damage : temporaryArmor;
        temporaryArmor -= absorbedDamage;
        return damage - absorbedDamage;
    }

    public void AddArmor(int amount, float duration)
    {
        if (!IsAlive) return;

        StartCoroutine(TemporaryArmor(amount, duration));
    }

    private IEnumerator TemporaryArmor(int amount, float duration)
    {
        temporaryArmor += amount;
        yield return new WaitForSeconds(duration);
        temporaryArmor -= amount;

        if (temporaryArmor < 0)
            temporaryArmor = 0;
    }

    public void AddExperience(int value)
    {
        if (!IsAlive || !CanPickupExperience) return;

        int experienceGained = Mathf.RoundToInt(value * ExperienceGainMultiplier);
        currentExperience += experienceGained;
        GameplayManager.Instance.CountExperienceGained(experienceGained);

        OnPlayerExperiencePickup?.Invoke();

        if (experienceToNextLevel <= currentExperience)
        {
            currentExperience -= experienceToNextLevel;

            experienceToNextLevel += baseExperienceIncrease * currentLevel;

            bool luckyHeal = Random.Range(0f, 1f) <= Luck;
            Heal(luckyHeal ? healthRestoreOnLevelUp * 2 : healthRestoreOnLevelUp);

            currentLevel++;

            OnPlayerLevelUp?.Invoke();
        }
    }

    public void AddGold(int value)
    {
        if (!IsAlive) return;

        acquiredGold += Mathf.RoundToInt(value * GoldGainMultiplier);
        OnPlayerGoldPickup?.Invoke();
    }

    public void SetMultiplier(StatType type, float value)
    {
        switch (type)
        {
            case StatType.DOUBLE_ATTACK_CHANCE:
                doubleAttackChance = value;
                break;
            case StatType.ATTACK_SPEED:
                attackSpeedMultiplier = value;
                break;
            case StatType.CRITICAL_CHANCE:
                criticalChanceMultiplier = value;
                break;
            case StatType.CRITICAL_DAMAGE:
                criticalDamageMultiplier = value;
                break;
            case StatType.EFFECT_POWER:
                effectPowerMultiplier = value;
                break;
            case StatType.EFFECT_DURATION:
                effectDurationMultiplier = value;
                break;
            case StatType.BULLET_SPEED:
                bulletSpeedMultiplier = value;
                break;
            case StatType.ATTACK_RANGE:
                attackRangeMultiplier = value;
                UpdateAttackRanges();
                break;
            case StatType.DASH_COOLDOWN:
                dashCooldownMultiplier = value;
                break;
            case StatType.LUCK:
                luckMultiplier = value;
                break;
            case StatType.PICKUP_RANGE:
                pickupRangeMultiplier = value;
                objectInteraction.UpdatePickupRange(PickupRange);
                break;
            case StatType.HEALTH:
                healthMultiplier = value;
                OnPlayerMaxHealthChange?.Invoke();
                break;
            case StatType.EXPERIENCE_GAIN:
                experienceGainMultiplier = value;
                break;
            case StatType.GOLD_GAIN:
                goldGainMultiplier = value;
                break;
        }
    }

    private void UpdateAttackRanges()
    {
        playerInventory.UpdateWeaponRanges();
    }
}
