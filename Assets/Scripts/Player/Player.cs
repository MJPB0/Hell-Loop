using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public UnityAction OnPlayerDash;
    public UnityAction OnPlayerTakeDamage;
    public UnityAction OnPlayerDeath;
    public UnityAction OnPlayerExperiencePickup;
    public UnityAction OnPlayerGoldPickup;
    public UnityAction OnPlayerLevelUp;

    #region Player stats

    [Header("Health")]
    [SerializeField] private int baseHealth = 100;
    [SerializeField] private int currentHealth = 100;

    [Space]
    [SerializeField] private float baseHealthMultiplier = 1;
    [SerializeField] private float healthMultiplier = 0;

    [SerializeField] private int temporaryArmor = 0;

    [Header("Combat")]
    [SerializeField] private float baseAttackSpeedMultiplier = 1f;
    [SerializeField] private float attackSpeedMultiplier = 1f;

    [Space]
    [SerializeField] private float baseCriticalChance = .1f;
    [SerializeField] private float baseCriticalChanceMultiplier = 1f;
    [SerializeField] private float criticalChanceMultiplier = 1f;

    [Space]
    [SerializeField] private float baseCriticalDamage = 2f;
    [SerializeField] private float baseCriticalDamageMultiplier = 1f;
    [SerializeField] private float criticalDamageMultiplier = 1f;

    [Space]
    [SerializeField] private float baseEffectDurationMultiplier = 1f;
    [SerializeField] private float effectDurationMultiplier = 1f;

    [Space]
    [SerializeField] private float baseEffectPowerMultiplier = 1f;
    [SerializeField] private float effectPowerMultiplier = 1f;

    [Space]
    [SerializeField] private float baseBulletSpeedMultiplier = 1f;
    [SerializeField] private float bulletSpeedMultiplier = 1f;

    [Space]
    [SerializeField] private float baseAttackRangeMultiplier = 1f;
    [SerializeField] private float attackRangeMultiplier = 1f;

    [Header("Movement")]
    [SerializeField] private float baseMovementSpeed = 4f;
    [SerializeField] private float baseMovementSpeedMultiplier = 1f;
    [SerializeField] private float movementSpeedMultiplier = 1f;

    [Space]
    public float DashStrength = 30f;
    public float DashDuration = 5f;
    public float DashDaze = .5f;

    [Space]
    public float TimeToNextDash = 0f;
    [SerializeField] private float baseDashCooldown = 4f;
    [SerializeField] private float baseDashCooldownMultiplier = 1f;
    [SerializeField] private float dashCooldownMultiplier = 1f;

    [Header("Weapons' masteries")]
    [SerializeField] private int knifeMastery = 1;
    [SerializeField] private int swordMasteryLevel = 1;
    [SerializeField] private int tomahawkMasteryLevel = 1;
    [SerializeField] private int axeMasteryLevel = 1;

    [Space]
    [SerializeField] private int iceWandMasteryLevel = 1;
    [SerializeField] private int fireWandMasteryLevel = 1;
    [SerializeField] private int airWandMasteryLevel = 1;
    [SerializeField] private int earthWandMasteryLevel = 1;

    [Space]
    [SerializeField] private int pistolMasteryLevel = 1;
    [SerializeField] private int shotgunMasteryLevel = 1;
    [SerializeField] private int rifleMasteryLevel = 1;
    [SerializeField] private int sniperMasteryLevel = 1;

    [Header("Other")]
    [SerializeField] private float baseLuck = .1f;
    [SerializeField] private float luckMultiplier = 1f;

    [Space]
    [SerializeField] private float basePickupRange = 1f;
    [SerializeField] private float basePickupRangeMultiplier = 1f;
    [SerializeField] private float pickupRangeMultiplier = 1f;

    [Header("Experience")]
    [SerializeField] private float baseExperienceGainMultiplier = 1;
    [SerializeField] private float experienceGainMultiplier = 1;

    [Space]
    [SerializeField] private int currentExperience = 0;
    [SerializeField] private int experienceToNextLevel = 100;
    [SerializeField] private int currentLevel = 1;

    [Header("Gold")]
    [SerializeField] private float baseGoldGainMultiplier = 1;
    [SerializeField] private float goldGainMultiplier = 1;

    [Space]
    [SerializeField] private int acquiredGold = 0;

    #endregion

    [Header("Checks")]
    public bool IsAlive = true;
    public bool CanTakeDamage = true;

    [Space]
    public bool IsDashing = false;

    [Space]
    public bool CanMove = true;
    public bool CanDash = true;

    [Space]
    public bool IsAbleToEvolveWeapon = false;

    #region Accessors

    public float MovementSpeed { get { return baseMovementSpeed * (baseMovementSpeedMultiplier + movementSpeedMultiplier); } }
    public float DashCooldown { get { return baseDashCooldown * (baseDashCooldown + dashCooldownMultiplier); } }

    public int MaxHealth { get { return Mathf.RoundToInt(baseHealth * (baseHealthMultiplier + healthMultiplier)); } }
    public int Health { get { return currentHealth; } }
    public int Armor { get { return temporaryArmor; } }

    public float AttackSpeedMultiplier { get { return baseAttackSpeedMultiplier + attackSpeedMultiplier; } }
    public float EffectDurationMultiplier { get { return baseEffectDurationMultiplier + effectDurationMultiplier; } }
    public float EffectPowerMultiplier { get { return baseEffectPowerMultiplier + effectPowerMultiplier; } }
    public float BulletSpeedMultiplier { get { return baseBulletSpeedMultiplier + bulletSpeedMultiplier; } }
    public float AttackRangeMultiplier { get { return baseAttackRangeMultiplier + attackRangeMultiplier; } }

    public float CriticalChance { get { return baseCriticalChance * (baseCriticalChanceMultiplier + criticalChanceMultiplier); } }
    public float CriticalDamageMultiplier { get { return baseCriticalDamage * (baseCriticalDamageMultiplier + criticalDamageMultiplier); } }
    public float PickupRange { get { return basePickupRange * (basePickupRangeMultiplier + pickupRangeMultiplier); } }

    #endregion

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;

        int amountToHeal = currentHealth + amount < MaxHealth ? amount : MaxHealth - currentHealth;
        currentHealth += amountToHeal;
        //Debug.Log($"Player healed {amountToHeal} hp");
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
        OnPlayerTakeDamage?.Invoke();

        if (currentHealth <= 0f)
        {
            // TODO Player death
            currentHealth = 0;
            OnPlayerDeath?.Invoke();
            GameplayManager.Instance.EndGame();
        }

        //Debug.Log($"Player took {damageDealt} damage!");
        //Debug.Log($"Player has {currentHealth}hp");
    }

    private int AbsorbDamage(int damage)
    {
        int absorbedDamage = damage <= temporaryArmor ? damage : temporaryArmor;
        //Debug.Log($"Player absorbed {absorbedDamage} damage!");
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
        if (!IsAlive) return;

        currentExperience += Mathf.RoundToInt(value * (baseExperienceGainMultiplier + experienceGainMultiplier));
        OnPlayerExperiencePickup?.Invoke();

        if (experienceToNextLevel <= currentExperience)
        {
            currentExperience -= experienceToNextLevel;
            // TODO experience needed to lvlup
            experienceToNextLevel *= 2;
            currentLevel++;

            OnPlayerLevelUp?.Invoke();
        }
    }

    public void AddGold(int value)
    {
        if (!IsAlive) return;

        acquiredGold += Mathf.RoundToInt(value * (baseGoldGainMultiplier + goldGainMultiplier));
        OnPlayerGoldPickup?.Invoke();
    }

    public void SetMultiplier(MultiplierType type, float value)
    {
        switch (type)
        {
            case MultiplierType.MOVEMENT_SPEED:
                movementSpeedMultiplier = value;
                break;
            case MultiplierType.ATTACK_SPEED:
                attackSpeedMultiplier = value;
                break;
            case MultiplierType.CRITICAL_CHANCE:
                criticalChanceMultiplier = value;
                break;
            case MultiplierType.CRITICAL_DAMAGE:
                criticalDamageMultiplier = value;
                break;
            case MultiplierType.EFFECT_POWER:
                effectPowerMultiplier = value;
                break;
            case MultiplierType.EFFECT_DURATION:
                effectDurationMultiplier = value;
                break;
            case MultiplierType.BULLET_SPEED:
                bulletSpeedMultiplier = value;
                break;
            case MultiplierType.ATTACK_RANGE:
                attackRangeMultiplier = value;
                break;
            case MultiplierType.DASH_COOLDOWN:
                dashCooldownMultiplier = value;
                break;
            case MultiplierType.LUCK:
                luckMultiplier = value;
                break;
            case MultiplierType.PICKUP_RANGE:
                pickupRangeMultiplier = value;
                break;
            case MultiplierType.HEALTH:
                healthMultiplier = value;
                break;
            case MultiplierType.EXPERIENCE_GAIN:
                experienceGainMultiplier = value;
                break;
            case MultiplierType.GOLD_GAIN:
                goldGainMultiplier = value;
                break;
        }
    }
}
