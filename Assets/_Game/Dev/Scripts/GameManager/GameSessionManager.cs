using UnityEngine;
using System;
using System.Collections.Generic;

namespace Eduzo.Games.PlaceValue
{
    public class GameSessionManager : MonoBehaviour
    {
        public static GameSessionManager Instance;

        public SessionDataSO sessionData;

        public string studentName = "Student";
        public GameMode currentMode;

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

        public void StartSession(GameMode mode, List<int> customList = null)
        {
            currentMode = mode;
            sessionId = Guid.NewGuid().ToString();

            sessionData.Clear();

            if (customList != null)
                customQuestions = customList;
            else
                customQuestions.Clear();

            Debug.Log("SESSION STARTED: " + sessionId);
        }

        public void LogQuestion(
            int questionIndex,
            int targetNumber,
            bool isCorrect,
            int wrongAttempts,
            float timeTaken
        )
        {
            QuestionLog log = new QuestionLog
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
