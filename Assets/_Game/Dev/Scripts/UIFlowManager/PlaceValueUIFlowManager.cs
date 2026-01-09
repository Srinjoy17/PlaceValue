using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueUIFlowManager : MonoBehaviour
    {
        public static PlaceValueUIFlowManager Instance;

        [Header("Main Panels")]
        public GameObject mainMenuPanel;
        public GameObject gameModePanel;
        public GameObject gameplayPanel;
        public GameObject gameOverPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                return; // DO NOT DESTROY
            }

            Instance = this;
        }

        private void Start()
        {
            ShowMainMenu();
        }

        // ---------------- FLOW METHODS ----------------

        public void ShowMainMenu()
        {
            DisableAll();
            SafeSetActive(mainMenuPanel, true);

            PlaceValueAudioManager.Instance.PlayBG("mainmenu");
        }

        public void ShowGameModes()
        {
            DisableAll();
            SafeSetActive(gameModePanel, true);
        }

        public void ShowGameplay()
        {
            DisableAll();
            SafeSetActive(gameplayPanel, true);

            PlaceValueAudioManager.Instance.PlayBG("game");
        }

        public void ShowGameOver()
        {
            DisableAll();
            SafeSetActive(gameOverPanel, true);
        }

        // ---------------- INTERNAL ----------------

        void DisableAll()
        {
            SafeSetActive(mainMenuPanel, false);
            SafeSetActive(gameModePanel, false);
            SafeSetActive(gameplayPanel, false);
            SafeSetActive(gameOverPanel, false);
        }

        void SafeSetActive(GameObject obj, bool value)
        {
            if (obj != null)
                obj.SetActive(value);
        }
    }
}
