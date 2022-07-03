using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPassive : MonoBehaviour
{
    protected SpriteRenderer spriteRenderer;

    [Header("Passive type")]
    [SerializeField] private PassiveName passiveName;
    [SerializeField] private string passiveDescription;

    [Space]
    [SerializeField] private StatType multiplierType;
    [SerializeField] private float baseValue;
    [SerializeField] private float additionalValue;

    [Header("Level")]
    [SerializeField] private int currentLevel = 0;
    [SerializeField] private int maxLevel;

    [Space]
    [SerializeField] private float[] levelUpgrades;

    [Header("Passive sprite")]
    [SerializeField] private Sprite sprite;

    public PassiveName PassiveName { get { return passiveName; } }
    public StatType MultiplierType { get { return multiplierType; } }
    public int CurrentLevel { get { return currentLevel; } set { currentLevel = value; } }
    public int MaxLevel { get { return maxLevel; } }
    public float Value { get { return baseValue + additionalValue; } }
    public float NextUpgradeValue { 
        get 
        {
            if (currentLevel == 0)
                return baseValue;
            return levelUpgrades[currentLevel - 1]; 
        } 
    }
    public string PassiveDescription { get { return passiveDescription; } }
    public Sprite PassiveSprite { get { return sprite; } }

    public void LevelUp()
    {
        if (currentLevel >= maxLevel) return;

        currentLevel++;
        ApplyUpgrades();
    }

    private void ApplyUpgrades()
    {
        additionalValue = 0f;
        for (int i = 0; i < currentLevel - 1; i++)
        {
            additionalValue += levelUpgrades[i];
        }
    }

    public void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
}
