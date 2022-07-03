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
        timeToNextAttack = timeBetweenAttacks - attackTimeReduction - attackSpeedIncreasePerLevel * level;
    }

    public override void TakeDamage(int damage, WeaponName weaponName)
    {
        if (!canTakeDamage) return;

        health -= damage;
        GameplayManager.Instance.CountDamageDealt(damage, weaponName);

        OnEnemyTakeDamage?.Invoke();

        if (health <= 0)
        {
            health = 0;
            canTakeDamage = false;
            canMove = false;
            isAlive = false;

            GameplayManager.Instance.AddEnemyKilled();

            anim.SetTrigger(ENEMY_DEATH_TRIGGER);
            OnEnemyDeath?.Invoke();
        }
    }

    public override void DealDamage(bool isSpecial)
    {
        if (!playerInRange) return;

        player.TakeDamage(Damage);
    }
}
