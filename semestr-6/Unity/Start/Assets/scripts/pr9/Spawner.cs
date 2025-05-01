using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Настройки спауна")]
    public GameObject cubePrefab;
    [Tooltip("Ссылка на PlayerControllerPR9 в сцене")]
    public PlayerControllerPR9 playerController;

    public float spawnIntervalMin = 1f;
    public float spawnIntervalMax = 3f;
    public float spawnRadius = 3f;
    public float objectLifetime = 5f;

    private float nextSpawnTime;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            SpawnCube();
            ScheduleNextSpawn();
        }
    }

    void ScheduleNextSpawn()
    {
        // По умолчанию берём базовые интервалы
        float min = spawnIntervalMin;
        float max = spawnIntervalMax;

        // Если здоровье игрока упало до 50 или ниже — спаун ускоряется вдвое
        if (playerController.health <= 50)
        {
            min *= 0.5f;
            max *= 0.5f;
        }

        nextSpawnTime = Time.time + Random.Range(min, max);
    }

    void SpawnCube()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = 0.5f; // фиксируем по высоте

        GameObject cube = Instantiate(cubePrefab, randomPos, Quaternion.identity);
        // Авто-уничтожение
        Destroy(cube, objectLifetime);

        // Дополнительно: если у игрока высокая сила, спавним ещё один куб
        if (playerController.strength >= 30)
        {
            Vector3 offset = Random.onUnitSphere * (spawnRadius / 2f);
            offset.y = 0.5f;
            Instantiate(cubePrefab, transform.position + offset, Quaternion.identity);
        }
    }
}
