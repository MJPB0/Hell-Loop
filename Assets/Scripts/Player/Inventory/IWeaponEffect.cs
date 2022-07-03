using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponEffect
{
    public abstract void Use(Player player, Enemy enemy, WeaponName weaponName);
}
