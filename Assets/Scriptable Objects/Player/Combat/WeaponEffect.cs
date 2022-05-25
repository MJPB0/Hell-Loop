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

    public void Use(int level, Player player, Enemy enemy)
    {
        switch (effectType)
        {
            case EffectType.DAMAGE:
                DealDamage(enemy, player, level);
                break;
            case EffectType.DOT:
                ApplyDamageOverTime(enemy, player, level);
                break;
            case EffectType.SLOW:
                ApplySlow(enemy, player, level);
                break;
            case EffectType.KNOCKBACK:
                KnockBack(enemy, player, level);
                break;
            case EffectType.PARRY:
                Parry(player, level);
                break;
            case EffectType.LIFE_STEAL:
                StealLife(enemy, player, level);
                break;
            case EffectType.HOLY_SLASH:
                HolyStrike(enemy, player, level);
                break;
        }
    }

    private void DealDamage(Enemy target, Player player, int level)
    {
        // TODO level dependence
        target.TakeDamage(Mathf.RoundToInt(power * player.EffectPowerMultiplier));
    }

    private void ApplyDamageOverTime(Enemy target, Player player, int level)
    {
        // TODO level dependence
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        target.ApplyDamageOverTime(pow, dur, timeBetweenAttacks);
    }

    private void ApplySlow(Enemy target, Player player, int level)
    {
        // TODO level dependence
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        target.ApplySlow(pow, dur);
    }

    private void KnockBack(Enemy target, Player player, int level)
    {
        // TODO level dependence
        target.KnockBack(Mathf.RoundToInt(power * player.EffectPowerMultiplier));
    }

    private void Parry(Player player, int level)
    {
        // TODO level dependence
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        int dur = Mathf.RoundToInt(duration * player.EffectDurationMultiplier);
        player.AddArmor(pow, dur);
    }

    private void StealLife(Enemy target, Player player, int level)
    {
        // TODO level dependence
        int pow = Mathf.RoundToInt(power * player.EffectPowerMultiplier);
        target.TakeDamage(pow);
        player.Heal(pow);
    }

    private void HolyStrike(Enemy target, Player player, int level)
    {
        // TODO level dependence
        target.TakeDamage(Mathf.RoundToInt(power * player.EffectPowerMultiplier) + player.Armor);
    }
}