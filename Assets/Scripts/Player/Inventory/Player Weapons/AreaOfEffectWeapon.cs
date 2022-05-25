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

    public override void SetCurrentlyUsed()
    {
        weaponWasUsed += Spawn;
    }

    public override void SetNotCurrentlyUsed()
    {
        weaponWasUsed -= Spawn;
    }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;

        weaponWasUsed?.Invoke();
    }

    public void UseEffects(AreaOfEffect aoe)
    {
        foreach (Enemy enemy in enemiesInRange)
        {
            primaryEffect.Use(currentLevel, player, enemy);
            secondaryEffect.Use(currentLevel, player, enemy);
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
            yield return new WaitForSeconds(damageInterval);

            if (aoe.CanApplyEffects)
                UseEffects(aoe);
        }

        aoe.Despawn();
    }
}
