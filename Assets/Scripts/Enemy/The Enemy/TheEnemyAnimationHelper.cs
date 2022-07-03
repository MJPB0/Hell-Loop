using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnemyAnimationHelper : MonoBehaviour
{
    protected const string ENEMY_ATTACK_TRIGGER = "Attack";

    [SerializeField] private float timeToAttack = 600f;

    [Space]
    [SerializeField] private TheEnemy theEnemy;

    private Animator animator;

    private void Start()
    {
        theEnemy = GetComponentInParent<TheEnemy>();
        animator = GetComponent<Animator>();

        StartCoroutine(WaitAndKillPlayer(timeToAttack));
    }

    public IEnumerator WaitAndKillPlayer(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        animator.SetTrigger(ENEMY_ATTACK_TRIGGER);
    }

    public void Attack()
    {
        theEnemy.KillPlayer();
    }
}
