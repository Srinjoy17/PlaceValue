using UnityEngine;
using System;
using System.Collections.Generic;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueGameSessionManager : MonoBehaviour
    {
        public static PlaceValueGameSessionManager Instance;

        public SessionDataSO sessionData;

        public string studentName = "Student";
        public PlaceValueGameMode currentMode;

        public List<int> customQuestions = new List<int>();
        private string sessionId;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // -----------------------------
        // START SESSION
        // -----------------------------
        public void StartSession(PlaceValueGameMode mode, List<int> customList = null)
        {
            currentMode = mode;
            sessionId = Guid.NewGuid().ToString();

            sessionData.Clear();

            if (customList != null)
                customQuestions = customList;
            else
                customQuestions.Clear();

            Debug.Log("SESSION STARTED: " + sessionId);
            Debug.Log("SESSION STORED QUESTIONS: " + string.Join(",", customQuestions));
        }

        // -----------------------------
        // END SESSION  ✅ (NEW)
        // -----------------------------
        public void EndSession()
        {
            sessionId = string.Empty;
            customQuestions.Clear();

            Debug.Log("SESSION ENDED");
        }

        // -----------------------------
        // LOG QUESTION
        // -----------------------------
        public void LogQuestion(
            int questionIndex,
            int targetNumber,
            bool isCorrect,
            int wrongAttempts,
            float timeTaken
        )
        {
            PlaceValueQuestionLog log = new PlaceValueQuestionLog
            {
                sessionId = sessionId,
                studentName = studentName,
                mode = currentMode,
                questionIndex = questionIndex,
                targetNumber = targetNumber,
                isCorrect = isCorrect,
                wrongAttempts = wrongAttempts,
                timeTaken = timeTaken,
                timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            sessionData.logs.Add(log);

            Debug.Log($"LOGGED Q{questionIndex} | Correct={isCorrect}");
        }
    }
}
