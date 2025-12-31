using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueScoreManager : MonoBehaviour
    {
        private int correctCount = 0;
        private int wrongCount = 0;

        private int finalScore = 0;   // store plain number (0–100)

        void OnEnable()
        {
            PlaceValueGameEvents.OnPlaceValueTileCorrect += AddCorrect;
            PlaceValueGameEvents.OnPlaceValueTileWrong += AddWrong;
            PlaceValueGameEvents.OnPlaceValueGameOver += FinalizeScore;
        }

        void OnDisable()
        {
            PlaceValueGameEvents.OnPlaceValueTileCorrect -= AddCorrect;
            PlaceValueGameEvents.OnPlaceValueTileWrong -= AddWrong;
            PlaceValueGameEvents.OnPlaceValueGameOver -= FinalizeScore;
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

            PlaceValueGameEvents.OnPlaceValueScoreUpdated?.Invoke(finalScore); // send just number
        }

        // Called when game ends
        void FinalizeScore()
        {
            PlaceValueGameEvents.OnPlaceValueScoreFinal?.Invoke();
            Debug.Log("Final Score: " + finalScore);
        }

        public int GetFinalScore()
        {
            return finalScore;   // no % sign
        }
    }
}
