using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SlotManager slotManager;
    public TileManager tileManager;

    private int currentQuestion = 0;
    private int totalQuestions = 5;

    private int filledSlots = 0;
    private int requiredSlots = 0;

    private int[] currentDigits;

    void OnEnable()
    {
        GameEvents.OnTileCorrect += HandleCorrectTile;
        GameEvents.OnTileWrong += HandleWrongTile;
        GameEvents.OnGameOver += HandleGameOver;
    }

    void OnDisable()
    {
        GameEvents.OnTileCorrect -= HandleCorrectTile;
        GameEvents.OnTileWrong -= HandleWrongTile;
        GameEvents.OnGameOver -= HandleGameOver;
    }

    void Start()
    {
        NextQuestion();
    }

    // -----------------------------
    // Generate Next Question
    // -----------------------------
    void NextQuestion()
    {
        if (currentQuestion >= totalQuestions)
        {
            Debug.Log("GAME COMPLETE!");
            return;
        }

        currentQuestion++;
        Debug.Log("GENERATING QUESTION " + currentQuestion);

        int digitCount = 2;

        if (currentQuestion == 3 || currentQuestion == 4)
            digitCount = 3;

        if (currentQuestion == 5)
            digitCount = 4;

        GenerateNumber(digitCount);
    }

    // -----------------------------
    // Generate digits
    // -----------------------------
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

        Debug.Log("Question Digits: " + string.Join(",", currentDigits));
    }

    // -----------------------------
    // CORRECT TILE HANDLER
    // -----------------------------
    void HandleCorrectTile()
    {
        filledSlots++;

        if (filledSlots >= requiredSlots)
        {
            Debug.Log("QUESTION COMPLETE!");
            GameEvents.OnCorrectPlacement.Invoke();
            Invoke("NextQuestion", 1f);
        }
    }

    // -----------------------------
    // WRONG TILE HANDLER
    // -----------------------------
    void HandleWrongTile()
    {
        Debug.Log("WRONG TILE — reduce health");
        // Health reduction will be added when we add health system
        // GameEvents.OnHealthChanged?.Invoke(...)
    }

    // -----------------------------
    // GAME OVER
    // -----------------------------
    void HandleGameOver()
    {
        Debug.Log("GAME OVER — Time up or Health = 0");
    }
}
