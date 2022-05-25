using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private bool canDoDamage = true;
    [SerializeField] private bool isMoving = false;

    [Space]
    [SerializeField] private Vector3 initialPosition;
    [SerializeField] private Vector2 projectileDirection;

    private RangedEnemy enemy;
    private Player player;

    private void Update()
    {
        if (isMoving)
            MoveProjectile();
    }

    public void Shoot(Vector2 direction, RangedEnemy enemy, Player player)
    {
        initialPosition = transform.position;
        isMoving = true;
        this.enemy = enemy;
        projectileDirection = direction;
        this.player = player;
    }

    private void MoveProjectile()
    {
        if (Vector3.Distance(initialPosition, transform.position) >= enemy.AttackRange)
            Destroy(gameObject);

        transform.Translate(enemy.ProjectileSpeed * Time.deltaTime * new Vector3(projectileDirection.x, projectileDirection.y, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            DealDamage();
        }
    }

    private void DealDamage()
    {
        if (!canDoDamage) return;

        player.TakeDamage(enemy.Damage);
        Destroy(gameObject);
    }
}
