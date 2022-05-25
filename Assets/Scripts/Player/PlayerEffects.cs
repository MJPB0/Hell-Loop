using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    private const string PLAYER_VFX_PARENT_TAG = "Player Effects";
    private const string STATIC_PLAYER_VFX_PARENT_TAG = "Player Static Effects";

    private Player player;

    [SerializeField] private GameObject levelup;
    [SerializeField] private GameObject bleed;
    [SerializeField] private GameObject dash;

    [Space]
    [SerializeField] private AnimationClip levelupClip;
    [SerializeField] private AnimationClip bleedClip;
    [SerializeField] private AnimationClip dashClip;

    private Transform playerVfxParent;
    private Transform staticPlayerVfxParent;

    private void Start()
    {
        playerVfxParent = GameObject.FindGameObjectWithTag(PLAYER_VFX_PARENT_TAG).transform;
        staticPlayerVfxParent = GameObject.FindGameObjectWithTag(STATIC_PLAYER_VFX_PARENT_TAG).transform;

        player = GetComponentInParent<Player>();
        player.OnPlayerLevelUp += PlayerLeveledUp;
        player.OnPlayerTakeDamage += PlayerBleed;
        player.OnPlayerDash += PlayerDash;
    }

    private void PlayerDash() => StartCoroutine(WaitAndDelete(dashClip.length, dash, true));

    private void PlayerLeveledUp() => StartCoroutine(WaitAndDelete(levelupClip.length, levelup, false));

    private void PlayerBleed() => StartCoroutine(WaitAndDelete(bleedClip.length, bleed, false));

    private IEnumerator WaitAndDelete(float time, GameObject obj, bool isStatic)
    {
        GameObject spawned = Instantiate(obj, transform.position, Quaternion.identity, isStatic ? staticPlayerVfxParent : playerVfxParent);
        yield return new WaitForSeconds(time);
        Destroy(spawned);
    }
}
