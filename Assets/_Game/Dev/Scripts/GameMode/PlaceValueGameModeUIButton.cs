using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueGameModeUIButton : MonoBehaviour
    {
        public PlaceValueCustomNumberUI form;   // Drag from inspector

        public void PlayPractice()
        {
            form.Open(PlaceValueGameMode.Practice);
            PlaceValueAudioManager.Instance.PlaySFX("button");
        }

        public void PlayTest()
        {
            form.Open(PlaceValueGameMode.Test);
            PlaceValueAudioManager.Instance.PlaySFX("button");
        }
    }
}
