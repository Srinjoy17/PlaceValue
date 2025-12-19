using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        // Play Main Menu Background Music
        AudioManager.Instance.PlayBG("mainmenu");
    }

    public void PlayGame()
    {
        // Button Click Sound
        AudioManager.Instance.PlaySFX("button");

        // Switch to Game BG so that it continues into GameScene later
        AudioManager.Instance.PlayBG("game");

        // Load Game Mode Selection Scene
        SceneManager.LoadScene("GameModes");
    }

    public void ExitGame()
    {
        // Button Click Sound
        AudioManager.Instance.PlaySFX("button");

        Application.Quit();
        Debug.Log("GAME QUIT");
    }
}
