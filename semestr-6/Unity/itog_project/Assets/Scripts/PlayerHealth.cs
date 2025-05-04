using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Events")]
    public UnityEvent onDamage;
    public UnityEvent onDeath;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;
        
        // Trigger damage event
        onDamage?.Invoke();

        Debug.Log($"Player took {damageAmount} damage. Health: {currentHealth}/{maxHealth}");

        // Check if player died
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        Debug.Log($"Player healed for {healAmount}. Health: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        currentHealth = 0;
        
        // Trigger death event
        onDeath?.Invoke();
        
        Debug.Log("Player died!");
        
        // You could add additional death behavior here
        // For example, game over screen, respawn, etc.
    }
}
