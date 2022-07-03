using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheEnemySpawn : MonoBehaviour
{
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private GameObject theEnemy;

    public Vector3 SpawnPos { get { return spawnPosition; } }

    public void SpawnTheEnemy()
    {
        GameObject enemy = Instantiate(theEnemy);
        enemy.transform.position = transform.position;
    }

    public void DestroySpawnAnimationObject()
    {
        Destroy(gameObject);
    }
}
