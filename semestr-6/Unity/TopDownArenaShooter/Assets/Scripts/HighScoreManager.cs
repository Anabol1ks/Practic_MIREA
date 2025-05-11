using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class HighScoreManager : MonoBehaviour
{
    public static HighScoreManager Instance;
    
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI currentTimeText;
    
    private float gameTime = 0f;
    private float bestTime = 0f;
    private bool gameActive = true;
    
    private const string HIGH_SCORE_KEY = "HighScore";
    
    void Awake()
    {
        Instance = this;
        LoadHighScore();
    }
    
    void Start()
    {
        UpdateHighScoreUI();
    }
    
    void Update()
    {
        if (gameActive)
        {
            gameTime += Time.deltaTime;
            if (currentTimeText != null)
                currentTimeText.text = "Время: " + FormatTime(gameTime);
        }
    }
    
    public void StopTimer()
    {
        gameActive = false;
        
        // Проверяем, установлен ли новый рекорд
        if (gameTime > bestTime)
        {
            bestTime = gameTime;
            SaveHighScore();
            Debug.Log("Новый рекорд: " + FormatTime(bestTime));
        }
    }
    
    public void ResetTimer()
    {
        gameTime = 0f;
        gameActive = true;
    }
    
    private void LoadHighScore()
    {
        bestTime = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0f);
    }
    
    private void SaveHighScore()
    {
        PlayerPrefs.SetFloat(HIGH_SCORE_KEY, bestTime);
        PlayerPrefs.Save();
        UpdateHighScoreUI();
    }
    
    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "Рекорд: " + FormatTime(bestTime);
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
} 