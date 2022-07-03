using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilSpawner : MonoBehaviour
{
    private const string PICKABLES_PARENT_TAG = "Pickables Parent";
    private const string INTERACTABLES_TAG = "Interactables Parent";

    [SerializeField] private SpawnPoints spawnNodes;

    [Space]
    [SerializeField] protected Transform interactablesParent;
    [SerializeField] protected Transform pickablesParent;

    [SerializeField] private GameObject[] chests;
    [SerializeField] private float chanceToSpawnChest = .5f;

    [SerializeField] private GameObject[] potions;
    [SerializeField] private float chanceToSpawnPotion = .8f;

    [SerializeField] private GameObject[] pouches;
    [SerializeField] private float chanceToSpawnPouch = .7f;

    [Space]
    [SerializeField] private bool canSpawnUtil;

    public bool CanSpawnUtil { get { return canSpawnUtil; } set { canSpawnUtil = value; } }

    private void Start()
    {
        ValidateChests();
        ValidatePotions();
        ValidatePouches();

        pickablesParent = GameObject.FindGameObjectWithTag(PICKABLES_PARENT_TAG).transform;
        interactablesParent = GameObject.FindGameObjectWithTag(INTERACTABLES_TAG).transform;

        spawnNodes = FindObjectOfType<SpawnPoints>();
    }

    public void SpawnUtil()
    {
        canSpawnUtil = false;

        int utilTypeToSpawn = Random.Range(0, 3);

        GameObject utilToSpawn;
        Transform utilParent;
        switch (utilTypeToSpawn)
        {
            case 0:
                if (Random.Range(0f, 1f) > chanceToSpawnChest)
                    return;

                utilToSpawn = chests[Random.Range(0, chests.Length)];
                utilParent = interactablesParent;
                break;
            case 1:
                if (Random.Range(0f, 1f) > chanceToSpawnPotion)
                    return;

                utilToSpawn = potions[Random.Range(0, potions.Length)];
                utilParent = pickablesParent;
                break;
            default:
                if (Random.Range(0f, 1f) > chanceToSpawnPouch)
                    return;

                utilToSpawn = pouches[Random.Range(0, pouches.Length)];
                utilParent = pickablesParent;
                break;
        }

        Transform node = spawnNodes.Nodes[Random.Range(0, spawnNodes.Nodes.Length)];
        Instantiate(utilToSpawn, node.position, Quaternion.identity, utilParent);
    }

    private void ValidateChests()
    {
        foreach (var chest in chests)
        {
            if (!chest.TryGetComponent(out Container _))
            {
                Debug.LogError($"{chest.name} is not a container!");
            }
        }
    }

    private void ValidatePotions()
    {
        foreach (var potion in potions)
        {
            if (!potion.TryGetComponent(out HealthPotion _))
            {
                Debug.LogError($"{potion.name} is not a health potion!");
            }
        }
    }

    private void ValidatePouches()
    {
        foreach (var pouch in pouches)
        {
            if (!pouch.TryGetComponent(out GoldPouch _))
            {
                Debug.LogError($"{pouch.name} is not a gold pouch!");
            }
        }
    }
}
