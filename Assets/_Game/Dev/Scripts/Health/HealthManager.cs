using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject heart1_On;
    public GameObject heart2_On;
    public GameObject heart3_On;

    public GameObject heart1_Off;
    public GameObject heart2_Off;
    public GameObject heart3_Off;

    private int health = 3;

    void OnEnable()
    {
        GameEvents.OnTileWrong += ReduceHealth;
        GameEvents.OnHealthChanged += UpdateHealthUI;
    }

    void OnDisable()
    {
        GameEvents.OnTileWrong -= ReduceHealth;
        GameEvents.OnHealthChanged -= UpdateHealthUI;
    }

    // Called whenever tile is wrong
    void ReduceHealth()
    {
        health--;

        GameEvents.OnHealthChanged?.Invoke(health);

        if (health <= 0)
        {
            GameEvents.OnGameOver?.Invoke();
        }
    }

    // Updates UI
    void UpdateHealthUI(int h)
    {
        heart1_On.SetActive(h >= 1);
        heart2_On.SetActive(h >= 2);
        heart3_On.SetActive(h >= 3);

        heart1_Off.SetActive(h < 1);
        heart2_Off.SetActive(h < 2);
        heart3_Off.SetActive(h < 3);
    }
}
