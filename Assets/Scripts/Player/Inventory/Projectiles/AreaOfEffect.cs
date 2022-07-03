using System.Collections;
using UnityEngine;

public class AreaOfEffect : MonoBehaviour
{
    [SerializeField] private CircleCollider2D aoeCollider;
    [SerializeField] private float bodySizeMultiplier = 3f;

    private AreaOfEffectWeapon playerWeapon;

    [Space]
    [SerializeField] private bool hasLimitedEffectsApplyments = false;
    [SerializeField] private bool canApplyEffects = true;
    [SerializeField] private float timesToApplyEffects = 10;
    [SerializeField] private float timesEffectsWereApplied = 0;

    public bool HasLimitedEffectApplyments { get { return hasLimitedEffectsApplyments; } }
    public bool CanApplyEffects { get { return canApplyEffects; } }
    public float TimesToApplyEffects { get { return timesEffectsWereApplied; } }
    public float TimesEffectsWereApplied { get { return timesEffectsWereApplied; } }

    public void EffectsApplied()
    {
        timesEffectsWereApplied++;

        if (hasLimitedEffectsApplyments && timesEffectsWereApplied >= timesToApplyEffects)
            canApplyEffects = false;
    }

    public void Spawn(AreaOfEffectWeapon weapon)
    {
        playerWeapon = weapon;
        aoeCollider.enabled = true;
        aoeCollider.radius = weapon.AttackRange;
        Debug.Log(weapon.AttackRange);

        GetComponentInChildren<SpriteRenderer>().transform.localScale = Vector3.one * bodySizeMultiplier * weapon.AttackRange;
    }

    public void Despawn()
    {
        aoeCollider.enabled = false;

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (!playerWeapon.Enemies.Contains(obj))
            playerWeapon.Enemies.Add(obj);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        Enemy obj = collision.gameObject.GetComponent<Enemy>();

        if (playerWeapon.Enemies.Contains(obj))
            playerWeapon.Enemies.Remove(obj);
    }

}