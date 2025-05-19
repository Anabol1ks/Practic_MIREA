using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using System.Collections; // Добавим namespace для Coroutines

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyType> enemyTypes;
    public float spawnRadius = 10f; // Используется как запасной вариант, если нет зон спавна
    public int maxEnemies = 20;
    
    [Header("Зоны спавна")]
    public bool useSpawnZones = true; // Использовать ли зоны спавна вместо случайного спавна
    public float minDistanceFromPlayer = 8f; // Минимальное расстояние до игрока при спавне

    [Header("Настройки волн")]
    public int startWaveEnemyCount = 5; // Количество врагов в первой волне
    public float enemyCountMultiplierPerWave = 1.2f; // Множитель количества врагов на каждую следующую волну
    public float timeBetweenWaves = 10f; // Время между волнами
    public float timeBetweenEnemySpawnsInWave = 0.5f; // Время между спавном отдельных врагов в волне
    public float waveDifficultyMultiplier = 1.1f; // Множитель сложности (здоровье, урон) на каждую следующую волну

    private Transform player;
    private Transform playerBase;
    private float currentDifficultyMultiplier = 1f;
    private SpawnZoneManager zoneManager;

    private int currentWave = 0;
    private int enemiesRemainingInWave = 0;
    private bool isSpawningWave = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Находим базу игрока
        GameObject baseObj = GameObject.FindGameObjectWithTag("PlayerBase");
        if (baseObj != null)
        {
            playerBase = baseObj.transform;
        }
        
        // Находим менеджер зон спавна
        zoneManager = FindObjectOfType<SpawnZoneManager>();
        if (useSpawnZones && zoneManager == null)
        {
            Debug.LogWarning("SpawnZoneManager не найден на сцене. Используем обычный метод спавна.");
            useSpawnZones = false;
        }
        
        StartCoroutine(WaveSpawner()); // Запускаем корутину управления волнами
    }

    void IncreaseDifficulty()
    {
        currentDifficultyMultiplier *= waveDifficultyMultiplier;
        Debug.Log($"Difficulty increased for wave {currentWave}! Multiplier: {currentDifficultyMultiplier}");
    }

    IEnumerator WaveSpawner()
    {
        // Ждем несколько секунд перед первой волной
        yield return new WaitForSeconds(5f);

        while (true) // Бесконечный цикл для спавна волн
        {
            currentWave++;
            Debug.Log($"Starting Wave {currentWave}");

            // Увеличиваем сложность для новой волны
            IncreaseDifficulty();

            // Рассчитываем количество врагов для текущей волны
            int enemiesToSpawnThisWave = Mathf.RoundToInt(startWaveEnemyCount * Mathf.Pow(enemyCountMultiplierPerWave, currentWave - 1));
            enemiesRemainingInWave = enemiesToSpawnThisWave; // Инициализируем счетчик оставшихся врагов в волне
            
            isSpawningWave = true;
            // Спавним врагов для текущей волны
            for (int i = 0; i < enemiesToSpawnThisWave; i++)
            {
                // Проверяем, не превышено ли максимальное количество врагов на сцене
                int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
                if (currentEnemies >= maxEnemies)
                {
                    Debug.LogWarning($"Max enemies reached ({maxEnemies}). Waiting for enemies to be defeated before spawning more.");
                    // Ждем, пока количество врагов не уменьшится
                    yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies);
                }
                
                SpawnEnemy();
                yield return new WaitForSeconds(timeBetweenEnemySpawnsInWave); // Пауза между спавном отдельных врагов
            }
            isSpawningWave = false;

            Debug.Log($"Finished spawning wave {currentWave}. Waiting for all enemies to be defeated.");
            
            // Ждем, пока все враги текущей волны не будут уничтожены
            // TODO: Возможно, нужно будет отслеживать врагов, принадлежащих только к текущей волне
            // Пока что ждем, пока на сцене не останется врагов с тегом "Enemy"
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            Debug.Log($"All enemies in wave {currentWave} defeated. Waiting for next wave.");
            // Пауза между волнами
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    // Получаем позицию для спавна
    Vector3 GetSpawnPosition()
    {
        // Если используем зоны спавна и есть менеджер зон
        if (useSpawnZones && zoneManager != null)
        {
            // Получаем список активных зон
            List<SpawnZone> activeZones = zoneManager.GetActiveZones();
            
            // Если активных зон нет, используем запасной метод
            if (activeZones.Count == 0)
                return GetBackupSpawnPosition();
                
            // Выбираем случайную зону
            SpawnZone zone = activeZones[Random.Range(0, activeZones.Count)];
            
            // Получаем случайную позицию внутри зоны
            Vector3 position = zone.GetRandomPosition();
            
            // Проверяем, находится ли точка на NavMesh
            NavMeshHit hit;
            if (NavMesh.SamplePosition(position, out hit, 5f, NavMesh.AllAreas))
            {
                // Проверяем расстояние до игрока
                if (player != null && Vector3.Distance(hit.position, player.position) < minDistanceFromPlayer)
                {
                    // Если слишком близко к игроку, пробуем другую точку
                    return GetSpawnPosition();
                }
                
                return hit.position;
            }
            
            // Если не удалось найти точку на NavMesh, используем запасной метод
            return GetBackupSpawnPosition();
        }
        else
        {
            // Если не используем зоны, используем запасной метод
            return GetBackupSpawnPosition();
        }
    }

    // Запасной метод получения позиции для спавна (старый метод)
    Vector3 GetBackupSpawnPosition()
    {
        // Находим позицию для спавна
        Vector3 spawnPos = player.position + Random.onUnitSphere * spawnRadius;
        spawnPos.y = 0f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        // Если вообще ничего не получилось, возвращаем позицию игрока
        // Это плохой вариант, но лучше, чем ошибка
        return player.position;
    }

    void SpawnEnemy()
    {
        // Проверяем количество врагов на сцене - эта проверка теперь дублируется в корутине, но оставим ее для надежности
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemies >= maxEnemies) return; // Если врагов слишком много, не спавним

        // Выбираем доступные типы врагов на основе времени игры - теперь на основе волны или других критериев, но пока оставим так
        var availableTypes = enemyTypes.Where(t => t.unlockTime <= Time.time).ToList(); // Используем Time.time вместо gameTime
        if (availableTypes.Count == 0) return;

        // Выбираем тип врага на основе весов
        float totalWeight = availableTypes.Sum(t => t.spawnWeight);
        float random = Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        EnemyType selectedType = null;

        foreach (var type in availableTypes)
        {
            currentWeight += type.spawnWeight;
            if (random <= currentWeight)
            {
                selectedType = type;
                break;
            }
        }

        if (selectedType == null) return;

        // Получаем позицию для спавна
        Vector3 spawnPosition = GetSpawnPosition();

        // Создаем врага
        GameObject enemy = Instantiate(selectedType.enemyPrefab, spawnPosition, Quaternion.identity);
        
        // Применяем множитель сложности к параметрам врага
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Создаем копию параметров врага, чтобы не менять оригинальные значения
            EnemyType modifiedType = ScriptableObject.CreateInstance<EnemyType>();
            modifiedType.maxHealth = selectedType.maxHealth * currentDifficultyMultiplier;
            modifiedType.damage = selectedType.damage * currentDifficultyMultiplier;
            modifiedType.moveSpeed = selectedType.moveSpeed; // Скорость не меняем от волны, только от типа врага
            modifiedType.attackRange = selectedType.attackRange;
            modifiedType.isRanged = selectedType.isRanged;
            modifiedType.minScore = selectedType.minScore;
            modifiedType.maxScore = selectedType.maxScore;
            // Копируем другие параметры, если есть
            // modifiedType.enemyPrefab = selectedType.enemyPrefab; // Не нужно копировать префаб
            // modifiedType.unlockTime = selectedType.unlockTime; // Не нужно копировать unlockTime
            modifiedType.spawnWeight = selectedType.spawnWeight; // Копируем spawnWeight

            enemyHealth.enemyType = modifiedType; // Присваиваем модифицированный тип врага
        }
        
        // Добавляем компонент EnemyVisibility, если его нет
        if (enemy.GetComponent<EnemyVisibility>() == null)
        {
            enemy.AddComponent<EnemyVisibility>();
        }
        
        // TODO: Возможно, уменьшать enemiesRemainingInWave при уничтожении врага
    }
}
