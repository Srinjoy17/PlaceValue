using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueGameModeUIButton : MonoBehaviour
    {
        public PlaceValueCustomNumberUI form; // Drag from inspector

        // -------------------------
        // PRACTICE MODE
        // -------------------------
        public void PlayPractice()
        {
            PlaceValueAudioManager.Instance.PlaySFX("button");

            // Set mode
            PlaceValueGameModeManager.CurrentMode = PlaceValueGameMode.Practice;

            // Open number entry UI
            form.Open();
        }

        // -------------------------
        // TEST MODE
        // -------------------------
        public void PlayTest()
        {
            PlaceValueAudioManager.Instance.PlaySFX("button");

            // Set mode
            PlaceValueGameModeManager.CurrentMode = PlaceValueGameMode.Test;

            // Open number entry UI
            form.Open();
        }
    }
}
