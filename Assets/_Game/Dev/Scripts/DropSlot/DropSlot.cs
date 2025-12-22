using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.PlaceValue
{
    public class DropSlot : MonoBehaviour
    {
        // ----------------------------------------------------
        // SLOT DATA
        // ----------------------------------------------------

        // The digit this slot expects (0–9)
        public int ExpectedValue { get; private set; } = -999;

        // Correct digit index (0..n-1)
        public int slotIndex = -1;

        private bool isFilled = false;

        // ----------------------------------------------------
        // GLOW COMPONENT
        // ----------------------------------------------------

        private Outline outline;

        [Header("Glow Settings")]
        [Tooltip("Overall glow intensity")]
        public float glowSize = 20f;

        [Tooltip("Glow expand animation duration")]
        public float glowExpandDuration = 0.18f;

        [Tooltip("Glow shrink animation duration")]
        public float glowShrinkDuration = 0.18f;

        [Tooltip("Stronger glow during tutorial preview")]
        public float tutorialGlowSize = 24f;

        // ----------------------------------------------------
        // UNITY
        // ----------------------------------------------------

        private void Awake()
        {
            outline = GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false;
                outline.effectDistance = Vector2.zero;
            }
        }

        // ----------------------------------------------------
        // SLOT SETUP
        // ----------------------------------------------------

        public void SetExpectedValue(int v)
        {
            ExpectedValue = v;
            isFilled = false;
            ClearTutorialGlow();
        }

        // ----------------------------------------------------
        // TUTORIAL GLOW (NO LEANTWEEN)
        // ----------------------------------------------------

        public void ShowTutorialGlow()
        {
            if (outline == null) return;

            outline.enabled = true;
            outline.effectColor = Color.green;
            outline.effectDistance = new Vector2(tutorialGlowSize, tutorialGlowSize);
        }

        public void ClearTutorialGlow()
        {
            if (outline == null) return;

            outline.enabled = false;
            outline.effectDistance = Vector2.zero;
        }

        // ----------------------------------------------------
        // TILE DROP LOGIC
        // ----------------------------------------------------

        public bool AcceptTile(TileDrag tile)
        {
            if (isFilled) return false;

            // Slot not assigned → wrong
            if (ExpectedValue < 0)
            {
                PlayGlow(new Color(1f, 0.2f, 0.2f));
                AudioManager.Instance?.PlaySFX("wrong");
                GameEvents.OnTileWrong?.Invoke();
                return false;
            }

            bool valueMatches = tile.value == ExpectedValue;
            bool indexMatches = tile.digitIndex == slotIndex;

            if (valueMatches && indexMatches)
                PlayGlow(Color.green);
            else
                PlayGlow(new Color(1f, 0.2f, 0.2f));

            // Correct placement
            if (valueMatches && indexMatches)
            {
                isFilled = true;

                tile.transform.SetParent(transform);

                RectTransform r = tile.GetComponent<RectTransform>();
                NormalizeRect(r, new Vector2(70, 70));

                CanvasGroup cg = tile.GetComponent<CanvasGroup>();
                if (cg != null) cg.blocksRaycasts = false;

                AudioManager.Instance?.PlaySFX("correct");
                GameEvents.OnTileCorrect?.Invoke();
                return true;
            }

            // Wrong placement
            AudioManager.Instance?.PlaySFX("wrong");
            GameEvents.OnTileWrong?.Invoke();
            return false;
        }

        // ----------------------------------------------------
        // CONFIGURABLE GLOW (LEANTWEEN)
        // ----------------------------------------------------

        private void PlayGlow(Color color)
        {
            if (outline == null) return;

            outline.enabled = true;
            outline.effectColor = color;

            LeanTween.value(gameObject, 0f, 1f, glowExpandDuration)
                .setOnUpdate((float v) =>
                {
                    outline.effectDistance = new Vector2(glowSize * v, glowSize * v);
                })
                .setOnComplete(() =>
                {
                    LeanTween.value(gameObject, 1f, 0f, glowShrinkDuration)
                        .setOnUpdate((float v2) =>
                        {
                            outline.effectDistance = new Vector2(glowSize * v2, glowSize * v2);
                        })
                        .setOnComplete(() =>
                        {
                            outline.enabled = false;
                            outline.effectDistance = Vector2.zero;
                        });
                });
        }

        // ----------------------------------------------------
        // RECT NORMALIZATION
        // ----------------------------------------------------

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
}
