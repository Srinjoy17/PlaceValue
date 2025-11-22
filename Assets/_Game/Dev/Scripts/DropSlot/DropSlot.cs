using UnityEngine;

public class DropSlot : MonoBehaviour
{
    private int expectedValue;
    private bool isFilled = false;

    public void SetExpectedValue(int v)
    {
        expectedValue = v;
        isFilled = false;
    }

    public bool AcceptTile(TileDrag tile)
    {
        if (isFilled) return false;

        if (tile.value == expectedValue)
        {
            isFilled = true;

            tile.transform.SetParent(transform);

            RectTransform r = tile.GetComponent<RectTransform>();
            NormalizeTileRectTransform(r, new Vector2(70, 70));

            tile.GetComponent<CanvasGroup>().blocksRaycasts = false;

            // 🔥 Correct! No parameters
            GameEvents.OnTileCorrect?.Invoke();
            return true;
        }

        // 🔥 Correct! No parameters
        GameEvents.OnTileWrong?.Invoke();
        return false;
    }

    private void NormalizeTileRectTransform(RectTransform r, Vector2 size)
    {
        r.localScale = Vector3.one;
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = size;
        r.anchoredPosition = Vector2.zero;
    }
}
