using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Level Upgrade", menuName = "Weapon Level Upgrade", order = 0)]
public class WeaponLevelUpgradeSO : ScriptableObject
{
    [SerializeField] private WeaponStatType statType;
    [SerializeField] private float value;

    [SerializeField] private WeaponEffect primaryEffectUpgrade;
    [SerializeField] private WeaponEffect secondaryEffectUpgrade;

    public WeaponStatType StatType { get { return statType; } }
    public float Value { get { return value; } }
    public WeaponEffect PrimaryEffectUpgrade { get { return primaryEffectUpgrade; } }
    public WeaponEffect SecondaryEffectUpgrade { get { return secondaryEffectUpgrade; } }
}
