using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackHelper : MonoBehaviour
{
    private Enemy attacker;

    private void Start()
    {
        attacker = GetComponentInParent<Enemy>();
    }

    public void DealDamage()
    {
        attacker.DealDamage(false);
    }

    public void DealSpecialAttackDamage()
    {
        attacker.DealDamage(true);
    }
}
