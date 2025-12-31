using System;

[Serializable]
public class PlaceValueQuestionLog
{
    public string studentName;
    public string sessionId;
    public PlaceValueGameMode mode;

    public int questionIndex;     // 1..N in this session
    public int targetNumber;      // e.g. 482, 37, etc
    public bool isCorrect;        // usually true when question is finished
    public int wrongAttempts;     // how many wrong drops before finishing
    public float timeTaken;       // seconds for this question

    public string timestamp;      // stored as string for inspector readability
}