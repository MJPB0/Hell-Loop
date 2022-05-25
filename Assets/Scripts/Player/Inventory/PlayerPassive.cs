using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPassive : MonoBehaviour
{
    [Header("Passive type")]
    [SerializeField] private PassiveName passiveName;
    [SerializeField] private MultiplierType multiplierType;

    [Header("Level")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevel;

    [Space]
    [SerializeField] private float inceasePerLevel = 1f;

    public PassiveName PassiveName { get { return passiveName; } }
    public MultiplierType MultiplierType { get { return multiplierType; } }
    public int CurrentLevel { get { return currentLevel; } }
    public int MaxLevel { get { return maxLevel; } }
    public float InceasePerLevel { get { return inceasePerLevel; } }

    public void LevelUp()
    {
        if (currentLevel >= maxLevel) return;

        currentLevel++;
    }
}
