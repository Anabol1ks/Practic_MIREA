using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public int startLives = 3;
    public float respawnTime = 3f;

    private int currentScore;
    private int currentLives;
    private bool gameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentLives = startLives;
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UIManager.Instance.UpdateScore(currentScore);
    }

    public void PlayerDied()
    {
        if (gameOver) return;

        currentLives--;
        UIManager.Instance.UpdateLives(currentLives);

        if (currentLives <= 0)
        {
            GameOver();
        }
        else
        {
            Invoke("RespawnPlayer", respawnTime);
        }
    }

    void RespawnPlayer()
    {
        // Логика респавна игрока
        var player = FindObjectOfType<PlayerHealth>();
        player.transform.position = Vector3.zero;
        player.Heal(player.maxHealth);
    }

    void GameOver()
    {
        gameOver = true;
        UIManager.Instance.ShowGameOver(currentScore);
        Time.timeScale = 0f;
    }
}