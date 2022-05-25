using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : Enemy
{
    private const string ENEMY_PROJECTILES_TAG = "Enemy Projectiles Parent";

    [Space]
    [SerializeField] private Transform enemyProjectilesParent;
    [SerializeField] private GameObject projectile;

    [Space]
    [SerializeField] private float projectileSpeed;

    [Space]
    [SerializeField] private Transform shootPos;

    public float ProjectileSpeed { get { return projectileSpeed; } }

    private void Start()
    {
        base.Start();
        enemyProjectilesParent = GameObject.FindGameObjectWithTag(ENEMY_PROJECTILES_TAG).transform;
    }

    public override void DealDamage(bool isSpecial)
    {
        if (!playerInRange) return;

        SpawnProjectile();
    }

    public override void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        health -= damage;
        anim.SetTrigger(ENEMY_GET_HIT_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(getHit.length));
        OnEnemyTakeDamage?.Invoke();
        //Debug.Log($"{gameObject.name} took {damage} damage");

        if (health <= 0)
        {
            health = 0;
            canTakeDamage = false;
            canMove = false;
            isAlive = false;

            anim.SetTrigger(ENEMY_DEATH_TRIGGER);
            OnEnemyDeath?.Invoke();
        }
    }

    protected override void Attack()
    {
        anim.SetTrigger(ENEMY_ATTACK_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(attack.length));

        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;
    }

    private void SpawnProjectile()
    {
        GameObject proj = Instantiate(projectile, shootPos.position, Quaternion.identity, enemyProjectilesParent);

        EnemyProjectile obj = proj.GetComponent<EnemyProjectile>();

        Vector2 direction = (player.transform.position - proj.transform.position).normalized;
        Transform objBody = obj.GetComponentInChildren<SpriteRenderer>().transform;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objBody.transform.Rotate(new Vector3(0, 0, angle));

        obj.Shoot(direction, this, player);
    }
}
