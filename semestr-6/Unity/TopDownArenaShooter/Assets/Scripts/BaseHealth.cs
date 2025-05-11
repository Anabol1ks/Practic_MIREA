using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    public float currentHealth;
    public TextMeshProUGUI baseHealthText;
    
    public GameObject player;
    private PlayerHealth playerHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateBaseHealthUI();
        
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerHealth = player.GetComponent<PlayerHealth>();
        }
    }
    
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("База получила урон: " + amount + ". Осталось HP: " + currentHealth);
        UpdateBaseHealthUI();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Debug.Log("База уничтожена! Игра окончена.");
        
        // Остановить таймер для проверки рекорда
        if (HighScoreManager.Instance != null)
        {
            HighScoreManager.Instance.StopTimer();
        }
        
        Time.timeScale = 4f;
        
        // Небольшая задержка перед загрузкой меню
        Invoke("LoadMainMenu", 2f);
    }
    
    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Позже создадим эту сцену
    }
    
    void UpdateBaseHealthUI()
    {
        if (baseHealthText != null)
            baseHealthText.text = "База: " + currentHealth.ToString("0");
    }
} 