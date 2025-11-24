using UnityEngine;
using UnityEngine.UI;

public class DropSlot : MonoBehaviour
{
    // The digit this slot expects (0-9). Set via SlotManager.SetExpectedValue(...)
    public int ExpectedValue { get; private set; } = -999;

    // The digit position index (0..n-1) relative to the current question's digits.
    public int slotIndex = -1;

    private bool isFilled = false;

    // Outline component used for glow preview / pulse
    private Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false;
            outline.effectDistance = Vector2.zero;
        }
    }

    // Called by SlotManager to set which digit this slot expects
    public void SetExpectedValue(int v)
    {
        ExpectedValue = v;
        isFilled = false;
        ClearTutorialGlow();
    }

    // Preview glow used during tutorial while dragging (instant, no LeanTween pulse)
    public void ShowTutorialGlow()
    {
        if (outline == null) return;
        outline.enabled = true;
        outline.effectColor = Color.green;
        outline.effectDistance = new Vector2(10f, 10f); // strong tutorial glow
    }

    public void ClearTutorialGlow()
    {
        if (outline == null) return;
        outline.enabled = false;
        outline.effectDistance = Vector2.zero;
    }

    // AcceptTile checks BOTH the tile.value and the tile.digitIndex (slotIndex)
    public bool AcceptTile(TileDrag tile)
    {
        if (isFilled) return false;

        // --- If slot is unassigned -> treat as WRONG (important fix) ---
        if (ExpectedValue < 0)
        {
            // visual feedback + sfx + health event
            PlayGlow(new Color(1f, 0.2f, 0.2f));
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("wrong");
            GameEvents.OnTileWrong?.Invoke();
            return false;
        }

        bool valueMatches = tile.value == ExpectedValue;
        bool indexMatches = tile.digitIndex == slotIndex;

        // Play preview/pulse immediately for feedback (green if both match, red otherwise)
        if (valueMatches && indexMatches)
            PlayGlow(Color.green);
        else
            PlayGlow(new Color(1f, 0.2f, 0.2f));

        // Accept only if both match
        if (valueMatches && indexMatches)
        {
            isFilled = true;

            // snap tile into this slot
            tile.transform.SetParent(transform);

            RectTransform r = tile.GetComponent<RectTransform>();
            NormalizeRect(r, new Vector2(70, 70));

            // disable dragging of this tile
            var cg = tile.GetComponent<CanvasGroup>();
            if (cg != null) cg.blocksRaycasts = false;

            // play sfx and notify
            if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("correct");
            GameEvents.OnTileCorrect?.Invoke();

            return true;
        }

        // wrong placement
        if (AudioManager.Instance != null) AudioManager.Instance.PlaySFX("wrong");
        GameEvents.OnTileWrong?.Invoke();
        return false;
    }

    // Pulse glow animation (LeanTween)
    private void PlayGlow(Color c)
    {
        if (outline == null) return;

        outline.enabled = true;
        outline.effectColor = c;

        // animate outline effectDistance outward then back
        LeanTween.value(gameObject, 0f, 1f, 0.20f)
            .setOnUpdate((float v) =>
            {
                outline.effectDistance = new Vector2(10f * v, 10f * v);
            })
            .setOnComplete(() =>
            {
                LeanTween.value(gameObject, 1f, 0f, 0.20f)
                    .setOnUpdate((float v2) =>
                    {
                        outline.effectDistance = new Vector2(10f * v2, 10f * v2);
                    })
                    .setOnComplete(() =>
                    {
                        outline.enabled = false;
                        outline.effectDistance = Vector2.zero;
                    });
            });
    }

    // Make sure tile's RectTransform fits and centers inside the slot
    private void NormalizeRect(RectTransform r, Vector2 size)
    {
        if (r == null) return;

        r.localScale = Vector3.one;
        r.anchorMin = new Vector2(0.5f, 0.5f);
        r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = size;
        r.anchoredPosition = Vector2.zero;
    }
}
