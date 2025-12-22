using System;

namespace Eduzo.Games.PlaceValue
{
    public static class GameEvents
    {
        public static Action OnTileCorrect;
        public static Action OnTileWrong;
        public static Action OnCorrectPlacement;

        public static Action<int> OnHealthChanged;
        public static Action OnGameOver;

        public static Action<int> OnTimerTick;

        public static Action<int> OnScoreUpdated;
        public static Action OnScoreFinal;
    }
}
