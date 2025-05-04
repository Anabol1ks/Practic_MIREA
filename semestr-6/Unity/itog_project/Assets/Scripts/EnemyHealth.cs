using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 50;
    public int scoreValue = 10;
    public bool dropHealthPickup = true;
    public float healthPickupChance = 0.3f;
    public GameObject healthPickupPrefab;

    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    private int currentHealth;
    private bool isDead;
    private EnemyAI enemyAI;

    public bool IsDead => isDead;
    public float HealthNormalized => (float)currentHealth / maxHealth;

    void Awake()
    {
        enemyAI = GetComponent<EnemyAI>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        onDamage.Invoke();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        onDeath.Invoke();
        
        // Награда игроку
        GameManager.Instance.AddScore(scoreValue);
        
        // Возможность выпадения здоровья
        if (dropHealthPickup && Random.value <= healthPickupChance && healthPickupPrefab)
        {
            Instantiate(healthPickupPrefab, transform.position, Quaternion.identity);
        }

        // Отключаем AI
        if (enemyAI) enemyAI.OnDeath();
        
        // Уничтожаем через 3 секунды
        Destroy(gameObject, 3f);
    }
}