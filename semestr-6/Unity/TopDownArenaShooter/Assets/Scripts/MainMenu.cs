using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public TextMeshProUGUI highScoreText;

    private const string HIGH_SCORE_KEY = "HighScore";
    
    void Start()
    {
        // Подключаем обработчики кнопок
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
        
        // Отображаем рекорд
        DisplayHighScore();
        
        // Разблокируем курсор
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Сбрасываем таймер
        Time.timeScale = 1f;
    }
    
    void StartGame()
    {
        SceneManager.LoadScene("GameScene"); // Название сцены с игрой
    }
    
    void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void DisplayHighScore()
    {
        if (highScoreText != null)
        {
            float bestTime = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0f);
            highScoreText.text = "Лучшее время: " + FormatTime(bestTime);
        }
    }
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}