using TMPro;
using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueScoreUI : MonoBehaviour
    {
        public TextMeshProUGUI scoreText;

        void OnEnable()
        {
            PlaceValueGameEvents.OnPlaceValueScoreUpdated += UpdateScore;
        }

        void OnDisable()
        {
            PlaceValueGameEvents.OnPlaceValueScoreUpdated -= UpdateScore;
        }

        void UpdateScore(int scoreValue)
        {
            scoreText.text = "Score:" + scoreValue.ToString();   // NO percentage symbol
        }
    }
}
