using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const string ENEMIES_PARENT_TAG = "Enemies Parent";

    [SerializeField] private SpawnPoints spawnNodes;
    [SerializeField] private Transform enemiesParent;

    [Space]
    [SerializeField] private List<GameObject> earlyGameEnemies;
    [SerializeField] private List<GameObject> middleGameEnemies;
    [SerializeField] private List<GameObject> lateGameEnemies;

    [Space]
    [SerializeField] private List<GameObject> chestEnemies;

    [Space]
    [SerializeField] private List<GameObject> miniBosses;
    [SerializeField] private List<GameObject> bosses;

    [Space]
    [SerializeField] private bool canSpawnWave;
    [SerializeField] private int enemiesPerWave = 1;
    public float BaseEnemiesPerWaveMultiplier = 1f;
    public float ChunkEnemiesAmountMultiplier = 1f;

    [Space]
    [SerializeField] private float enemiesIncreasePerInterval = .05f;

    [Space]
    [SerializeField] private bool canSpawnChestEnemy;

    [Space]
    [SerializeField] private GameObject theEnemySpawnAnimationObject;

    public int EnemiesInWave { get {
            return Mathf.RoundToInt(enemiesPerWave * (BaseEnemiesPerWaveMultiplier + ChunkEnemiesAmountMultiplier));
    } }
    public bool CanSpawnWave { get { return canSpawnWave;} }
    public bool CanSpawnChestEnemy { get { return canSpawnChestEnemy; } }

    private void Start()
    {
        enemiesParent = GameObject.FindGameObjectWithTag(ENEMIES_PARENT_TAG).transform;

        spawnNodes = FindObjectOfType<SpawnPoints>();
    }

    public IEnumerator SpawnWave()
    {
        canSpawnWave = false;
        StartCoroutine(WaitAndEnableWaveSpawn(2));

        for (int i = 0; i < EnemiesInWave; i++)
        {
            int nodeIndex = Random.Range(0, spawnNodes.Nodes.Length);
            SpawnEnemy(nodeIndex);
            yield return new WaitForSeconds(GameplayManager.Instance.TimeBetweenWaves);
        }
    }

    public void SpawnTheEnemy()
    {
        foreach (Enemy enemy in FindObjectsOfType<Enemy>())
        {
            enemy.Die();
        }
        GameObject theEnemy = Instantiate(theEnemySpawnAnimationObject);
        TheEnemySpawn spawn = theEnemy.GetComponent<TheEnemySpawn>();
        theEnemy.transform.position = FindObjectOfType<Player>().transform.position + spawn.SpawnPos;
    }

    private void SpawnEnemy(int nodeIndex)
    {
        GameObject enemyToSpawn;
        Transform node = spawnNodes.Nodes[nodeIndex];
        GameStages currentStage = GameplayManager.Instance.CurrentGameStage;

        if (currentStage == GameStages.EARLY)
            enemyToSpawn = earlyGameEnemies[Random.Range(0, earlyGameEnemies.Count)];
        else if (currentStage == GameStages.MID)
            enemyToSpawn = middleGameEnemies[Random.Range(0, middleGameEnemies.Count)];
        else
            enemyToSpawn = lateGameEnemies[Random.Range(0, lateGameEnemies.Count)];

        if (Random.Range(0f, 1f) > enemyToSpawn.GetComponent<Enemy>().SpawnChance)
            return;

        GameObject enemy = Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);
        enemy.GetComponent<Enemy>().ScaleEnemyWithGameTime(GameplayManager.Instance.CurrentTime % GameplayManager.Instance.CurrentBreakpoint);
    }

    public void SpawnChestEnemy()
    {
        canSpawnChestEnemy = false;
        Transform node = spawnNodes.Nodes[Random.Range(0, spawnNodes.Nodes.Length)];
        GameObject enemyToSpawn = chestEnemies[Random.Range(0, chestEnemies.Count)];
        GameObject obj = Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);

        if (obj.TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.ScaleEnemyWithGameTime(GameplayManager.Instance.CurrentTime % GameplayManager.Instance.CurrentBreakpoint);
        }

        StartCoroutine(WaitAndEnableChestEnemySpawn(GameplayManager.Instance.TimeBetweenChestEnemySpawns));
    }

    public void SpawnMiniBoss()
    {
        Transform node = spawnNodes.Nodes[Random.Range(0, spawnNodes.Nodes.Length)];
        GameObject enemyToSpawn = miniBosses[Random.Range(0, miniBosses.Count)];
        GameObject obj = Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.ScaleEnemyWithGameTime(GameplayManager.Instance.CurrentTime % GameplayManager.Instance.CurrentBreakpoint);

        GameplayManager.Instance.MiniBossesCount++;
        enemy.OnEnemyDeath += () => GameplayManager.Instance.MiniBossesCount--;
    }

    public void SpawnBoss()
    {
        Transform node = spawnNodes.Nodes[Random.Range(0, spawnNodes.Nodes.Length)];
        GameObject enemyToSpawn = bosses[Random.Range(0, bosses.Count)];
        GameObject obj = Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);

        GameplayManager.Instance.BossesCount++;
        obj.GetComponent<Enemy>().OnEnemyDeath += () => GameplayManager.Instance.BossesCount--;
    }

    private IEnumerator WaitAndEnableWaveSpawn(float time)
    {
        yield return new WaitForSeconds(time);
        canSpawnWave = true;
    }

    private IEnumerator WaitAndEnableChestEnemySpawn(float time)
    {
        yield return new WaitForSeconds(time);
        canSpawnChestEnemy = true;
    }
}
