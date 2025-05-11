using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;
    public TextMeshProUGUI healthText;
    
    [Header("Респавн")]
    public float respawnTime = 12f;
    private float respawnTimer;
    private bool isDead = false;
    public TextMeshProUGUI respawnTimerText;
    public GameObject respawnPanel;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private PlayerMovement playerMovement;
    private PlayerShooting playerShooting;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Сохраняем начальную позицию для респавна
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        playerMovement = GetComponent<PlayerMovement>();
        playerShooting = GetComponent<PlayerShooting>();
        
        // Скрываем панель респавна при старте
        if (respawnPanel != null)
            respawnPanel.SetActive(false);
    }
    
    void Update()
    {
        if (isDead)
        {
            respawnTimer -= Time.deltaTime;
            
            if (respawnTimerText != null)
                respawnTimerText.text = "Возрождение через: " + respawnTimer.ToString("0");
            
            if (respawnTimer <= 0)
            {
                Respawn();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Player HP: " + currentHealth);
        UpdateHealthUI();
        
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Игрок погиб. Ожидание респавна...");
        isDead = true;
        respawnTimer = respawnTime;
        
        // Отключаем компоненты игрока
        GetComponent<Collider>().enabled = false;
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (playerShooting != null)
            playerShooting.enabled = false;
        
        // Делаем игрока невидимым или запускаем анимацию смерти
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;
        
        // Показываем панель респавна
        if (respawnPanel != null)
            respawnPanel.SetActive(true);
        
        // Разблокируем курсор для управления интерфейсом
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void Respawn()
    {
        Debug.Log("Игрок возрождён");
        isDead = false;
        
        // Включаем коллайдер
        GetComponent<Collider>().enabled = true;
        
        // Включаем компоненты
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (playerShooting != null)
            playerShooting.enabled = true;
        
        // Восстанавливаем здоровье
        currentHealth = maxHealth;
        UpdateHealthUI();
        
        // Делаем игрока видимым
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = true;
        
        // Перемещаем игрока на стартовую позицию
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        // Скрываем панель респавна
        if (respawnPanel != null)
            respawnPanel.SetActive(false);
        
        // Блокируем курсор для игры
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth.ToString("0");
    }
}
