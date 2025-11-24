using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Core managers assigned from Inspector
    public SlotManager slotManager;
    public TileManager tileManager;
    public GameOverManager gameOverManager;
    public TimerManager timerManager;

    // Question flow
    private int currentQuestion = 0;
    private int totalQuestions = 5;

    // Slot tracking per question
    private int filledSlots = 0;
    private int requiredSlots = 0;

    // Stores the digits of the current question
    private int[] currentDigits;

    // Determines if the game is still in tutorial mode
    private bool isTutorial = true;


    // ----------------------------------------------------
    // Enable event listeners
    // ----------------------------------------------------
    void OnEnable()
    {
        GameEvents.OnTileCorrect += HandleCorrectTile;
        GameEvents.OnTileWrong += HandleWrongTile;
        GameEvents.OnGameOver += HandleGameOver;

        // Hide game over UI at the beginning
        gameOverManager.gameObject.SetActive(false);
    }

    // ----------------------------------------------------
    // Remove listeners to avoid over-subscription
    // ----------------------------------------------------
    void OnDisable()
    {
        GameEvents.OnTileCorrect -= HandleCorrectTile;
        GameEvents.OnTileWrong -= HandleWrongTile;
        GameEvents.OnGameOver -= HandleGameOver;
    }


    // ----------------------------------------------------
    // Game entry point
    // ----------------------------------------------------
    void Start()
    {
        StartTutorial();
    }


    // ----------------------------------------------------
    // TUTORIAL — first question with fixed example digits
    // Timer does NOT run during tutorial
    // ----------------------------------------------------
    void StartTutorial()
    {
        Debug.Log("TUTORIAL STARTED");

        int[] tutorialDigits = { 4, 2 }; // Example digits for demonstration

        currentDigits = tutorialDigits;
        requiredSlots = tutorialDigits.Length;
        filledSlots = 0;

        slotManager.tutorialMode = true; // Show preview arrows/guide

        // Set up tutorial UI
        slotManager.SetupSlots(tutorialDigits);
        tileManager.SetupTiles(tutorialDigits);

        // Timer is intentionally disabled during tutorial
    }


    // ----------------------------------------------------
    // Starts generating all normal questions after tutorial
    // ----------------------------------------------------
    void NextQuestion()
    {
        // When all questions are completed → show WIN screen
        if (currentQuestion >= totalQuestions)
        {
            Debug.Log("GAME COMPLETE!");

            gameOverManager.gameObject.SetActive(true);
            gameOverManager.GameWon();

            AudioManager.Instance.PlaySFX("win");
            return;
        }

        // Restart timer per question
        timerManager.ResetTimer();

        currentQuestion++;

        // Reset player health for each new question
        HealthManager hm = FindAnyObjectByType<HealthManager>();
        if (hm != null) hm.ResetHealth();

        Debug.Log("GENERATING QUESTION " + currentQuestion);

        // Determine digit count based on level progression
        int digitCount = 2; // Default
        if (currentQuestion == 3 || currentQuestion == 4) digitCount = 3;
        if (currentQuestion == 5) digitCount = 4;

        // Generate and show the new question
        GenerateNumber(digitCount);
    }


    // ----------------------------------------------------
    // Generate random digits for the current question
    // Ensures digit[0] is never zero
    // ----------------------------------------------------
    void GenerateNumber(int digits)
    {
        currentDigits = new int[digits];

        for (int i = 0; i < digits; i++)
            currentDigits[i] = Random.Range(0, 10);

        // Prevent numbers like 012 or 006
        if (currentDigits[0] == 0)
            currentDigits[0] = Random.Range(1, 10);

        requiredSlots = digits;
        filledSlots = 0;

        // Update UI
        slotManager.SetupSlots(currentDigits);
        tileManager.SetupTiles(currentDigits);

        Debug.Log("Digits: " + string.Join(",", currentDigits));
    }


    // ----------------------------------------------------
    // Handles correct tile placement
    // Tutorial → completes without timer  
    // Normal game → moves to next question
    // ----------------------------------------------------
    void HandleCorrectTile()
    {
        filledSlots++;

        // When tutorial is complete (all slots filled)
        if (isTutorial && filledSlots >= requiredSlots)
        {
            Debug.Log("TUTORIAL COMPLETE!");

            isTutorial = false;
            slotManager.tutorialMode = false;

            // Remove preview markers
            slotManager.ClearPreview();

            // Start normal game after a short delay
            Invoke("NextQuestion", 1f);
            return;
        }

        // For normal gameplay
        if (filledSlots >= requiredSlots)
        {
            // Notify timer to reset itself
            GameEvents.OnCorrectPlacement?.Invoke();

            // Delay before going to the next question
            Invoke("NextQuestion", 1f);
        }
    }


    // ----------------------------------------------------
    // Handles WRONG tile dropping
    // Tutorial: NO health loss
    // Normal game: health is reduced elsewhere
    // ----------------------------------------------------
    void HandleWrongTile()
    {
        if (isTutorial)
        {
            Debug.Log("WRONG (Tutorial) — NO HEALTH LOSS");
            return;
        }

        Debug.Log("WRONG TILE — normal health reduction");
    }


    // ----------------------------------------------------
    // GAME OVER  
    // Shows loss screen + stops timer
    // ----------------------------------------------------
    void HandleGameOver()
    {
        Debug.Log("GAME OVER");

        // Activate game over UI
        gameOverManager.gameObject.SetActive(true);
        gameOverManager.GameLost();

        // Play game over sound
        AudioManager.Instance.PlaySFX("lose");

        // Stop countdown immediately
        timerManager.StopTimer();
    }
}
