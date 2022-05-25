using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public UnityAction OnGameStart;
    public UnityAction OnGamePause;
    public UnityAction OnGameResume;
    public UnityAction OnGameEnd;
    public UnityAction OnGameWin;

    [SerializeField] private float currentGameTime = 0f;
    [SerializeField] private GameStages currentGameStage;
    [SerializeField] private List<float> gameStagesBreakpoints;

    [Space]
    [SerializeField] private bool isPaused = false;
    [SerializeField] private bool hasEnded = false;

    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject enemySpawnerObject;

    [Space]
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float timeBetweenChestEnemySpawns;

    [Space]
    [SerializeField] private bool canSpawnMiniBoss;
    [SerializeField] private float minTimeBetweenMiniBosses = 120f;
    [SerializeField] private float timeToNextMiniBoss = 0f;
    public int MiniBossesCount = 0;

    [Space]
    [SerializeField] private List<int> bossSpawnTimestamps;
    public int BossesCount = 0;

    private Player player;

    private EnemySpawner enemySpawner;

    public GameStages CurrentGameStage { get { return currentGameStage; } }

    public bool IsPaused { get { return isPaused; } }
    public bool HasEnded { get { return hasEnded; } }
    public float TimeBetweenWaves { get { return timeBetweenWaves; } }
    public float TimeBetweenChestEnemySpawns { get { return timeBetweenChestEnemySpawns;} }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isPaused && !hasEnded)
            CalculateStageTime();
    }

    private void CalculateStageTime()
    {
        currentGameTime += Time.deltaTime;

        if (timeToNextMiniBoss > 0) 
            timeToNextMiniBoss -= Time.deltaTime;
        else if (!canSpawnMiniBoss && MiniBossesCount == 0)
        {
            timeToNextMiniBoss = 0f;
            canSpawnMiniBoss = true;
        }

        if (currentGameTime >= gameStagesBreakpoints[(int)currentGameStage])
            AdvanceGameStage();

        int currentGameTimeInSeconds = Mathf.RoundToInt(currentGameTime);

        if (currentGameTimeInSeconds % timeBetweenWaves == 0 && enemySpawner.CanSpawnWave)
            StartCoroutine(enemySpawner.SpawnWave());

        if (currentGameTimeInSeconds % timeBetweenChestEnemySpawns == 0 && enemySpawner.CanSpawnChestEnemy)
            enemySpawner.SpawnChestEnemy();

        if (bossSpawnTimestamps.Contains(Mathf.RoundToInt(currentGameTimeInSeconds)) && BossesCount == 0)
            enemySpawner.SpawnBoss();

        if (player.IsAbleToEvolveWeapon && canSpawnMiniBoss)
        {
            enemySpawner.SpawnMiniBoss();
            timeToNextMiniBoss = minTimeBetweenMiniBosses;
            canSpawnMiniBoss = false;
        }
    }

    private void AdvanceGameStage()
    {
        switch (currentGameStage)
        {
            case GameStages.EARLY:
                currentGameStage++;
                break;
            case GameStages.MID:
                currentGameStage++;
                break;
            case GameStages.LATE:
                WinGame();
                break;
        }
    }

    private void SpawnPlayer()
    {
        player = FindObjectOfType<Player>();
        Player playerToSpawn = playerObject.GetComponent<Player>();

        if (player != null && player != playerToSpawn) return;
        else
        {
            Instantiate(playerObject);
            player = playerToSpawn;
        }
    }

    private void SpawnEnemySpawner()
    {
        enemySpawner = FindObjectOfType<EnemySpawner>();
        EnemySpawner enemySpawnerToSpawn = enemySpawnerObject.GetComponent<EnemySpawner>();

        if (player != null && player != enemySpawnerToSpawn) return;
        else
        {
            Instantiate(enemySpawnerObject);
            enemySpawner = enemySpawnerToSpawn;
        }
    }

    public void StartGame()
    {
        SpawnPlayer();
        SpawnEnemySpawner();

        currentGameStage = GameStages.EARLY;

        Time.timeScale = 1f;

        isPaused = false;
        hasEnded = false;
        player.CanTakeDamage = true;
        player.CanMove = true;
        player.CanDash = true;

        OnGameStart?.Invoke();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        OnGamePause?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        OnGameResume?.Invoke();
    }

    public void EndGame()
    {
        Time.timeScale = .2f;
        hasEnded = true;
        player.IsAlive = false;
        OnGameEnd?.Invoke();
    }

    private void WinGame()
    {
        Time.timeScale = 0f;
        hasEnded = true;
        OnGameWin?.Invoke();
    }
}
