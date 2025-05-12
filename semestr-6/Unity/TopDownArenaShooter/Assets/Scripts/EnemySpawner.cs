using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyType> enemyTypes;
    public float spawnInterval = 2f;
    public float spawnRadius = 10f; // Используется как запасной вариант, если нет зон спавна
    public int maxEnemies = 20;
    public float difficultyIncreaseInterval = 30f;
    public float difficultyMultiplier = 1.05f;
    public float maxDifficultyMultiplier = 2f;
    
    [Header("Зоны спавна")]
    public bool useSpawnZones = true; // Использовать ли зоны спавна вместо случайного спавна
    public float minDistanceFromPlayer = 8f; // Минимальное расстояние до игрока при спавне

    private Transform player;
    private Transform playerBase;
    private float gameTime = 0f;
    private float nextDifficultyIncrease = 0f;
    private float currentDifficultyMultiplier = 1f;
    private SpawnZoneManager zoneManager;

    [System.Obsolete]
    void Start()
    {
        ResetDifficulty();
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
        
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
    }

    void ResetDifficulty()
    {
        gameTime = 0f;
        currentDifficultyMultiplier = 1f;
        nextDifficultyIncrease = difficultyIncreaseInterval;
    }

    void Update()
    {
        gameTime += Time.deltaTime;
        
        // Увеличиваем сложность каждые difficultyIncreaseInterval секунд
        if (gameTime >= nextDifficultyIncrease)
        {
            IncreaseDifficulty();
            nextDifficultyIncrease = gameTime + difficultyIncreaseInterval;
        }
    }

    void IncreaseDifficulty()
    {
        // Проверяем, не превышен ли максимальный множитель
        if (currentDifficultyMultiplier < maxDifficultyMultiplier)
        {
            currentDifficultyMultiplier *= difficultyMultiplier;
            // Ограничиваем множитель максимальным значением
            currentDifficultyMultiplier = Mathf.Min(currentDifficultyMultiplier, maxDifficultyMultiplier);
            Debug.Log($"Difficulty increased! Multiplier: {currentDifficultyMultiplier}");
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
        // Проверяем количество врагов на сцене
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (currentEnemies >= maxEnemies) return;

        // Выбираем доступные типы врагов на основе времени игры
        var availableTypes = enemyTypes.Where(t => t.unlockTime <= gameTime).ToList();
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
            enemyHealth.enemyType = selectedType;
            // Создаем копию параметров врага, чтобы не менять оригинальные значения
            EnemyType modifiedType = ScriptableObject.CreateInstance<EnemyType>();
            modifiedType.maxHealth = selectedType.maxHealth * currentDifficultyMultiplier;
            modifiedType.damage = selectedType.damage * currentDifficultyMultiplier;
            modifiedType.moveSpeed = selectedType.moveSpeed;
            modifiedType.attackRange = selectedType.attackRange;
            modifiedType.isRanged = selectedType.isRanged;
            modifiedType.minScore = selectedType.minScore;
            modifiedType.maxScore = selectedType.maxScore;
            enemyHealth.enemyType = modifiedType;
        }
        
        // Добавляем компонент EnemyVisibility, если его нет
        if (enemy.GetComponent<EnemyVisibility>() == null)
        {
            enemy.AddComponent<EnemyVisibility>();
        }
    }

    // Вызывается при перезапуске игры
    void OnEnable()
    {
        ResetDifficulty();
    }
}
