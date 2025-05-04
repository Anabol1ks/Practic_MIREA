using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Health")]
    public Slider healthSlider;
    public Image healthFill;
    public Gradient healthGradient;

    [Header("Stamina")]
    public Slider staminaSlider;

    [Header("Wave Info")]
    public TextMeshProUGUI waveText;
    public GameObject waveCompletePanel;
    public float waveTextDuration = 2f;

    [Header("Score/Lives")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

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
    }

    public void UpdateHealth(float healthNormalized)
    {
        healthSlider.value = healthNormalized;
        healthFill.color = healthGradient.Evaluate(healthNormalized);
    }

    public void UpdateStamina(float staminaNormalized)
    {
        staminaSlider.value = staminaNormalized;
    }

    public void UpdateScore(int score)
    {
        scoreText.text = $"Score: {score}";
    }

    public void UpdateLives(int lives)
    {
        livesText.text = $"Lives: {lives}";
    }

    public void ShowWaveText(int waveNumber)
    {
        waveText.text = $"Wave {waveNumber}";
        waveText.gameObject.SetActive(true);
        Invoke("HideWaveText", waveTextDuration);
    }

    void HideWaveText()
    {
        waveText.gameObject.SetActive(false);
    }

    public void ShowWaveComplete()
    {
        waveCompletePanel.SetActive(true);
        Invoke("HideWaveComplete", 2f);
    }

    void HideWaveComplete()
    {
        waveCompletePanel.SetActive(false);
    }

    public void ShowGameOver(int finalScore)
    {
        finalScoreText.text = $"Final Score: {finalScore}";
        gameOverPanel.SetActive(true);
    }
}