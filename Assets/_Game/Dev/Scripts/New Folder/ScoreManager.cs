using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int correctCount = 0;
    private int wrongCount = 0;

    private int finalScore = 0;   // store plain number (0–100)

    void OnEnable()
    {
        GameEvents.OnTileCorrect += AddCorrect;
        GameEvents.OnTileWrong += AddWrong;
        GameEvents.OnGameOver += FinalizeScore;
    }

    void OnDisable()
    {
        GameEvents.OnTileCorrect -= AddCorrect;
        GameEvents.OnTileWrong -= AddWrong;
        GameEvents.OnGameOver -= FinalizeScore;
    }

    // When tile is correct
    void AddCorrect()
    {
        correctCount++;
        UpdateScore();
    }

    // When tile is wrong
    void AddWrong()
    {
        wrongCount++;
        UpdateScore();
    }

    // Score calculation (only number)
    void UpdateScore()
    {
        int totalAttempts = correctCount + wrongCount;

        if (totalAttempts == 0) return;

        finalScore = Mathf.RoundToInt((correctCount / (float)totalAttempts) * 100f);

        GameEvents.OnScoreUpdated?.Invoke(finalScore); // send just number
    }

    // Called when game ends
    void FinalizeScore()
    {
        GameEvents.OnScoreFinal?.Invoke();
        Debug.Log("Final Score: " + finalScore);
    }

    public int GetFinalScore()
    {
        return finalScore;   // no % sign
    }
}
