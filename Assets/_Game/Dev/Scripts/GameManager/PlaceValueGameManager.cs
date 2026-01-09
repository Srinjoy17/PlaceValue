using UnityEngine;
using System.Collections.Generic;

namespace Eduzo.Games.PlaceValue
{
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

        // -----------------------------
        // GAME STATE
        // -----------------------------
        private int currentQuestionIndex = 0;
        private int totalQuestions = 5;

        private int filledSlots = 0;
        private int requiredSlots = 0;
        private int[] currentDigits;

        private bool gameStarted = false;

        // -----------------------------
        // MODE & SESSION
        // -----------------------------
        private bool isPractice =>
            PlaceValueGameModeManager.CurrentMode == PlaceValueGameMode.Practice;

        private List<int> customList =>
            PlaceValueGameSessionManager.Instance.customQuestions;

        private bool hasCustomNumbers =>
            customList != null && customList.Count > 0;

        // -----------------------------
        // EVENTS
        // -----------------------------
        void OnEnable()
        {
            PlaceValueGameEvents.OnPlaceValueTileCorrect += HandleCorrectTile;
            PlaceValueGameEvents.OnPlaceValueTileWrong += HandleWrongTile;
            PlaceValueGameEvents.OnPlaceValueGameOver += HandleGameOver;
        }

        void OnDisable()
        {
            PlaceValueGameEvents.OnPlaceValueTileCorrect -= HandleCorrectTile;
            PlaceValueGameEvents.OnPlaceValueTileWrong -= HandleWrongTile;
            PlaceValueGameEvents.OnPlaceValueGameOver -= HandleGameOver;
        }

        // -----------------------------
        // DO NOT AUTO START
        // -----------------------------
        void Start()
        {
            // Game starts ONLY via StartGame()
        }

        // -----------------------------
        // RESET GAME COMPLETELY
        // -----------------------------
        public void ResetGame()
        {
            gameStarted = false;
            currentQuestionIndex = 0;
            filledSlots = 0;
            requiredSlots = 0;

            tileManager.ResetTiles();
            slotManager.SetupSlots(new int[0]);
        }

        // -----------------------------
        // START GAME (CALLED FROM UI)
        // -----------------------------
        public void StartGame()
        {
            ResetGame();
            gameStarted = true;

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

            NextQuestion();
        }

        // -----------------------------
        // NEXT QUESTION
        // -----------------------------
        void NextQuestion()
        {
            if (hasCustomNumbers)
                totalQuestions = customList.Count;

            // GAME FINISHED
            if (currentQuestionIndex >= totalQuestions)
            {
                if (isPractice)
                {
                    PlaceValueUIFlowManager.Instance.ShowMainMenu();
                    return;
                }

                PlaceValueUIFlowManager.Instance.ShowGameOver();
                gameOverManager.GameWon();
                PlaceValueAudioManager.Instance.PlaySFX("win");
                timerManager.StopTimer();
                return;
            }

            // LOAD QUESTION
            if (hasCustomNumbers)
            {
                GenerateCustomNumber(customList[currentQuestionIndex]);
            }
            else
            {
                int digitCount = 2;
                if (currentQuestionIndex == 2 || currentQuestionIndex == 3)
                    digitCount = 3;
                if (currentQuestionIndex >= 4)
                    digitCount = 4;

                GenerateRandomNumber(digitCount);
            }

            if (!isPractice)
            {
                PlaceValueHealthManager hm =
                    FindAnyObjectByType<PlaceValueHealthManager>();
                if (hm != null) hm.ResetHealth();

                timerManager.ResetTimer();
            }

            currentQuestionIndex++;
        }

        // -----------------------------
        // RANDOM NUMBER
        // -----------------------------
        void GenerateRandomNumber(int digits)
        {
            currentDigits = new int[digits];

            for (int i = 0; i < digits; i++)
                currentDigits[i] = Random.Range(0, 10);

            if (currentDigits[0] == 0)
                currentDigits[0] = Random.Range(1, 10);

            ApplyDigits();
        }

        // -----------------------------
        // CUSTOM NUMBER
        // -----------------------------
        void GenerateCustomNumber(int number)
        {
            string numStr = number.ToString();
            currentDigits = new int[numStr.Length];

            for (int i = 0; i < numStr.Length; i++)
                currentDigits[i] = numStr[i] - '0';

            ApplyDigits();
        }

        // -----------------------------
        // APPLY DIGITS
        // -----------------------------
        void ApplyDigits()
        {
            requiredSlots = currentDigits.Length;
            filledSlots = 0;

            slotManager.SetupSlots(currentDigits);
            tileManager.SetupTiles(currentDigits);
        }

        // -----------------------------
        // TILE EVENTS
        // -----------------------------
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

        void HandleWrongTile()
        {
            if (isPractice) return;
        }

        // -----------------------------
        // GAME LOST
        // -----------------------------
        void HandleGameOver()
        {
            if (isPractice) return;

            PlaceValueUIFlowManager.Instance.ShowGameOver();
            gameOverManager.GameLost();
            PlaceValueAudioManager.Instance.PlaySFX("lose");
            timerManager.StopTimer();
        }
    }
}
