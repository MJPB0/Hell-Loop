using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon effect", menuName = "Weapon effect", order = 0)]
public class WeaponEffect : ScriptableObject, IWeaponEffect
{
    [SerializeField] private EffectType effectType;

    [Space]
    [SerializeField] private float duration;
    [SerializeField] private int power;
    [SerializeField] private float timeBetweenAttacks = 1f;

    public float Duration { get { return duration; } }
    public EffectType EffectType { get { return effectType; } }

    public void Use(Player player, Enemy enemy, WeaponName weaponName)
    {
        switch (effectType)
        {
            case EffectType.DAMAGE:
                DealDamage(enemy, player, weaponName);
                break;
            case EffectType.DOT:
                ApplyDamageOverTime(enemy, player, weaponName);
                break;
            case EffectType.SLOW:
                ApplySlow(enemy, player);
                break;
            case EffectType.KNOCKBACK:
                KnockBack(enemy, player);
                break;
            case EffectType.PARRY:
                Parry(player);
                break;
            case EffectType.LIFE_STEAL:
                StealLife(enemy, player, weaponName);
                break;
            case EffectType.HOLY_SLASH:
                HolyStrike(enemy, player);
                break;
        }
    }

    private void DealDamage(Enemy target, Player player, WeaponName weaponName)
    {
        bool isCritical = Random.Range(0f, 1f) <= player.CriticalChance;
        bool willDoubleAttack = Random.Range(0f, 1f) <= player.DoubleAttackChance;
        int damage = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        damage = isCritical ? Mathf.RoundToInt(damage * player.CriticalDamageMultiplier) : damage;

        target.TakeDamage(damage, weaponName);

        // TODO refactor
        if (player.CanDoubleAttack && willDoubleAttack)
        {
            target.TakeDamage(damage, weaponName);
            player.GetComponentInChildren<PlayerEffects>().PlayerDoubleAttack(target.transform.position);
        }

        SoundManager.PlayEnemyDamageSound();

        Vector3 positionVector;
        positionVector = new Vector3(target.transform.position.x + 9.8f, target.transform.position.y - 1.5f, target.transform.position.z);
        DamagePopup.Create(positionVector, damage.ToString(), isCritical ? DamagePopupOwner.ENEMY_CRITICAL_HIT : DamagePopupOwner.ENEMY_HIT);
    }

    private void ApplyDamageOverTime(Enemy target, Player player, WeaponName weaponName)
    {
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        target.ApplyDamageOverTime(pow, dur, timeBetweenAttacks, weaponName);
    }

    private void ApplySlow(Enemy target, Player player)
    {
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        target.ApplySlow(pow, dur);
    }

    private void KnockBack(Enemy target, Player player)
    {
        target.KnockBack(Mathf.RoundToInt(power * player.EffectPowerMultiplier));
    }

    private void Parry(Player player)
    {
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        player.AddArmor(pow, dur);
    }

    private void StealLife(Enemy target, Player player, WeaponName weaponName)
    {
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        target.TakeDamage(pow, weaponName);
        player.Heal(pow);
    }

    private void HolyStrike(Enemy target, Player player)
    {
        // TODO refactor
        player.GetComponentInChildren<PlayerEffects>().PlayerHolyAttack(target.transform.position);
        target.TakeDamage(Mathf.RoundToInt(power * player.EffectPowerMultiplier) + player.Armor, WeaponName.HOLY_SWORD);
    }
}