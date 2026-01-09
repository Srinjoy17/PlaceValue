using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueTileManager : MonoBehaviour
    {
        public Transform tileStartParent;

        public PlaceValueTileDrag tileTenThousands;
        public PlaceValueTileDrag tileThousands;
        public PlaceValueTileDrag tileHundreds;
        public PlaceValueTileDrag tileTens;
        public PlaceValueTileDrag tileOnes;

        private PlaceValueTileDrag[] tiles;

        private Vector2[] originalPositions;

        void Awake()
        {
            tiles = new PlaceValueTileDrag[]
            {
                tileTenThousands, // 0
                tileThousands,    // 1
                tileHundreds,     // 2
                tileTens,         // 3
                tileOnes          // 4
            };

            originalPositions = new Vector2[tiles.Length];

            for (int i = 0; i < tiles.Length; i++)
            {
                originalPositions[i] =
                    tiles[i].GetComponent<RectTransform>().anchoredPosition;
            }
        }

        // ----------------------------------------------------
        public void ResetTiles()
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                var tile = tiles[i];
                var rect = tile.GetComponent<RectTransform>();

                tile.gameObject.SetActive(false);
                tile.transform.SetParent(tileStartParent);

                rect.anchoredPosition = originalPositions[i];
                rect.localScale = Vector3.one;

                var cg = tile.GetComponent<CanvasGroup>();
                cg.blocksRaycasts = true;
                cg.interactable = true;
            }
        }

        // ----------------------------------------------------
        public void SetupTiles(int[] digits)
        {
            ResetTiles();

            int len = digits.Length;
            int start = tiles.Length - len;

            for (int i = 0; i < len; i++)
            {
                int placeIndex = start + i;
                var tile = tiles[placeIndex];

                tile.gameObject.SetActive(true);
                tile.SetDigit(digits[i], placeIndex);
            }
        }
    }
}
