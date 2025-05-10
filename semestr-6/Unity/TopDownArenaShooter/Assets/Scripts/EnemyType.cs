using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Type", menuName = "Enemy/Enemy Type")]
public class EnemyType : ScriptableObject
{
    public string enemyName;
    public GameObject enemyPrefab;
    public float maxHealth = 3f;
    public float moveSpeed = 3.5f;
    public float damage = 1f;
    public float attackRange = 1f;
    public bool isRanged = false;
    public int minScore = 1;
    public int maxScore = 3;
    public float spawnWeight = 1f; // Вес для вероятности появления
    public float unlockTime = 0f; // Время в секундах, когда этот тип врага начинает появляться
} 