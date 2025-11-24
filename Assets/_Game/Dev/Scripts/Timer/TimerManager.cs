using System;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    private float timeRemaining = 60f;
    private bool running = true;

    private void Start()
    {
        GameEvents.OnCorrectPlacement += ResetTimer;
    }

    void Update()
    {
        if (!running) return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            running = false;
            GameEvents.OnGameOver?.Invoke();
        }

        int seconds = Mathf.CeilToInt(timeRemaining);
        timerText.text = "00:" + seconds.ToString("00");

        GameEvents.OnTimerTick?.Invoke(seconds);
    }

    // ---------------------------------------------------
    //  Always reset timer to 60 seconds
    // Called by GameManager on every new question
    // ---------------------------------------------------
    public void ResetTimer()
    {
        timeRemaining = 60f;
        running = true;   // ensure timer continues running
    }

    // ---------------------------------------------------
    //  Only used when game is finished or lost
    // ---------------------------------------------------
    public void StopTimer()
    {
        running = false;
    }
}
