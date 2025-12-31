using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceValueMainMenu : MonoBehaviour
{
    [Header("Button Click Particle")]
    public ParticleSystem buttonClickParticle;

    [Tooltip("Delay to allow particle to play before action")]
    public float actionDelay = 0.25f;

    void Start()
    {
        // Play Main Menu Background Music
        PlaceValueAudioManager.Instance.PlayBG("mainmenu");
    }

    // ----------------------------------------------------
    // PLAY GAME
    // ----------------------------------------------------
    public void PlayGame()
    {
        PlaceValueAudioManager.Instance.PlaySFX("button");

        PlayParticle();

        // Switch to Game BG
        PlaceValueAudioManager.Instance.PlayBG("game");

        // Load scene after small delay
        Invoke(nameof(LoadGameModes), actionDelay);
    }

    void LoadGameModes()
    {
        SceneManager.LoadScene("PlaceValueGameModes");
    }

    // ----------------------------------------------------
    // EXIT GAME
    // ----------------------------------------------------
    public void ExitGame()
    {
        PlaceValueAudioManager.Instance.PlaySFX("button");

        PlayParticle();

        Invoke(nameof(QuitGame), actionDelay);
    }

    void QuitGame()
    {
        Application.Quit();
        Debug.Log("GAME QUIT");
    }

    // ----------------------------------------------------
    // PARTICLE SPAWN
    // ----------------------------------------------------
    void PlayParticle()
    {
        if (buttonClickParticle == null) return;

        ParticleSystem ps = Instantiate(
            buttonClickParticle,
            transform.position,
            Quaternion.identity,
            transform.parent   // keeps it in UI hierarchy
        );

        ps.Play();

        float life = ps.main.duration + ps.main.startLifetime.constantMax;
        Destroy(ps.gameObject, life);
    }
}
