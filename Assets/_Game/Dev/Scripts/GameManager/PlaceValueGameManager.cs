using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Eduzo.Games.PlaceValue;

public class PlaceValueGameManager : MonoBehaviour
{
    [Header("Core Managers")]
    public PlaceValueSlotManager slotManager;
    public PlaceValueTileManager tileManager;
    public PlaceValueGameOverManager gameOverManager;
    public PlaceValueTimerManager timerManager;

    [Header("UI")]
    public GameObject timerUI;
    public GameObject healthUI;
    public GameObject scoreUI;

    // Question flow
    private int currentQuestion = 0;
    private int totalQuestions = 5;

    // Slot tracking
    private int filledSlots = 0;
    private int requiredSlots = 0;
    private int[] currentDigits;

    // Mode checks
    private bool isPractice => PlaceValueGameModeManager.CurrentMode == PlaceValueGameMode.Practice;
    private List<int> customList => PlaceValueGameSessionManager.Instance.customQuestions;
    private bool hasCustomNumbers => customList != null && customList.Count > 0;

    // ----------------------------------------------------
    // EVENT LISTENERS
    // ----------------------------------------------------
    void OnEnable()
    {
        PlaceValueGameEvents.OnPlaceValueTileCorrect += HandleCorrectTile;
        PlaceValueGameEvents.OnPlaceValueTileWrong += HandleWrongTile;
        PlaceValueGameEvents.OnPlaceValueGameOver += HandleGameOver;

        gameOverManager.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        PlaceValueGameEvents.OnPlaceValueTileCorrect -= HandleCorrectTile;
        PlaceValueGameEvents.OnPlaceValueTileWrong -= HandleWrongTile;
        PlaceValueGameEvents.OnPlaceValueGameOver -= HandleGameOver;
    }

    // ----------------------------------------------------
    // GAME START
    // ----------------------------------------------------
    void Start()
    {
        // Practice mode UI handling
        if (isPractice)
        {
            timerUI.SetActive(false);
            healthUI.SetActive(false);
            scoreUI?.SetActive(false);
            timerManager.StopTimer();
        }
        else
        {
            timerUI.SetActive(true);
            healthUI.SetActive(true);
            timerManager.ResetTimer();
        }

        // Start first question directly
        NextQuestion();
    }

    // ----------------------------------------------------
    // NEXT QUESTION
    // ----------------------------------------------------
    void NextQuestion()
    {
        // Custom mode → number of questions = custom entries
        if (hasCustomNumbers)
            totalQuestions = customList.Count;

        // All questions completed
        if (currentQuestion >= totalQuestions)
        {
            if (isPractice)
            {
                SceneManager.LoadScene("PlaceValueGameModes");
                return;
            }

            gameOverManager.gameObject.SetActive(true);
            gameOverManager.GameWon();
            PlaceValueAudioManager.Instance.PlaySFX("win");
            timerManager.StopTimer();
            return;
        }

        currentQuestion++;

        // Reset systems per question (TEST MODE ONLY)
        if (!isPractice)
        {
            PlaceValueHealthManager hm = FindAnyObjectByType < PlaceValueHealthManager>();
            if (hm != null) hm.ResetHealth();

            timerManager.ResetTimer();
        }

        Debug.Log("QUESTION " + currentQuestion);

        // Generate question
        if (hasCustomNumbers)
        {
            GenerateCustomNumber(customList[currentQuestion - 1]);
        }
        else
        {
            int digitCount = 2;
            if (currentQuestion == 3 || currentQuestion == 4) digitCount = 3;
            if (currentQuestion >= 5) digitCount = 4;

            GenerateRandomNumber(digitCount);
        }
    }

    // ----------------------------------------------------
    // RANDOM NUMBER
    // ----------------------------------------------------
    void GenerateRandomNumber(int digits)
    {
        currentDigits = new int[digits];

        for (int i = 0; i < digits; i++)
            currentDigits[i] = Random.Range(0, 10);

        if (currentDigits[0] == 0)
            currentDigits[0] = Random.Range(1, 10);

        ApplyDigits();
    }

    // ----------------------------------------------------
    // CUSTOM NUMBER
    // ----------------------------------------------------
    void GenerateCustomNumber(int number)
    {
        string numStr = number.ToString();
        currentDigits = new int[numStr.Length];

        for (int i = 0; i < numStr.Length; i++)
            currentDigits[i] = numStr[i] - '0';

        ApplyDigits();
    }

    // ----------------------------------------------------
    // APPLY DIGITS
    // ----------------------------------------------------
    void ApplyDigits()
    {
        requiredSlots = currentDigits.Length;
        filledSlots = 0;

        slotManager.SetupSlots(currentDigits);
        tileManager.SetupTiles(currentDigits);

        Debug.Log("Digits: " + string.Join(",", currentDigits));
    }

    // ----------------------------------------------------
    // CORRECT TILE
    // ----------------------------------------------------
    void HandleCorrectTile()
    {
        filledSlots++;

        if (filledSlots >= requiredSlots)
        {
            if (!isPractice)
                PlaceValueGameEvents.OnPlaceValueCorrectPlacement?.Invoke();

            Invoke(nameof(NextQuestion), 1f);
        }
    }

    // ----------------------------------------------------
    // WRONG TILE
    // ----------------------------------------------------
    void HandleWrongTile()
    {
        if (isPractice)
        {
            Debug.Log("Practice Mode → No penalty");
            return;
        }

        Debug.Log("Wrong tile → Health reduced");
    }

    // ----------------------------------------------------
    // GAME OVER
    // ----------------------------------------------------
    void HandleGameOver()
    {
        if (isPractice)
            return;

        gameOverManager.gameObject.SetActive(true);
        gameOverManager.GameLost();
        PlaceValueAudioManager.Instance.PlaySFX("lose");
        timerManager.StopTimer();
    }
}
