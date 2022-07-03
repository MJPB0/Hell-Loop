using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MeleeWeapon : PlayerWeapon
{
    [Space]
    [SerializeField] private GameObject weaponEffect;
    [SerializeField] private AnimationClip weaponEffectClip;

    [Space]
    [SerializeField] private Vector3 effectPositionOffset;

    public GameObject WeaponEffect { get { return weaponEffect; } }
    public AnimationClip WeaponEffectClip { get { return weaponEffectClip; } }
    public Vector3 EffectPositionOffset { get { return effectPositionOffset; } }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (!enemiesInRange.Contains(obj))
            enemiesInRange.Add(obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (enemiesInRange.Contains(obj))
            enemiesInRange.Remove(obj);
    }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks - AttackSpeed;

        WeaponIsUsed?.Invoke();
    }

    private void UsePrimaryEffect()
    {
        if (enemiesInRange.Count <= 0) return;

        foreach (Enemy enemy in enemiesInRange)
            primaryEffect.Use(player, enemy, weaponName);
    }

    private void UseSecondaryEffect()
    {
        if (enemiesInRange.Count <= 0) return;

        foreach (Enemy enemy in enemiesInRange)
            secondaryEffect.Use(player, enemy, weaponName);
    }

    private void UseParry()
    {
        secondaryEffect.Use(player, null, weaponName);
    }

    public override void SetCurrentlyUsed()
    {
        rangeCollider.enabled = true;
        WeaponIsUsed += UsePrimaryEffect;
        if (secondaryEffect.EffectType == EffectType.PARRY)
            player.OnPlayerTakeDamage += UseParry;
        else
            WeaponIsUsed += UseSecondaryEffect;
    }

    public override void SetNotCurrentlyUsed()
    {
        rangeCollider.enabled = false;
        WeaponIsUsed -= UsePrimaryEffect;
        if (secondaryEffect.EffectType == EffectType.PARRY)
            player.OnPlayerTakeDamage -= UseParry;
        else
            WeaponIsUsed -= UseSecondaryEffect;
    }

    protected override void ApplyUpgrades()
    {
        WeaponLevelUpgradeSO upgrade = levelUpgrades[currentLevel - 2];
        switch (upgrade.StatType)
        {
            case WeaponStatType.ATTACK_RANGE:
                additionalAttackRange += upgrade.Value;
                break;
            case WeaponStatType.ATTACK_SPEED:
                additionalAttackSpeed += upgrade.Value;
                break;
        }

        if (upgrade.PrimaryEffectUpgrade != null)
            primaryEffect = upgrade.PrimaryEffectUpgrade;
        if (upgrade.SecondaryEffectUpgrade != null)
            secondaryEffect = upgrade.SecondaryEffectUpgrade;
    }
}