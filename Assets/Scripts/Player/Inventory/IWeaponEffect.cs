using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeaponEffect
{
    public abstract void Use(int level, Player player, Enemy enemy);
}
