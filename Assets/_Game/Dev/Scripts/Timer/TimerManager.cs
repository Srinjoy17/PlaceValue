using UnityEngine;
using TMPro;

namespace Eduzo.Games.PlaceValue
{
    public class TimerManager : MonoBehaviour
    {
        public TMP_Text timerText;

        private float timeRemaining = 60f;
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
        }

        public void ResetTimer()
        {
            timeRemaining = 60f;
            running = true;
        }

        public void StopTimer()
        {
            running = false;
        }

        // 🔥 THIS IS CRITICAL
        public float GetTimeSpent()
        {
            return 60f - timeRemaining;
        }
    }
}
