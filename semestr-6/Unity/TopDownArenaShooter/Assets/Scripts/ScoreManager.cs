using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public int scoreToUpgrade = 10;
    public TextMeshProUGUI scoreText;

    void Start(){
      UpdateScoreUI();
    }
    void Awake()
    {
        Instance = this;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
        UpdateScoreUI();

        if (score >= scoreToUpgrade)
        {
            UpgradeManager.Instance.ShowUpgradePanel();
            score = 0; // Сброс или увеличим порог
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }

}
