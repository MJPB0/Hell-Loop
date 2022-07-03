using System.Collections;
using UnityEngine;

public class RangedWeapon : PlayerWeapon
{
    [Space]
    [Header("Ranged")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float additionalProjectileSpeed;

    [Space]
    [SerializeField] private GameObject projectile;

    [Space]
    [SerializeField] private bool hasEnemyCap;
    [SerializeField] private int maxEnemiesHit;

    public int MaxEnemiesHit { get { return maxEnemiesHit; } set { maxEnemiesHit = value; } }
    public bool HasEnemyCap { get { return hasEnemyCap; } }
    public float ProjectileSpeed { get { return projectileSpeed + additionalProjectileSpeed; } }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks - AttackSpeed;

        WeaponIsUsed?.Invoke();
    }

    public void UseEffects(Enemy enemy)
    {
        primaryEffect.Use(player, enemy, weaponName);
        secondaryEffect.Use(player, enemy, weaponName);
    }

    private void ShootProjectile()
    {
        GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity, projectilesParent);

        proj.transform.tag = TAG_AFTER_PICKUP;
        proj.layer = LayerMask.NameToLayer(LAYER_AFTER_PICKUP);

        WeaponProjectile obj = proj.GetComponent<WeaponProjectile>();

        Vector2 direction = controller.MouseDirection();
        Transform objBody = obj.GetComponentInChildren<SpriteRenderer>().transform;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objBody.transform.Rotate(new Vector3(0, 0, angle));

        obj.Shoot(direction, player.BulletSpeedMultiplier, this);
    }

    public override void SetCurrentlyUsed()
    {
        WeaponIsUsed += ShootProjectile;
    }

    public override void SetNotCurrentlyUsed()
    {
        WeaponIsUsed -= ShootProjectile;
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
            case WeaponStatType.PROJECTILE_SPEED:
                additionalProjectileSpeed += upgrade.Value;
                break;
            case WeaponStatType.ENEMY_HIT_CAP:
                maxEnemiesHit += Mathf.RoundToInt(upgrade.Value);
                break;
        }

        if (upgrade.PrimaryEffectUpgrade != null)
            primaryEffect = upgrade.PrimaryEffectUpgrade;
        if (upgrade.SecondaryEffectUpgrade != null)
            secondaryEffect = upgrade.SecondaryEffectUpgrade;
    }
}