using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PlaceValue/Session Data", fileName = "SessionData")]
public class SessionDataSO : ScriptableObject
{
    public List<QuestionLog> logs = new List<QuestionLog>();

    public void Clear()
    {
        logs.Clear();
    }
}
