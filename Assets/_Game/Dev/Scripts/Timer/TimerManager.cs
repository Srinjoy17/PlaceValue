using System;
using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    // Reference to the on-screen timer text (MM:SS format)
    public TMP_Text timerText;

    // Internal timer values
    private float timeRemaining = 60f;   // Starting countdown time
    private bool running = true;         // Controls whether the timer is active

    private void Start()
    {
        // Whenever a correct tile is placed, reset the countdown timer
        GameEvents.OnCorrectPlacement += ResetTimer;
    }

    void Update()
    {
        // If timer is paused or game is over, skip updates
        if (!running) return;

        // Reduce time based on frame time
        timeRemaining -= Time.deltaTime;

        // Timer should not go below 0
        if (timeRemaining <= 0)
        {
            timeRemaining = 0;
            running = false;

            // Trigger the Game Over event when time runs out
            GameEvents.OnGameOver?.Invoke();
        }

        // Convert float time into whole seconds for clean display
        int seconds = Mathf.CeilToInt(timeRemaining);

        // Update timer UI (always displayed as 00:XX)
        timerText.text = "00:" + seconds.ToString("00");

        // Broadcast the current timer value (optional for other systems)
        GameEvents.OnTimerTick?.Invoke(seconds);
    }

    // ---------------------------------------------------
    // Resets countdown back to 60 seconds
    // Called whenever the player makes a correct move
    // ---------------------------------------------------
    public void ResetTimer()
    {
        timeRemaining = 60f;
        running = true; // Ensure timer resumes counting
    }

    // ---------------------------------------------------
    // Stops the timer entirely — useful on game over or win
    // ---------------------------------------------------
    public void StopTimer()
    {
        running = false;
    }
}
