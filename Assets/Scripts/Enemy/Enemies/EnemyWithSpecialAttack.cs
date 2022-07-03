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
            timeToNextSpecialAttack -= Time.deltaTime * attackTimeReduction;
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
        timeToNextAttack = timeBetweenAttacks - attackTimeReduction - attackSpeedIncreasePerLevel * level;
    }

    private void SpecialAttack()
    {
        anim.SetTrigger(ENEMY_SPECIAL_ATTACK_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(specialAttack.length));

        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;

        canUseSpecialAttack = false;
        timeToNextSpecialAttack = timeBetweenSpecialAttacks - attackTimeReduction - attackSpeedIncreasePerLevel * level;
    }

    public override void TakeDamage(int damage, WeaponName weaponName)
    {
        if (!canTakeDamage) return;

        health -= damage;
        GameplayManager.Instance.CountDamageDealt(damage, weaponName);

        anim.SetTrigger(ENEMY_GET_HIT_TRIGGER);
        StartCoroutine(WaitAndEnableMovement(getHit.length));
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

        if (isSpecial)
            player.TakeDamage(Damage * specialAttackDamageMultiplier );
        else
            player.TakeDamage(Damage);
    }
}
