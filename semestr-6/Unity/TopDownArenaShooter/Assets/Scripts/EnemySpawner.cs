using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    public List<EnemyType> enemyTypes;
    public float spawnInterval = 2f;
    public float spawnRadius = 10f;
    public int maxEnemies = 20;
    public float difficultyIncreaseInterval = 30f;
    public float difficultyMultiplier = 1.05f; // Уменьшаем множитель
    public float maxDifficultyMultiplier = 2f; // Максимальный множитель сложности

    private Transform player;
    private float gameTime = 0f;
    private float nextDifficultyIncrease = 0f;
    private float currentDifficultyMultiplier = 1f;

    void Start()
    {
        ResetDifficulty();
        player = GameObject.FindGameObjectWithTag("Player").transform;
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

        // Находим позицию для спавна
        Vector3 spawnPos = player.position + Random.onUnitSphere * spawnRadius;
        spawnPos.y = 0f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, 2f, NavMesh.AllAreas))
        {
            GameObject enemy = Instantiate(selectedType.enemyPrefab, hit.position, Quaternion.identity);
            
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
    }

    // Вызывается при перезапуске игры
    void OnEnable()
    {
        ResetDifficulty();
    }
}
