using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWithSpecialAttack : Enemy
{
    private const string ENEMY_SPECIAL_ATTACK_TRIGGER = "Special Attack";

    [Space]
    [Header("Special attack")]
    [SerializeField] private int specialAttackDamageMultiplier = 2;

    [Space]
    [SerializeField] private bool canUseSpecialAttack = false;
    [SerializeField] private float timeBetweenSpecialAttacks = 5f;
    [SerializeField] private float timeToNextSpecialAttack = 0f;

    [Space]
    [SerializeField] private AnimationClip specialAttack;

    protected void Update()
    {
        base.Update();

        if (!canUseSpecialAttack)
            CanUseSpecialAttack();
    }

    private void CanUseSpecialAttack()
    {
        if (timeToNextSpecialAttack > 0)
            timeToNextSpecialAttack -= Time.deltaTime * attackSpeed;
        else
        {
            timeToNextSpecialAttack = 0;
            canUseSpecialAttack = true;
        }
    }

    protected override void Attack()
    {
        if (!playerInRange)
            return;

        if (!canUseSpecialAttack)
            BaseAttack();
        else
            SpecialAttack();
    }

    private void BaseAttack()
    {
        anim.SetTrigger(ENEMY_ATTACK_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(attack.length));

        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;
    }

    private void SpecialAttack()
    {
        anim.SetTrigger(ENEMY_SPECIAL_ATTACK_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(specialAttack.length));

        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;

        canUseSpecialAttack = false;
        timeToNextSpecialAttack = timeBetweenSpecialAttacks;
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

    public override void DealDamage(bool isSpecial)
    {
        if (!playerInRange) return;

        if (isSpecial)
            player.TakeDamage(damage * specialAttackDamageMultiplier);
        else
            player.TakeDamage(damage);
    }
}
