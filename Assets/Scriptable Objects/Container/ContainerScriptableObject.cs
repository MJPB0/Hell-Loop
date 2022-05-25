using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interactable container", menuName = "Interactable Container", order = 0)]
public class ContainerScriptableObject : ScriptableObject
{
    [SerializeField] private GameObject[] drops;

    public AnimatorOverrideController[] overrideControllers;

    public GameObject[] Drops { get { return drops; } }
}
