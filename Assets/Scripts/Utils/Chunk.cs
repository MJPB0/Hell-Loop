using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public bool HasLeftChunk = false;
    public bool HasRightChunk = false;

    public bool HasTopChunk = false;
    public bool HasBottomChunk = false;

    public bool HasBottomRightChunk = false;
    public bool HasBottomLeftChunk = false;

    public bool HasTopLeftChunk = false;
    public bool HasTopRightChunk = false;

    public float EnemyMultiplier = 1f;

    public bool IsSurrounded() => HasLeftChunk && HasRightChunk && HasTopChunk && HasBottomChunk &&
            HasBottomRightChunk && HasBottomLeftChunk && HasTopLeftChunk && HasTopRightChunk;
}
