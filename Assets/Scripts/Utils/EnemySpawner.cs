using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const string ENEMIES_PARENT_TAG = "Enemies Parent";

    [SerializeField] private List<Transform> nodes;
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
    [SerializeField] private int enemiesPerWave;
    [SerializeField] private float enemiesPerWaveMultiplier;

    [Space]
    [SerializeField] private bool canSpawnChestEnemy;

    private Camera mainCamera;

    public int EnemiesInWave { get { return Mathf.RoundToInt(enemiesPerWave * enemiesPerWaveMultiplier);} }
    public bool CanSpawnWave { get { return canSpawnWave;} }
    public bool CanSpawnChestEnemy { get { return canSpawnChestEnemy; } }

    private void Start()
    {
        mainCamera = FindObjectOfType<Camera>();
        enemiesParent = GameObject.FindGameObjectWithTag(ENEMIES_PARENT_TAG).transform;
    }

    private void Update()
    {
        transform.position = new(mainCamera.transform.position.x, mainCamera.transform.position.y, 0);
    }

    public IEnumerator SpawnWave()
    {
        canSpawnWave = false;
        StartCoroutine(WaitAndEnableWaveSpawn(2));

        for (int i = 0; i < EnemiesInWave; i++)
        {
            int nodeIndex = Random.Range(0, nodes.Count);
            SpawnEnemy(nodeIndex);
            yield return new WaitForSeconds(GameplayManager.Instance.TimeBetweenWaves);
        }
    }

    private void SpawnEnemy(int nodeIndex)
    {
        GameObject enemyToSpawn;
        Transform node = nodes[nodeIndex];
        GameStages currentStage = GameplayManager.Instance.CurrentGameStage;

        if (currentStage == GameStages.EARLY)
            enemyToSpawn = earlyGameEnemies[Random.Range(0, earlyGameEnemies.Count)];
        else if (currentStage == GameStages.MID)
            enemyToSpawn = middleGameEnemies[Random.Range(0, middleGameEnemies.Count)];
        else
            enemyToSpawn = lateGameEnemies[Random.Range(0, lateGameEnemies.Count)];

        if (Random.Range(0f, 1f) > enemyToSpawn.GetComponent<Enemy>().SpawnChance)
            return;

        Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);
    }

    public void SpawnChestEnemy()
    {
        canSpawnChestEnemy = false;
        Transform node = nodes[Random.Range(0, nodes.Count)];
        GameObject enemyToSpawn = chestEnemies[Random.Range(0, chestEnemies.Count)];
        Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);

        StartCoroutine(WaitAndEnableChestEnemySpawn(GameplayManager.Instance.TimeBetweenChestEnemySpawns));
    }

    public void SpawnMiniBoss()
    {
        Transform node = nodes[Random.Range(0, nodes.Count)];
        GameObject enemyToSpawn = miniBosses[Random.Range(0, miniBosses.Count)];
        GameObject obj = Instantiate(enemyToSpawn, node.position, Quaternion.identity, enemiesParent);

        GameplayManager.Instance.MiniBossesCount++;
        obj.GetComponent<Enemy>().OnEnemyDeath += () => GameplayManager.Instance.MiniBossesCount--;
    }

    public void SpawnBoss()
    {
        Transform node = nodes[Random.Range(0, nodes.Count)];
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
