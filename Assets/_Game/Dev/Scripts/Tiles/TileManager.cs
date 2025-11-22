using UnityEngine;

public class TileManager : MonoBehaviour
{
    public Transform tileStartParent;   // The LOWER tile holder panel
    public TileDrag tileGreen;
    public TileDrag tilePink;
    public TileDrag tileYellow;
    public TileDrag tileBlue;

    private TileDrag[] tileSlots;
    private Vector2[] originalPositions;

    void Awake()
    {
        tileSlots = new TileDrag[]
        {
            null,
            tileGreen,
            tilePink,
            tileYellow,
            tileBlue
        };

        // STORE ORIGINAL POSITIONS
        originalPositions = new Vector2[5];

        for (int i = 0; i < tileSlots.Length; i++)
        {
            if (tileSlots[i] != null)
                originalPositions[i] = tileSlots[i].GetComponent<RectTransform>().anchoredPosition;
        }
    }

    // ============================================
    // RESET TILES BEFORE EACH QUESTION
    // ============================================
    public void ResetTiles()
    {
        for (int i = 0; i < tileSlots.Length; i++)
        {
            TileDrag tile = tileSlots[i];
            if (tile == null) continue;

            RectTransform rect = tile.GetComponent<RectTransform>();

            //  Reset parent
            tile.transform.SetParent(tileStartParent);

            //  Reset anchored position
            rect.anchoredPosition = originalPositions[i];

            //  Reset scale
            rect.localScale = Vector3.one;

            //  Reset size
            rect.sizeDelta = new Vector2(70, 70); // <— match your tile size

            //  Reset anchors
            /*rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);

            //  Reset pivot
            rect.pivot = new Vector2(0.5f, 0.5f);*/

            //  Reactivate raycasts
            CanvasGroup cg = tile.GetComponent<CanvasGroup>();
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }


    // =============================
    // SETUP TILES FOR NEW QUESTION
   
    public void SetupTiles(int[] digits)
    {
        ResetTiles();  // <-- MOST IMPORTANT FIX

        int len = digits.Length;

        tileGreen.gameObject.SetActive(false);
        tilePink.gameObject.SetActive(false);
        tileYellow.gameObject.SetActive(false);
        tileBlue.gameObject.SetActive(false);

        int startIndex = 5 - len;

        for (int i = 0; i < len; i++)
        {
            int tileIndex = startIndex + i;
            TileDrag tile = tileSlots[tileIndex];

            if (tile == null) continue;

            tile.gameObject.SetActive(true);
            tile.SetValue(digits[i]);
        }
    }
}
