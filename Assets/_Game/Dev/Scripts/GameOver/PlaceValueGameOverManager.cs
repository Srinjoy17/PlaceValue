using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueGameOverManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameWonImage;
        [SerializeField] private GameObject gameLostImage;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            PlaceValueGameManager gm = FindAnyObjectByType<PlaceValueGameManager>();
            if (gm != null)
                gm.ResetGame();

            PlaceValueUIFlowManager.Instance.ShowMainMenu();
        }

        public void GameWon()
        {
            gameWonImage.SetActive(true);
            gameLostImage.SetActive(false);
        }

        public void GameLost()
        {
            gameLostImage.SetActive(true);
            gameWonImage.SetActive(false);
        }
    }
}
