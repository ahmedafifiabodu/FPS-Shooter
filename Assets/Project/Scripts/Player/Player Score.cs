using TMPro;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public static PlayerScore Instance { get; private set; }

    private int score = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void IncreaseScore(int increaseAmount)
    {
        score += increaseAmount;
        scoreText.text = "Score: " + score;
    }

    public int GetScore() => score;

    internal void SetScore(int score)
    {
        this.score = score;
        scoreText.text = "Score: " + score;
    }
}