using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnemy : MonoBehaviour
{
    [SerializeField] private GameObject onHitAnimation;
    [SerializeField] private AnimationClip onHitAnimationClip;

    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void KillPlayer()
    {
        GameObject animation = Instantiate(onHitAnimation);
        animation.transform.position = player.transform.position;
        StartCoroutine(WaitAndDestroy(animation, onHitAnimationClip.length));

        player.TakeDamage(9999);
    }

    private IEnumerator WaitAndDestroy(GameObject obj, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Destroy(obj);
    }
}
