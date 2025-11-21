using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TMP_Text timerText;

    private float timeRemaining = 60f; // 1 minute
    private bool running = true;

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
}
