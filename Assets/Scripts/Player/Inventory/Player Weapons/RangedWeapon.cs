using System.Collections;
using UnityEngine;

public class RangedWeapon : PlayerWeapon
{
    [Space]
    [Header("Ranged")]
    [SerializeField] private float projectileSpeed;

    [Space]
    [SerializeField] private GameObject projectile;

    [Space]
    [SerializeField] private bool hasEnemyCap;
    [SerializeField] private int maxEnemiesHit;

    public int MaxEnemiesHit { get { return maxEnemiesHit; } set { maxEnemiesHit = value; } }
    public bool HasEnemyCap { get { return hasEnemyCap; } }
    public float ProjectileSpeed { get { return projectileSpeed; } }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;

        weaponWasUsed?.Invoke();
    }

    public void UseEffects(Enemy enemy)
    {
        primaryEffect.Use(currentLevel, player, enemy);
        secondaryEffect.Use(currentLevel, player, enemy);
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
        weaponWasUsed += ShootProjectile;
    }

    public override void SetNotCurrentlyUsed()
    {
        weaponWasUsed -= ShootProjectile;
    }
}