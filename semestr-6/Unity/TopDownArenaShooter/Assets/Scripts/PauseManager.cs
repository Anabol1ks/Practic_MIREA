using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public Button continueButton, restartButton;

    void Start()
    {
        pausePanel.SetActive(false);
        continueButton.onClick.AddListener(ResumeGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        bool isPaused = Time.timeScale == 0f;
        Time.timeScale = isPaused ? 1f : 0f;
        pausePanel.SetActive(!isPaused);

        Cursor.lockState = isPaused ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isPaused;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
