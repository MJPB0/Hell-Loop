using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dark Explosion", menuName = "Trap", order = 0)]
public class TrapScriptableObject : ScriptableObject
{
    [SerializeField] private int damage;
    [SerializeField] private float activationDelay;

    public Sprite trapSprite;

    public int Damage { get { return damage; } }
    public float ActivationDelay { get { return activationDelay; } }
}
