using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyWithoutHurtAnimation : Enemy
{
    protected override void Attack()
    {
        if (!playerInRange)
            return;

        anim.SetTrigger(ENEMY_ATTACK_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(attack.length));

        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;
    }

    public override void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        health -= damage;
        OnEnemyTakeDamage?.Invoke();
        //Debug.Log($"{gameObject.name} took {damage} damage");

        if (health <= 0)
        {
            health = 0;
            canTakeDamage = false;
            canMove = false;
            isAlive = false;
            //Debug.Log($"{gameObject.name} died");

            anim.SetTrigger(ENEMY_DEATH_TRIGGER);
            OnEnemyDeath?.Invoke();
        }
    }

    public override void DealDamage(bool isSpecial)
    {
        if (!playerInRange) return;

        player.TakeDamage(damage);
    }
}
