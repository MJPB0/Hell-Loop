using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pickable object", menuName = "Pickable object", order = 0)]
public class PickableScriptableObject : ScriptableObject
{
    [SerializeField] private int value;

    public Sprite sprite;

    public int Value { get { return value; } }
}
