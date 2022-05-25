using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    private Player player;

    private Animator anim;

    [SerializeField] private SpriteRenderer body;

    [SerializeField] private TrapScriptableObject trapObject;

    private const string EXPLODE_TRIGGER = "Explode";

    private bool isPlayerInRange = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = true;
            StartCoroutine(WaitAndExplode());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private IEnumerator WaitAndExplode()
    {
        yield return new WaitForSeconds(trapObject.ActivationDelay);
        anim.SetTrigger(EXPLODE_TRIGGER);
    }

    public void TrapActivated()
    {
        body.sprite = null;
        if (isPlayerInRange) player.TakeDamage(trapObject.Damage);
    }

    public void TrapAnimationEnded()
    {
        Destroy(transform.parent.gameObject);
    }
}
