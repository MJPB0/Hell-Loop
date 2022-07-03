using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    private const string TRAPS_PARENT_TAG = "Traps Parent";

    [SerializeField] private SpawnPoints spawnNodes;

    [Space]
    [SerializeField] protected Transform trapsParent;

    [SerializeField] private GameObject[] traps;
    [SerializeField] private float chanceToSpawnTrap = .75f;

    [Space]
    [SerializeField] private bool canSpawnTrap;

    public bool CanSpawnTrap { get { return canSpawnTrap; } set { canSpawnTrap = value; } }

    private void Start()
    {
        trapsParent = GameObject.FindGameObjectWithTag(TRAPS_PARENT_TAG).transform;

        spawnNodes = FindObjectOfType<SpawnPoints>();
    }

    public void SpawnTrap()
    {
        canSpawnTrap = false;

        GameObject trapToSpawn = traps[Random.Range(0, traps.Length)];
        Transform trapParent = trapsParent;
        if (Random.Range(0f, 1f) > chanceToSpawnTrap)
            return;

        Transform node = spawnNodes.Nodes[Random.Range(0, spawnNodes.Nodes.Length)];
        Instantiate(trapToSpawn, node.position, Quaternion.identity, trapParent);
    }
}
