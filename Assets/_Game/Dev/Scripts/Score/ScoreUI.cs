using TMPro;
using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class ScoreUI : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;

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
            scoreText.text = "Score:" + scoreValue.ToString();   // NO percentage symbol
        }
    }
}
