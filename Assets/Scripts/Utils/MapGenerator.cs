using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    private const string MAP_PARENT_TAG = "Map Parent";
    private const string CHUNK_TAG = "Map Chunk";

    private const int CHUNK_COLLIDER_X_OFFSET = -3;
    private const int CHUNK_COLLIDER_Y_OFFSET = 2;

    private const int LEFT_CHUNK_END = -14;
    private const int RIGHT_CHUNK_END = 8;
    private const int TOP_CHUNK_END = 22;
    private const int BOTTOM_CHUNK_END = -18;

    [SerializeField] private Transform mapParent;

    [Space]
    [SerializeField] private int chunkSize = 64;

    [Header("Ground")]
    [SerializeField] private GameObject[] grounds;

    [Header("Environment")]
    [SerializeField] private GameObject[] rockEnvironments;
    [SerializeField] private GameObject[] fireplaceEnvironments;
    [SerializeField] private GameObject[] flowerEnvironments;
    [SerializeField] private GameObject[] bonesEnvironments;
    [SerializeField] private GameObject[] grassEnvironments;
    [SerializeField] private GameObject[] tombstoneEnvironments;

    [Space]
    [SerializeField] private GameObject currentChunk;
    [SerializeField] private List<GameObject> chunks;

    private Transform player;
    private EnemySpawner enemySpawner;

    private void Start()
    {
        mapParent = GameObject.FindGameObjectWithTag(MAP_PARENT_TAG).transform;

        player = FindObjectOfType<Player>().transform;
        enemySpawner = FindObjectOfType<EnemySpawner>();

        chunks = new List<GameObject>();
        GameObject chunk = GameObject.FindGameObjectWithTag(CHUNK_TAG);
        chunks.Add(chunk);
        currentChunk = chunk;
    }

    private void Update()
    {
        DetectEndOfChunk();
    }

    private void DetectEndOfChunk()
    {
        Vector3 chunkPos = currentChunk.transform.position;

        bool isAtLeftEdge = player.position.x <= chunkPos.x + LEFT_CHUNK_END;
        bool isAtRightEdge = player.position.x >= chunkPos.x + RIGHT_CHUNK_END;
        bool isAtBottomEdge = player.position.y <= chunkPos.y + BOTTOM_CHUNK_END;
        bool isAtTopEdge = player.position.y >= chunkPos.y + TOP_CHUNK_END;
        if (!isAtBottomEdge && !isAtTopEdge && !isAtLeftEdge && !isAtRightEdge) return;

        Chunk chunk = currentChunk.GetComponent<Chunk>();
        if (chunk.IsSurrounded) return;

        if (isAtTopEdge && !chunk.HasTopChunk)
            SpawnChunk(1, 0);
        if (isAtBottomEdge && !chunk.HasBottomChunk)
            SpawnChunk(-1, 0);

        if (isAtRightEdge && !chunk.HasRightChunk)
            SpawnChunk(0, 1);
        if (isAtLeftEdge && !chunk.HasLeftChunk)
            SpawnChunk(0, -1);

        if (isAtLeftEdge && isAtTopEdge && !chunk.HasTopLeftChunk)
            SpawnChunk(1, -1);
        else if (isAtLeftEdge && isAtBottomEdge && !chunk.HasBottomLeftChunk)
            SpawnChunk(-1, -1);

        if (isAtRightEdge && isAtTopEdge && !chunk.HasTopRightChunk)
            SpawnChunk(1, 1);
        else if (isAtRightEdge && isAtBottomEdge && !chunk.HasBottomRightChunk)
            SpawnChunk(-1, 1);
    }

    private void DetectAdjacentChunks(Chunk newChunk)
    {
        Vector3 newChunkPos = newChunk.transform.position;

        foreach (GameObject mapChunk in chunks)
        {
            Chunk chunk = mapChunk.GetComponent<Chunk>();
            if (chunk.IsSurrounded) continue;

            Vector3 chunkPos = chunk.transform.position;
            if (chunkPos.x - chunkSize == newChunkPos.x && chunkPos.y == newChunkPos.y)
            {
                chunk.HasLeftChunk = true;
                newChunk.HasRightChunk = true;
            }
            if (chunkPos.x + chunkSize == newChunkPos.x && chunkPos.y == newChunkPos.y)
            {
                chunk.HasRightChunk = true;
                newChunk.HasLeftChunk = true;
            }

            if (chunkPos.y - chunkSize == newChunkPos.y && chunkPos.x == newChunkPos.x)
            {
                chunk.HasBottomChunk = true;
                newChunk.HasTopChunk = true;
            }
            if (chunkPos.y + chunkSize == newChunkPos.y && chunkPos.x == newChunkPos.x)
            {
                chunk.HasTopChunk = true;
                newChunk.HasBottomChunk = true;
            }

            if (chunkPos.x - chunkSize == newChunkPos.x &&
                chunkPos.y + chunkSize == newChunkPos.y)
            {
                chunk.HasTopLeftChunk = true;
                newChunk.HasBottomRightChunk = true;
            }
            if (chunkPos.x - chunkSize == newChunkPos.x &&
                chunkPos.y - chunkSize == newChunkPos.y)
            {
                chunk.HasBottomLeftChunk = true;
                newChunk.HasTopRightChunk = true;
            }

            if (chunkPos.x + chunkSize == newChunkPos.x &&
                chunkPos.y + chunkSize == newChunkPos.y)
            {
                chunk.HasTopRightChunk = true;
                newChunk.HasBottomLeftChunk = true;
            }
            if (chunkPos.x + chunkSize == newChunkPos.x &&
                chunkPos.y - chunkSize == newChunkPos.y)
            {
                chunk.HasBottomRightChunk = true;
                newChunk.HasTopLeftChunk = true;
            }
        }
    }

    public void ChangeCurrentChunk(string chunkName)
    {
        currentChunk = chunks.Find(c => c.name == chunkName);
        enemySpawner.ChunkEnemiesAmountMultiplier = currentChunk.GetComponent<Chunk>().EnemyMultiplier;
    }

    private void SpawnChunk(int vertical, int horizontal)
    {
        GameObject chunk = new();

        chunk.name = $"Chunk{chunks.Count}";
        chunk.tag = CHUNK_TAG;

        chunk.transform.parent = mapParent;
        chunk.transform.position = new(
            currentChunk.transform.position.x + chunkSize * horizontal,
            currentChunk.transform.position.y + chunkSize * vertical,
            0
        );

        BoxCollider2D chunkCollider = chunk.AddComponent<BoxCollider2D>();
        chunkCollider.size = new(chunkSize, chunkSize);
        chunkCollider.offset = new(CHUNK_COLLIDER_X_OFFSET, CHUNK_COLLIDER_Y_OFFSET);
        chunkCollider.isTrigger = true;

        Chunk newChunk = chunk.AddComponent<Chunk>();
        DetectAdjacentChunks(newChunk);

        GameObject groundToSpawn = grounds[Random.Range(0, grounds.Length)];

        List<GameObject> environments = new();
        if (Random.Range(0,2) == 1) environments.Add(rockEnvironments[Random.Range(0, rockEnvironments.Length)]);
        if (Random.Range(0,2) == 1)
        {
            newChunk.EnemyMultiplier -= .2f;
            environments.Add(fireplaceEnvironments[Random.Range(0, fireplaceEnvironments.Length)]);
        }
        if (Random.Range(0,2) == 1)
        {
            newChunk.EnemyMultiplier -= .1f;
            environments.Add(flowerEnvironments[Random.Range(0, flowerEnvironments.Length)]);
        }
        if (Random.Range(0,2) == 1)
        {
            newChunk.EnemyMultiplier += .15f;
            environments.Add(bonesEnvironments[Random.Range(0, bonesEnvironments.Length)]);
        }
        if (Random.Range(0,2) == 1)
        {
            newChunk.EnemyMultiplier -= .1f;
            environments.Add(grassEnvironments[Random.Range(0, grassEnvironments.Length)]);
        }
        if (Random.Range(0,2) == 1)
        {
            newChunk.EnemyMultiplier += .25f;
            environments.Add(tombstoneEnvironments[Random.Range(0, tombstoneEnvironments.Length)]);
        }

        Instantiate(groundToSpawn, chunk.transform.position, Quaternion.identity, chunk.transform);
        foreach (GameObject env in environments)
            Instantiate(env, chunk.transform.position, Quaternion.identity, chunk.transform);

        chunks.Add(chunk);
    }
}
