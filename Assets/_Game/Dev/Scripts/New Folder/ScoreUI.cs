using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TMP_Text scoreText;

    void OnEnable()
    {
        GameEvents.OnScoreUpdated += UpdateScore;
    }

    void OnDisable()
    {
        GameEvents.OnScoreUpdated -= UpdateScore;
    }

    void UpdateScore(int scoreValue)
    {
        scoreText.text = scoreValue.ToString();   // NO percentage symbol
    }
}
