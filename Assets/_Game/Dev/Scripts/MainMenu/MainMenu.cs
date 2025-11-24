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
        //  Button Click Sound
        AudioManager.Instance.PlaySFX("button");

        //  Switch to Game BG before loading scene
        AudioManager.Instance.PlayBG("game");

        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        //  Button Click Sound
        AudioManager.Instance.PlaySFX("button");

        Application.Quit();
        Debug.Log("GAME QUIT");
    }
}
