using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaOfEffectWeapon : PlayerWeapon
{
    [Space]
    [Header("Area of effect")]
    [SerializeField] private GameObject areaOfEffect;

    [Space]
    [SerializeField] private float damageInterval;
    [SerializeField] private float damageIntervalDecrease;

    public override void SetCurrentlyUsed()
    {
        WeaponIsUsed += Spawn;
    }

    public override void SetNotCurrentlyUsed()
    {
        WeaponIsUsed -= Spawn;
    }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks - AttackSpeed;

        WeaponIsUsed?.Invoke();
    }

    public void UseEffects(AreaOfEffect aoe)
    {
        foreach (Enemy enemy in enemiesInRange)
        {
            primaryEffect.Use(player, enemy, weaponName);
            secondaryEffect.Use(player, enemy, weaponName);
            aoe.EffectsApplied();

            if (onHitEffect != null)
                enemy.EnemyEffectsController.SpawnEffect(onHitEffect, enemy.transform.position, onHitEffectAnimation.length);
        }
    }

    private void Spawn()
    {
        PlayerController controller = GetComponentInParent<PlayerController>();
        Vector2 pos = controller.MousePosition();

        GameObject proj = Instantiate(areaOfEffect, pos, Quaternion.identity, projectilesParent);

        AreaOfEffect obj = proj.GetComponent<AreaOfEffect>();
        obj.Spawn(this);

        StartCoroutine(WaitAndDespawn(obj));
    }

    private IEnumerator WaitAndDespawn(AreaOfEffect aoe)
    {
        float startTime = Time.time;
        while (Time.time < startTime + primaryEffect.Duration)
        {
            yield return new WaitForSeconds(damageInterval - damageIntervalDecrease);

            if (aoe.CanApplyEffects)
                UseEffects(aoe);
        }

        aoe.Despawn();
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
            case WeaponStatType.DAMAGE_INTERVAL:
                damageIntervalDecrease += upgrade.Value;
                break;
        }

        if (upgrade.PrimaryEffectUpgrade != null)
            primaryEffect = upgrade.PrimaryEffectUpgrade;
        if (upgrade.SecondaryEffectUpgrade != null)
            secondaryEffect = upgrade.SecondaryEffectUpgrade;
    }
}
