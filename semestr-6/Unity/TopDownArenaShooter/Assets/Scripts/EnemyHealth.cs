using UnityEngine;
public class EnemyHealth : MonoBehaviour
{
    public EnemyType enemyType;
    private float currentHealth;

    void Start()
    {
        currentHealth = enemyType.maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        int score = Random.Range(enemyType.minScore, enemyType.maxScore + 1);
        ScoreManager.Instance.AddScore(score);
        Destroy(gameObject);
    }
}
