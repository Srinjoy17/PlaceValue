using System;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class TestCSVExporter : MonoBehaviour
    {
        private string filePath;

        void Awake()
        {
#if UNITY_EDITOR
            string dir = Path.Combine(
                Directory.GetParent(Application.dataPath).FullName,
                "Game", "Dev", "Reports"
            );
#else
            string dir = Path.Combine(Application.persistentDataPath, "Reports");
#endif
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            filePath = Path.Combine(dir, "PlaceValue_Report.csv");

            if (!File.Exists(filePath))
            {
                SafeWriteLine(
                    "RunId,StudentName,Mode,TotalQues,Attempted,Correct,Wrong,Score,TimeTaken,Date"
                );
            }
        }

        public void ExportTestResult()
        {
            var logs = GameSessionManager.Instance.sessionData.logs;
            if (logs == null || logs.Count == 0) return;

            string runId = Guid.NewGuid().ToString();
            string student = GameSessionManager.Instance.studentName;
            string mode = GameSessionManager.Instance.currentMode.ToString();

            int attempted = logs.Count;
            int correct = logs.Count(l => l.isCorrect);
            int wrong = attempted - correct;
            int score = correct * 20;
            float time = logs.Sum(l => l.timeTaken);

            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string row =
                $"{runId},{student},{mode},{attempted},{attempted},{correct},{wrong},{score},{Mathf.RoundToInt(time)},{date}";

            SafeWriteLine(row);
        }

        // ---------------------------------------------------
        // 🔥 SAFE WRITE WITH RETRY (NO SHARING VIOLATION)
        // ---------------------------------------------------
        void SafeWriteLine(string line)
        {
            const int maxRetries = 5;
            const int delayMs = 100;

            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    using (FileStream fs = new FileStream(
                        filePath,
                        FileMode.Append,
                        FileAccess.Write,
                        FileShare.ReadWrite))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(line);
                        }
                    }
                    return; // ✅ success
                }
                catch (IOException)
                {
                    // File is locked → wait and retry
                    Thread.Sleep(delayMs);
                }
            }

            // If we reach here, file stayed locked
            Debug.LogWarning("CSV WRITE SKIPPED (file locked)");
        }
    }
}
