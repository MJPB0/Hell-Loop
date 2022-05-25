using System.Collections;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    [SerializeField] private bool canDoDamage = true;
    [SerializeField] private bool isMoving = false;

    [Space]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector2 projectileDirection;

    [Space]
    [SerializeField] private bool bouncesOfObstacles;
    [SerializeField] private int bounceCount;

    [Space]
    [SerializeField] private int enemiesHit = 0;

    private float bulletSpeedMultiplier;

    private RangedWeapon playerWeapon;

    private void Update()
    {
        if (isMoving)
            MoveProjectile();
    }

    public void Shoot(Vector2 direction, float speedMultiplier, RangedWeapon weapon)
    {
        initialPosition = transform.position;
        isMoving = true;
        projectileDirection = direction;
        playerWeapon = weapon;
        bulletSpeedMultiplier = speedMultiplier;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            EnemyHit(collision.gameObject.GetComponent<Enemy>());
        else if (collision.CompareTag("Obstacle"))
            Bounce();
    }

    private void MoveProjectile()
    {
        if (Vector3.Distance(initialPosition, transform.position) >= playerWeapon.AttackRange) 
            Destroy(gameObject);

        transform.Translate(new Vector3(projectileDirection.x, projectileDirection.y, 0) * playerWeapon.ProjectileSpeed * bulletSpeedMultiplier * Time.deltaTime);
    }

    private void EnemyHit(Enemy enemy)
    {
        if (!canDoDamage) return;

        playerWeapon.UseEffects(enemy);

        enemiesHit++;

        if (playerWeapon.OnHitEffect != null)
            enemy.EnemyEffectsController.SpawnEffect(playerWeapon.OnHitEffect, enemy.transform.position, playerWeapon.OnHitEffectAnimation.length);

        if (playerWeapon.MaxEnemiesHit == enemiesHit && playerWeapon.HasEnemyCap)
        {
            canDoDamage = false;
            Destroy(gameObject);
        }
    }

    private void Bounce()
    {
        if (bounceCount == 0 || !bouncesOfObstacles)
            Destroy(gameObject);

        Debug.Log("Projectile bounced");

        // TODO bounce logic
        projectileDirection *= -1;
        bounceCount--;
    }
}