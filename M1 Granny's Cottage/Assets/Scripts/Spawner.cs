using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    // Using object pooling for enemies instead of destroying and re-instantiating
    // Enemies come in waves randomly from set spawnpoints (for now)

    public static Spawner Instance;
    [SerializeField] private Transform[] spawnPoints;
    public int numEnemiesPerWave = 10;
    public float spawnInterval = 2f;
    public float spawnTimer;
    public int enemiesSpawned = 0;
    public int aliveEnemies = 0;

    public bool waveActive = false;
    public float timeBetweenWaves = 20f;
    private float waveTimer;
    private bool waitingForNextWave = false;
    private bool waveFinishedSpawning = false;

    [SerializeField] private Enemy enemyPrefab;
    private IObjectPool<Enemy> enemyPool;

    private void Awake()
    {
        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGet, OnRelease);

        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    void Start()
    {
        Invoke("StartWave", 2);
    }

    private void StartWave()
    {
        waveActive = true;
        waitingForNextWave = false;
        waveFinishedSpawning = false;
        spawnTimer = spawnInterval;
        enemiesSpawned = 0;
        numEnemiesPerWave += 2;
    }

    // Spawns the enemies (gets them from pool)
    private void OnGet(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemy.transform.position = randomSpawnPoint.position;
        enemy.player = GameObject.FindGameObjectWithTag("Player").transform;
        enemy.ResetEnemy();
    }

    // Inactive when "dead"
    private void OnRelease(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    // Instanciates enemies in pool
    private Enemy CreateEnemy()
    {
        Enemy enemy = Instantiate(enemyPrefab);
        enemy.SetPool(enemyPool);

        return enemy;
    }

    // Handles incrementing/decrementing variables based on current wave state
    void Update()
    {
        if (waveActive)
        {
            spawnTimer -= Time.deltaTime;

            if (spawnTimer <= 0f && enemiesSpawned < numEnemiesPerWave) 
            {
                enemyPool.Get();
                enemiesSpawned++;
                aliveEnemies++;
                spawnTimer = spawnInterval;
            }

            if (enemiesSpawned >= numEnemiesPerWave && aliveEnemies <= 0)
            {
                waveActive = false;
                waveFinishedSpawning = true;
            }
        }

        if (waveFinishedSpawning)
        {
            if (aliveEnemies <= 0)
            {
                waitingForNextWave = true;
                waveFinishedSpawning = false;
                waveTimer = timeBetweenWaves;
            }
        }

        if (waitingForNextWave)
        {
            waveTimer -= Time.deltaTime;

            if (waveTimer <= 0f)
            {
                StartWave();
            }
        }
    }


}
