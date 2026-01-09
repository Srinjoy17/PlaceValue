using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueMainMenu : MonoBehaviour
    {
        [Header("Button Click Particle")]
        public ParticleSystem buttonClickParticle;

        [Tooltip("Delay to allow particle to play before action")]
        public float actionDelay = 0.25f;

        void Start()
        {
            PlaceValueAudioManager.Instance.PlayBG("mainmenu");
        }

        // ----------------------------------------------------
        // PLAY GAME
        // ----------------------------------------------------
        public void PlayGame()
        {
            PlaceValueAudioManager.Instance.PlaySFX("button");
            PlayParticle();

            Invoke(nameof(OpenGameModes), actionDelay);
        }

        void OpenGameModes()
        {
            PlaceValueUIFlowManager.Instance.ShowGameModes();
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
        // PARTICLE
        // ----------------------------------------------------
        void PlayParticle()
        {
            if (buttonClickParticle == null) return;

            ParticleSystem ps = Instantiate(
                buttonClickParticle,
                transform.position,
                Quaternion.identity,
                transform.parent
            );

            ps.Play();

            float life = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(ps.gameObject, life);
        }
    }
}
