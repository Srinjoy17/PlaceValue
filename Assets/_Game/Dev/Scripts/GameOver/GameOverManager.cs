using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
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
        SceneManager.LoadScene("MainMenu");
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
