using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    // UI references assigned through the Inspector
    [SerializeField] private GameObject gameWonImage;  // Shown when the player wins
    [SerializeField] private GameObject gameLostImage; // Shown when the player loses
    [SerializeField] private Button restartButton;     // Button used to return to the main menu

    private void Start()
    {
        // Attach the restart function to the button's click event
        restartButton.onClick.AddListener(RestartGame);
    }

    // Called when the restart button is pressed
    // Loads the MainMenu scene and resets the game flow
    private void RestartGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Handles the "Game Won" state
    // Displays the win UI and hides the loss UI
    public void GameWon()
    {
        gameWonImage.SetActive(true);
        gameLostImage.SetActive(false);
    }

    // Handles the "Game Lost" state
    // Displays the loss UI and hides the win UI
    public void GameLost()
    {
        gameLostImage.SetActive(true);
        gameWonImage.SetActive(false);
    }
}
