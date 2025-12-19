using UnityEngine;

public class GameModeUIButton : MonoBehaviour
{
    public CustomNumberUI form;   // Drag from inspector

    public void PlayPractice()
    {
        form.Open(GameMode.Practice);
    }

    public void PlayTest()
    {
        form.Open(GameMode.Test);
    }
}
