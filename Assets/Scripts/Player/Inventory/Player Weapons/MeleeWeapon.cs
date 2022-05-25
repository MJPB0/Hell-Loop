using System.Collections;
using UnityEngine;

public class MeleeWeapon : PlayerWeapon
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (!enemiesInRange.Contains(obj))
            enemiesInRange.Add(obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (enemiesInRange.Contains(obj))
            enemiesInRange.Remove(obj);
    }

    public override void Use()
    {
        canAttack = false;
        timeToNextAttack = timeBetweenAttacks;

        weaponWasUsed?.Invoke();
    }

    private void UsePrimaryEffect()
    {
        if (enemiesInRange.Count <= 0) return;

        foreach (Enemy enemy in enemiesInRange)
            primaryEffect.Use(currentLevel, player, enemy);
    }

    private void UseSecondaryEffect()
    {
        if (enemiesInRange.Count <= 0) return;

        foreach (Enemy enemy in enemiesInRange)
            secondaryEffect.Use(currentLevel, player, enemy);
    }

    private void UseParry()
    {
        secondaryEffect.Use(currentLevel, player, null);
    }

    public override void SetCurrentlyUsed()
    {
        rangeCollider.enabled = true;
        weaponWasUsed += UsePrimaryEffect;
        if (secondaryEffect.EffectType == EffectType.PARRY)
            player.OnPlayerTakeDamage += UseParry;
        else
            weaponWasUsed += UseSecondaryEffect;
    }

    public override void SetNotCurrentlyUsed()
    {
        rangeCollider.enabled = false;
        weaponWasUsed -= UsePrimaryEffect;
        if (secondaryEffect.EffectType == EffectType.PARRY)
            player.OnPlayerTakeDamage -= UseParry;
        else
            weaponWasUsed -= UseSecondaryEffect;
    }
}