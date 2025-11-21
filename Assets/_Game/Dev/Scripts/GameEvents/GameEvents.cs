using System;

public static class GameEvents
{
    public static Action OnTileCorrect;
    public static Action OnTileWrong;

    public static Action<int> OnHealthChanged;
    public static Action OnGameOver;

    public static Action<int> OnTimerTick;
}
