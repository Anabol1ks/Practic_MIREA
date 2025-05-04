using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
        public float spawnInterval = 0.5f;
        public GameObject[] enemyPrefabs;
    }

    [Header("Wave Settings")]
    public Wave[] waves;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 10f;
    public float waveStartDelay = 3f;

    [Header("Difficulty")]
    public float difficultyMultiplier = 1.1f;
    public float maxSpawnRate = 0.2f;

    private int currentWaveIndex = 0;
    private bool isSpawning = false;
    private int enemiesRemaining;

    void Start()
    {
        StartCoroutine(WaveCountdown());
    }

    IEnumerator WaveCountdown()
    {
        yield return new WaitForSeconds(waveStartDelay);

        while (currentWaveIndex < waves.Length)
        {
            StartCoroutine(SpawnWave(waves[currentWaveIndex]));
            yield return new WaitUntil(() => enemiesRemaining <= 0);
            yield return new WaitForSeconds(timeBetweenWaves);
            currentWaveIndex++;

            // Увеличиваем сложность
            timeBetweenWaves = Mathf.Max(5f, timeBetweenWaves * 0.9f);
        }

        // Все волны пройдены
        Debug.Log("All waves completed!");
    }

    IEnumerator SpawnWave(Wave wave)
    {
        isSpawning = true;
        enemiesRemaining = wave.enemyCount;

        UIManager.Instance.ShowWaveText(currentWaveIndex + 1);

        int spawned = 0;
        while (spawned < wave.enemyCount)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyPrefab = wave.enemyPrefabs[Random.Range(0, wave.enemyPrefabs.Length)];
            
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            spawned++;

            yield return new WaitForSeconds(Mathf.Max(maxSpawnRate, wave.spawnInterval));
        }

        isSpawning = false;
    }

    public void OnEnemyDeath()
    {
        enemiesRemaining--;
        if (!isSpawning && enemiesRemaining <= 0)
        {
            UIManager.Instance.ShowWaveComplete();
        }
    }
}   