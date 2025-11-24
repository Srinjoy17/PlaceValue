using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SlotManager slotManager;
    public TileManager tileManager;
    public GameOverManager gameOverManager;
    public TimerManager timerManager;

    private int currentQuestion = 0;
    private int totalQuestions = 5;

    private int filledSlots = 0;
    private int requiredSlots = 0;

    private int[] currentDigits;

    private bool isTutorial = true;   // First question is tutorial


    void OnEnable()
    {
        GameEvents.OnTileCorrect += HandleCorrectTile;
        GameEvents.OnTileWrong += HandleWrongTile;
        GameEvents.OnGameOver += HandleGameOver;

        gameOverManager.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        GameEvents.OnTileCorrect -= HandleCorrectTile;
        GameEvents.OnTileWrong -= HandleWrongTile;
        GameEvents.OnGameOver -= HandleGameOver;
    }


    // ----------------------------------------------------
    // START
    // ----------------------------------------------------
    void Start()
    {
        StartTutorial();
    }


    // ----------------------------------------------------
    // 🔥 TUTORIAL QUESTION
    // ----------------------------------------------------
    void StartTutorial()
    {
        Debug.Log("TUTORIAL STARTED");

        int[] tutorialDigits = { 4, 2 };   // dummy tutorial number

        currentDigits = tutorialDigits;
        requiredSlots = tutorialDigits.Length;
        filledSlots = 0;

        slotManager.tutorialMode = true; // enable tutorial preview

        slotManager.SetupSlots(tutorialDigits);
        tileManager.SetupTiles(tutorialDigits);

        // Timer must stay OFF during tutorial
       
    }


    // ----------------------------------------------------
    // 🔥 NORMAL QUESTIONS AFTER TUTORIAL
    // ----------------------------------------------------
    void NextQuestion()
    {
        if (currentQuestion >= totalQuestions)
        {
            Debug.Log("GAME COMPLETE!");

            gameOverManager.gameObject.SetActive(true);
            gameOverManager.GameWon();

            AudioManager.Instance.PlaySFX("win");
            return;
        }
        timerManager.ResetTimer();

        currentQuestion++;

        // Reset health for each new question
        HealthManager hm = FindAnyObjectByType<HealthManager>();
        if (hm != null) hm.ResetHealth();

        Debug.Log("GENERATING QUESTION " + currentQuestion);

        int digitCount = 2;
        if (currentQuestion == 3 || currentQuestion == 4) digitCount = 3;
        if (currentQuestion == 5) digitCount = 4;

        GenerateNumber(digitCount);

    }


    // ----------------------------------------------------
    // GENERATE DIGITS
    // ----------------------------------------------------
    void GenerateNumber(int digits)
    {
        currentDigits = new int[digits];

        for (int i = 0; i < digits; i++)
            currentDigits[i] = Random.Range(0, 10);

        if (currentDigits[0] == 0)
            currentDigits[0] = Random.Range(1, 10);

        requiredSlots = digits;
        filledSlots = 0;

        slotManager.SetupSlots(currentDigits);
        tileManager.SetupTiles(currentDigits);

        Debug.Log("Digits: " + string.Join(",", currentDigits));
    }


    // ----------------------------------------------------
    // CORRECT TILE HANDLER
    // ----------------------------------------------------
    void HandleCorrectTile()
    {
        filledSlots++;

        // Tutorial complete
        if (isTutorial && filledSlots >= requiredSlots)
        {
            Debug.Log("TUTORIAL COMPLETE!");

            isTutorial = false;
            slotManager.tutorialMode = false;
            slotManager.ClearPreview();

            // After tutorial → Start Timer + first question
            Invoke("NextQuestion", 1f);
            return;
        }

        // Normal question complete
        if (filledSlots >= requiredSlots)
        {
            GameEvents.OnCorrectPlacement?.Invoke();
            Invoke("NextQuestion", 1f);
        }
    }


    // ----------------------------------------------------
    // WRONG TILE HANDLER
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
    // ----------------------------------------------------
    void HandleGameOver()
    {
        Debug.Log("GAME OVER");

        gameOverManager.gameObject.SetActive(true);
        gameOverManager.GameLost();

        AudioManager.Instance.PlaySFX("lose");

        // Stop timer instantly
        timerManager.StopTimer();
    }
}
