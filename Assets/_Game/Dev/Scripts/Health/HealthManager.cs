using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class HealthManager : MonoBehaviour
    {
        // UI Heart Objects (ON = visible when health is available)
        public GameObject heart1_On;
        public GameObject heart2_On;
        public GameObject heart3_On;

        // UI Heart Objects (OFF = visible when health is lost)
        public GameObject heart1_Off;
        public GameObject heart2_Off;
        public GameObject heart3_Off;

        // Player starts with 3 health points
        private int health = 3;

        void OnEnable()
        {
            // Subscribe to events when this object becomes active
            GameEvents.OnTileWrong += ReduceHealth;       // Triggered when the player makes a mistake
            GameEvents.OnHealthChanged += UpdateHealthUI; // Triggered whenever health value updates
        }

        void OnDisable()
        {
            // Unsubscribe to prevent memory leaks or double-calling when disabled
            GameEvents.OnTileWrong -= ReduceHealth;
            GameEvents.OnHealthChanged -= UpdateHealthUI;
        }

        // Reduces player's health when a wrong tile is selected
        void ReduceHealth()
        {
            health--; // Deduct 1 health

            // Notify all listeners that the health value has changed
            GameEvents.OnHealthChanged?.Invoke(health);

            // If health reaches 0 or below → trigger game over
            if (health <= 0)
            {
                GameEvents.OnGameOver?.Invoke();
            }
        }

        // Resets the player's health back to full (3 hearts)
        public void ResetHealth()
        {
            health = 3;
            GameEvents.OnHealthChanged?.Invoke(health); // Update UI immediately
        }

        // Updates heart UI visuals based on the current health value
        void UpdateHealthUI(int h)
        {
            // Turn ON hearts depending on remaining health
            heart1_On.SetActive(h >= 1);
            heart2_On.SetActive(h >= 2);
            heart3_On.SetActive(h >= 3);

            // Turn OFF hearts when health for that slot is lost
            heart1_Off.SetActive(h < 1);
            heart2_Off.SetActive(h < 2);
            heart3_Off.SetActive(h < 3);
        }
    }
}
