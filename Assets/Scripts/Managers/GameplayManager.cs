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

    [Header("Enemy spawning")]
    [SerializeField] private float timeBetweenWaves;
    [SerializeField] private float timeBetweenChestEnemySpawns;

    [Space]
    [SerializeField] private float percentagePerIncrease = 1.5f;
    [SerializeField] private float timeBetweenEnemiesAmountIncrease = 30f;
    [SerializeField] private float timeToEnemiesAmountIncrease = 30f;

    [Space]
    [SerializeField] private bool canSpawnMiniBoss;
    [SerializeField] private float minTimeBetweenMiniBosses = 120f;
    [SerializeField] private float timeToNextMiniBoss = 0f;
    public int MiniBossesCount = 0;

    [Space]
    [SerializeField] private List<int> bossSpawnTimestamps;
    public int BossesCount = 0;

    [Header("Util spawning")]
    [SerializeField] private float timeToUtilSpawn = 15f;
    [SerializeField] private float timeBetweenUtilSpawns = 30f;

    [Header("Trap spawning")]
    [SerializeField] private float timeToTrapSpawn = 15f;
    [SerializeField] private float timeBetweenTrapSpawns = 30f;

    [Space]
    [SerializeField] private bool reachedEnd = false;

    [Header("Game stats")]
    public int KnifeDamage = 0;
    public int EvolvedKnifeDamage = 0;
    public int SwordDamage = 0;
    public int EvolvedSwordDamage = 0;
    public int TomahawkDamage = 0;
    public int EvolvedTomahawkDamage = 0;
    public int AxeDamage = 0;
    public int EvolvedAxeDamage = 0;
    public int IceWandDamage = 0;
    public int EvolvedIceWandDamage = 0;
    public int FireWandDamage = 0;
    public int EvolvedFireWandDamage = 0;
    public int EarthWandDamage = 0;
    public int EvolvedEarthWandDamage = 0;
    public int WindWandDamage = 0;
    public int EvolvedWindWandDamage = 0;

    public int OverallDamage = 0;
    public int DamageTaken = 0;
    public int HealingDone = 0;

    public int EnemiesKilled = 0;

    public int ExperienceGained = 0;

    public int LevelReached = 0;
    public float TimeAlive = 0;

    private Player player;

    private EnemySpawner enemySpawner;
    private UtilSpawner utilSpawner;
    private TrapSpawner trapSpawner;

    public GameStages CurrentGameStage { get { return currentGameStage; } }

    public float CurrentTime { get { return currentGameTime; } }

    public bool IsPaused { get { return isPaused; } }
    public bool HasEnded { get { return hasEnded; } }
    public float TimeBetweenWaves { get { return timeBetweenWaves; } }
    public float TimeBetweenChestEnemySpawns { get { return timeBetweenChestEnemySpawns;} }

    public float CurrentGameTime { get { return currentGameTime; } }

    public float CurrentBreakpoint { get { return gameStagesBreakpoints[(int)currentGameStage > 0 ? (int)currentGameStage - 1 : (int)currentGameStage]; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        player = FindObjectOfType<Player>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
        utilSpawner = FindObjectOfType<UtilSpawner>();
        trapSpawner = FindObjectOfType<TrapSpawner>();

        SoundManager.Initialize(player);
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!isPaused && !hasEnded && !reachedEnd)
            CalculateStageTime();
    }

    private void CalculateStageTime()
    {
        currentGameTime += Time.deltaTime;

        CalculateTimeToMiniBoss();
        CalculateTimeToEnemiesAmountIncrease();
        CalculateTimeToUtilSpawn();
        CalculateTimeToTrapSpawn();

        if (currentGameTime >= gameStagesBreakpoints[(int)currentGameStage])
            AdvanceGameStage();

        if (utilSpawner.CanSpawnUtil)
            utilSpawner.SpawnUtil();

        if (trapSpawner.CanSpawnTrap)
            trapSpawner.SpawnTrap();

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

    private void CalculateTimeToTrapSpawn()
    {
        if (timeToTrapSpawn > 0)
            timeToTrapSpawn -= Time.deltaTime;
        else
        {
            timeToTrapSpawn = timeBetweenTrapSpawns;
            trapSpawner.CanSpawnTrap = true;
        }
    }

    private void CalculateTimeToUtilSpawn()
    {
        if (timeToUtilSpawn > 0)
            timeToUtilSpawn -= Time.deltaTime;
        else
        {
            timeToUtilSpawn = timeBetweenUtilSpawns;
            utilSpawner.CanSpawnUtil = true;
        }
    }

    private void CalculateTimeToEnemiesAmountIncrease()
    {
        if (timeToEnemiesAmountIncrease > 0)
            timeToEnemiesAmountIncrease -= Time.deltaTime;
        else
        {
            timeToEnemiesAmountIncrease = timeBetweenEnemiesAmountIncrease;
            enemySpawner.BaseEnemiesPerWaveMultiplier += percentagePerIncrease;
        }
    }

    private void CalculateTimeToMiniBoss()
    {
        if (timeToNextMiniBoss > 0)
            timeToNextMiniBoss -= Time.deltaTime;
        else if (!canSpawnMiniBoss && MiniBossesCount == 0)
        {
            timeToNextMiniBoss = 0f;
            canSpawnMiniBoss = true;
        }
    }

    private void AdvanceGameStage()
    {
        enemySpawner.BaseEnemiesPerWaveMultiplier = 1f;
        switch (currentGameStage)
        {
            case GameStages.EARLY:
                currentGameStage++;
                break;
            case GameStages.MID:
                currentGameStage++;
                break;
            case GameStages.LATE:
                SpawnTheEnemy();
                break;
        }
    }

    private void SpawnTheEnemy()
    {
        player.IsStunned = true;
        player.CanPickupExperience = false;
        reachedEnd = true;

        StopAllCoroutines();

        enemySpawner.SpawnTheEnemy();
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

    public void CountDamageDealt(int value, WeaponName weaponType)
    {
        OverallDamage += value;
        switch (weaponType)
        {
            case WeaponName.AXE:
                AxeDamage += value;
                break;
            case WeaponName.KNIFE:
                KnifeDamage += value;
                break;
            case WeaponName.SWORD:
                SwordDamage += value;
                break;
            case WeaponName.FIRE_WAND:
                FireWandDamage += value;
                break;
            case WeaponName.EARTH_WAND:
                EarthWandDamage += value;
                break;
            case WeaponName.WIND_WAND:
                WindWandDamage += value;
                break;
            case WeaponName.ICE_WAND:
                IceWandDamage += value;
                break;
            case WeaponName.BATTLEAXE:
                EvolvedAxeDamage += value;
                break;
            case WeaponName.BLOODY_KNIFE:
                EvolvedKnifeDamage += value;
                break;
            case WeaponName.HOLY_SWORD:
                EvolvedSwordDamage += value;
                break;
            case WeaponName.LASER_WAND:
                EvolvedFireWandDamage += value;
                break;
            case WeaponName.COSMIC_WAND:
                EvolvedEarthWandDamage += value;
                break;
            case WeaponName.THUNDER_WAND:
                EvolvedWindWandDamage += value;
                break;
            case WeaponName.TIME_WAND:
                EvolvedIceWandDamage += value;
                break;
        }
    }

    public void CountDamageTaken(int value)
    {
        DamageTaken += value;
    } 

    public void CountExperienceGained(int value)
    {
        ExperienceGained += value;
    }

    public void AddEnemyKilled()
    {
        EnemiesKilled++;
    }

    public void CountHealingDone(int value)
    {
        HealingDone += value;
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

        LevelReached = player.CurrentLvl;
        if (currentGameTime >= 30)
            DamageTaken -= 9999;
        TimeAlive = Mathf.Round((currentGameTime / 60) * 100.0f) * 0.01f;

        OnGameEnd?.Invoke();
    }
}
