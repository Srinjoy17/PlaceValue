using UnityEngine;

public class TileManager : MonoBehaviour
{
    // Parent panel where all tiles return before each question
    public Transform tileStartParent;

    // Drag tile references assigned in the Inspector
    public TileDrag tileGreen;
    public TileDrag tilePink;
    public TileDrag tileYellow;
    public TileDrag tileBlue;

    // Internal array for easy tile indexing (1–4)
    private TileDrag[] tileSlots;

    // Stores each tile's original anchored position for perfect resetting
    private Vector2[] originalPositions;

    void Awake()
    {
        // Create an indexed list of all tiles (0th index kept null intentionally)
        tileSlots = new TileDrag[]
        {
            null,        // Index 0 unused (makes indexing match your logic)
            tileGreen,   // Index 1
            tilePink,    // Index 2
            tileYellow,  // Index 3
            tileBlue     // Index 4
        };

        // Store the starting anchored positions of each tile
        originalPositions = new Vector2[5];

        for (int i = 0; i < tileSlots.Length; i++)
        {
            if (tileSlots[i] != null)
            {
                // Save each tile's original anchored UI position
                originalPositions[i] = tileSlots[i]
                    .GetComponent<RectTransform>()
                    .anchoredPosition;
            }
        }
    }

    // =====================================================
    // RESET ALL TILES TO THEIR ORIGINAL STATE & POSITION
    // Called before every question to ensure clean gameplay
    // =====================================================
    public void ResetTiles()
    {
        for (int i = 0; i < tileSlots.Length; i++)
        {
            TileDrag tile = tileSlots[i];
            if (tile == null) continue;

            RectTransform rect = tile.GetComponent<RectTransform>();

            // Reset tile parent so it goes back to the lower panel
            tile.transform.SetParent(tileStartParent);

            // Reset anchored position to its original saved value
            rect.anchoredPosition = originalPositions[i];

            // Reset scale to default
            rect.localScale = Vector3.one;

            // Reset size (ensure consistent tile dimensions)
            rect.sizeDelta = new Vector2(70, 70);

            // Restore raycast ability so tiles can be dragged again
            CanvasGroup cg = tile.GetComponent<CanvasGroup>();
            cg.blocksRaycasts = true;
            cg.interactable = true;
        }
    }

    // =====================================================
    // SETUP TILES BASED ON THE NUMBER OF DIGITS IN QUESTION
    // Hides unused tiles and sets the visible tile values
    // =====================================================
    public void SetupTiles(int[] digits)
    {
        // Always reset tiles before configuring a new question
        ResetTiles();

        int len = digits.Length;

        // Hide all tiles initially
        tileGreen.gameObject.SetActive(false);
        tilePink.gameObject.SetActive(false);
        tileYellow.gameObject.SetActive(false);
        tileBlue.gameObject.SetActive(false);

        // Align tiles properly based on how many digits exist
        int startIndex = 5 - len;

        // Activate required tiles and assign digits
        for (int i = 0; i < len; i++)
        {
            int tileIndex = startIndex + i;
            TileDrag tile = tileSlots[tileIndex];
            if (tile == null) continue;

            tile.gameObject.SetActive(true);

            // Set the number displayed + index for slot validation
            tile.SetDigit(digits[i], i);
        }
    }
}
