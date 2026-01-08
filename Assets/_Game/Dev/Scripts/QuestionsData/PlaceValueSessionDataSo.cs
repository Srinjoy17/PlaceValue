using System.Collections.Generic;
using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    [CreateAssetMenu(menuName = "PlaceValue/Session Data", fileName = "SessionData")]
    public class SessionDataSO : ScriptableObject
    {
        public List<PlaceValueQuestionLog> logs = new List<PlaceValueQuestionLog>();

        public void Clear()
        {
            logs.Clear();
        }
    }
}
