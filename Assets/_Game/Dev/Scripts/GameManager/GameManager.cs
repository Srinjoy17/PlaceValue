using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eduzo.Games.PlaceValue
{
    public class GameManager : MonoBehaviour
    {
        // Core managers assigned from Inspector
        public SlotManager slotManager;
        public TileManager tileManager;
        public GameOverManager gameOverManager;
        public TimerManager timerManager;

        public GameObject timerUI;
        public GameObject healthUI;
        public GameObject scoreUI;

        private int currentQuestion = 0;
        private int totalQuestions = 5;

        private int filledSlots = 0;
        private int requiredSlots = 0;
        private int[] currentDigits;

        private bool isTutorial = true;

        // PRACTICE MODE CHECK
        private bool isPractice => GameModeManager.CurrentMode == GameMode.Practice;
        private List<int> customList => GameSessionManager.Instance.customQuestions;
        private bool hasCustomNumbers => customList != null && customList.Count > 0;

        // ----------------------------------------------------
        // EVENT LISTENERS
        // ----------------------------------------------------
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
        // GAME START
        // ----------------------------------------------------
        void Start()
        {
            // Start session
            GameSessionManager.Instance.StartSession(
                GameModeManager.CurrentMode,
                hasCustomNumbers ? customList : null
            );

            if (isPractice)
            {
                timerUI.SetActive(false);
                healthUI.SetActive(false);
                scoreUI?.SetActive(false);
                timerManager.StopTimer();
            }

            StartTutorial();
        }

        // ----------------------------------------------------
        // TUTORIAL
        // ----------------------------------------------------
        void StartTutorial()
        {
            int[] tutorialDigits = { 4, 2 };

            currentDigits = tutorialDigits;
            requiredSlots = tutorialDigits.Length;
            filledSlots = 0;

            slotManager.tutorialMode = true;
            slotManager.SetupSlots(tutorialDigits);
            tileManager.SetupTiles(tutorialDigits);
        }

        // ----------------------------------------------------
        // NEXT QUESTION
        // ----------------------------------------------------
        void NextQuestion()
        {
            if (hasCustomNumbers)
                totalQuestions = customList.Count;

            if (currentQuestion >= totalQuestions)
            {
                if (isPractice)
                {
                    SceneManager.LoadScene("GameModes");
                    return;
                }

                // WIN
                gameOverManager.gameObject.SetActive(true);
                gameOverManager.GameWon();
                AudioManager.Instance.PlaySFX("win");

                PrintDetailedDebugReport();
                return;
            }

            currentQuestion++;

            if (!isPractice)
                timerManager.ResetTimer();

            if (hasCustomNumbers)
                GenerateCustomNumber(customList[currentQuestion - 1]);
            else
            {
                int digitCount = 2;
                if (currentQuestion == 3 || currentQuestion == 4) digitCount = 3;
                if (currentQuestion == 5) digitCount = 4;

                GenerateRandomNumber(digitCount);
            }
        }

        // ----------------------------------------------------
        // NUMBER GENERATION
        // ----------------------------------------------------
        void GenerateRandomNumber(int digits)
        {
            currentDigits = new int[digits];

            for (int i = 0; i < digits; i++)
                currentDigits[i] = Random.Range(0, 10);

            if (currentDigits[0] == 0)
                currentDigits[0] = Random.Range(1, 10);

            ApplyDigitsToGame();
        }

        void GenerateCustomNumber(int number)
        {
            string numStr = number.ToString();
            currentDigits = new int[numStr.Length];

            for (int i = 0; i < numStr.Length; i++)
                currentDigits[i] = int.Parse(numStr[i].ToString());

            ApplyDigitsToGame();
        }

        void ApplyDigitsToGame()
        {
            requiredSlots = currentDigits.Length;
            filledSlots = 0;

            slotManager.SetupSlots(currentDigits);
            tileManager.SetupTiles(currentDigits);
        }

        // ----------------------------------------------------
        // CORRECT TILE DROP
        // ----------------------------------------------------
        void HandleCorrectTile()
        {
            filledSlots++;

            if (isTutorial && filledSlots >= requiredSlots)
            {
                isTutorial = false;
                slotManager.tutorialMode = false;
                slotManager.ClearPreview();

                Invoke(nameof(NextQuestion), 1f);
                return;
            }

            if (filledSlots >= requiredSlots)
            {
                GameSessionManager.Instance.LogQuestion(
                    currentQuestion,
                    int.Parse(string.Join("", currentDigits)),
                    true,
                    0,
                    timerManager.GetTimeSpent()
                );

                if (!isPractice)
                    GameEvents.OnCorrectPlacement?.Invoke();

                Invoke(nameof(NextQuestion), 1f);
            }
        }

        // ----------------------------------------------------
        // WRONG TILE DROP
        // ----------------------------------------------------
        void HandleWrongTile()
        {
            if (isTutorial || isPractice)
                return;

            GameSessionManager.Instance.LogQuestion(
                currentQuestion,
                int.Parse(string.Join("", currentDigits)),
                false,
                1,
                timerManager.GetTimeSpent()
            );
        }

        // ----------------------------------------------------
        // GAME OVER (LOSS)
        // ----------------------------------------------------
        void HandleGameOver()
        {
            if (isPractice)
                return;

            gameOverManager.gameObject.SetActive(true);
            gameOverManager.GameLost();
            AudioManager.Instance.PlaySFX("lose");

            timerManager.StopTimer();

            PrintDetailedDebugReport();
        }

        // ----------------------------------------------------
        // DETAILED DEBUG CONSOLE REPORT
        // ----------------------------------------------------
        void PrintDetailedDebugReport()
        {
            var logs = GameSessionManager.Instance.sessionData.logs;

            int total = logs.Count;
            int correct = 0;
            float totalTime = 0f;

            foreach (var log in logs)
            {
                if (log.isCorrect)
                    correct++;

                totalTime += log.timeTaken;
            }

            int wrong = total - correct;
            float scorePercent = total > 0 ? (correct / (float)total) * 100f : 0f;

            string report =
                "\n========== GAME SCORE SUMMARY ==========\n" +
                $"Score: {scorePercent:0}%\n" +
                $"Active Time: {totalTime:0.00}s\n" +
                $"Idle Time: 0s\n\n" +
                $"Total Responses: {total}\n" +
                $"Correct Answers: {correct}\n" +
                $"Wrong Answers: {wrong}\n\n" +
                "========== QUESTION BREAKDOWN ==========\n";

            foreach (var log in logs)
            {
                report +=
                    $"• Question {log.questionIndex}\n" +
                    $"Question: Arrange in correct order\n" +
                    $"Correct Answer: {log.targetNumber}\n" +
                    $"Result: {(log.isCorrect ? "CORRECT" : "INCORRECT")}\n" +
                    $"Response Time: {log.timeTaken:0.00}s\n" +
                    $"Mistakes: {(log.isCorrect ? "None" : "Wrong answer selected")}\n" +
                    "---------------------------------------\n";
            }

            report += "=======================================\n";

            Debug.Log(report);
        }
    }
}
