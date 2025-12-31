using System;

namespace Eduzo.Games.PlaceValue
{
    public static class PlaceValueGameEvents
    {
        public static Action OnPlaceValueTileCorrect;
        public static Action OnPlaceValueTileWrong;
        public static Action OnPlaceValueCorrectPlacement;

        public static Action<int> OnPlaceValueHealthChanged;
        public static Action OnPlaceValueGameOver;

        public static Action<int> OnPlaceValueTimerTick;

        public static Action<int> OnPlaceValueScoreUpdated;
        public static Action OnPlaceValueScoreFinal;
    }
}
