using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int score = 0;
    public TextMeshProUGUI scoreText;

    void Start(){
        UpdateScoreUI();
    }
    void Awake()
    {
        Instance = this;
    }

    public bool SpendScore(int amount)
    {
        if (score >= amount)
        {
            score -= amount;
            UpdateScoreUI();
            return true;
        }
        return false;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
    }
}