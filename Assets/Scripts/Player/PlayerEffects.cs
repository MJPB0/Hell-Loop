using System.Collections;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private const string PLAYER_VFX_PARENT_TAG = "Player Effects";
    private const string STATIC_PLAYER_VFX_PARENT_TAG = "Player Static Effects";

    private Player player;
    private PlayerInventory inventory;
    private Transform playerBody;

    [Header("VFX")]
    [SerializeField] private GameObject levelup;
    [SerializeField] private GameObject bleed;
    [SerializeField] private GameObject dash;

    [Space]
    [SerializeField] private GameObject doubleAttack;
    [SerializeField] private GameObject holyStrike;

    [Header("VFX clips")]
    [SerializeField] private AnimationClip levelupClip;
    [SerializeField] private AnimationClip bleedClip;
    [SerializeField] private AnimationClip dashClip;

    [Space]
    [SerializeField] private AnimationClip doubleAttackClip;
    [SerializeField] private AnimationClip holyStrikeClip;

    [Header("Current weapon")]
    [SerializeField] private MeleeWeapon currentMeleeWeapon;

    private Transform playerVfxParent;
    private Transform staticPlayerVfxParent;

    private void Start()
    {
        playerVfxParent = GameObject.FindGameObjectWithTag(PLAYER_VFX_PARENT_TAG).transform;
        staticPlayerVfxParent = GameObject.FindGameObjectWithTag(STATIC_PLAYER_VFX_PARENT_TAG).transform;

        player = GetComponentInParent<Player>();
        playerBody = player.GetComponentInChildren<SpriteRenderer>().transform;
        inventory = GetComponentInParent<PlayerInventory>();

        inventory.OnCurrentWeaponChange += UpdateCurrentWeapon;
        player.OnPlayerLevelUp += PlayerLeveledUp;
        player.OnPlayerTakeDamage += PlayerBleed;
        player.OnPlayerDash += PlayerDash;
    }

    public void UpdateCurrentWeapon()
    {
        if (inventory.CurrentWeapon && inventory.CurrentWeapon.TryGetComponent<MeleeWeapon>(out MeleeWeapon weapon))
        {
            currentMeleeWeapon = weapon;
            weapon.WeaponIsUsed += PlayerAttackEffect;
        }
    }

    private void PlayerDash() => StartCoroutine(SpawnAndDelete(dashClip.length, dash, true, Vector3.zero));

    private void PlayerLeveledUp() => StartCoroutine(SpawnAndDelete(levelupClip.length, levelup, false, Vector3.zero));

    private void PlayerBleed() => StartCoroutine(SpawnAndDelete(bleedClip.length, bleed, false, Vector3.zero));

    private void PlayerAttackEffect() => StartCoroutine(
        SpawnAndDelete(currentMeleeWeapon.WeaponEffectClip.length, currentMeleeWeapon.WeaponEffect, true, currentMeleeWeapon.EffectPositionOffset)
    );

    public void PlayerDoubleAttack(Vector3 enemyPosition) => StartCoroutine(
        SpawnStaticAndDelete(doubleAttackClip.length, doubleAttack, enemyPosition)
    );

    public void PlayerHolyAttack(Vector3 enemyPosition) => StartCoroutine(
        SpawnStaticAndDelete(holyStrikeClip.length, holyStrike, enemyPosition)
    );

    private IEnumerator SpawnAndDelete(float time, GameObject obj, bool isStatic, Vector3 posOffset)
    {
        GameObject spawned = Instantiate(obj, transform.position, Quaternion.identity, isStatic ? staticPlayerVfxParent : playerVfxParent);
        
        Vector3 offset = playerBody.rotation.y > 0 ? new(-posOffset.x - .15f, posOffset.y, posOffset.z) : posOffset;
        spawned.transform.position += offset;
        spawned.transform.rotation = new(0, playerBody.rotation.y, 0, 0);
        yield return new WaitForSeconds(time);
        Destroy(spawned);
    }

    private IEnumerator SpawnStaticAndDelete(float time, GameObject obj, Vector3 pos)
    {
        GameObject spawned = Instantiate(obj, transform.position, Quaternion.identity, staticPlayerVfxParent);

        spawned.transform.position = pos;
        yield return new WaitForSeconds(time);
        Destroy(spawned);
    }
}
