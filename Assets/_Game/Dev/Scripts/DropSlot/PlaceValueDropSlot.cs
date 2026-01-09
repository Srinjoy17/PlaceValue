using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueDropSlot : MonoBehaviour
    {
        // ----------------------------------------------------
        // SLOT DATA
        // ----------------------------------------------------

        public int ExpectedValue { get; private set; } = -999;
        public int slotIndex = -1;

        private bool isFilled = false;

        // ----------------------------------------------------
        // GLOW COMPONENT
        // ----------------------------------------------------

        private Outline outline;

        [Header("Glow Settings")]
        public float glowSize = 20f;
        public float glowExpandDuration = 0.18f;
        public float glowShrinkDuration = 0.18f;
        public float tutorialGlowSize = 24f;

        // ----------------------------------------------------
        // PARTICLE EFFECTS
        // ----------------------------------------------------

        [Header("Particle Effects")]
        public ParticleSystem correctParticle;
        public ParticleSystem wrongParticle;

        [Tooltip("Local offset for particle spawn")]
        public Vector3 particleOffset = new Vector3(0f, 40f, 0f);

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
        // TUTORIAL GLOW
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

        public bool AcceptTile(PlaceValueTileDrag tile)
        {
            if (isFilled) return false;

            // Slot not active
            if (ExpectedValue < 0)
            {
                PlayGlow(Color.red);
                PlayParticle(wrongParticle);
                PlaceValueAudioManager.Instance?.PlaySFX("wrong");
                PlaceValueGameEvents.OnPlaceValueTileWrong?.Invoke();
                return false;
            }

            bool valueMatches = tile.value == ExpectedValue;
            bool indexMatches = tile.digitIndex == slotIndex;

            if (valueMatches && indexMatches)
                PlayGlow(Color.green);
            else
                PlayGlow(Color.red);

            // ------------------------------------------------
            // CORRECT PLACEMENT
            // ------------------------------------------------
            if (valueMatches && indexMatches)
            {
                isFilled = true;

                // IMPORTANT: false prevents scale inheritance
                tile.transform.SetParent(transform, false);

                RectTransform tileRect = tile.GetComponent<RectTransform>();
                RectTransform slotRect = GetComponent<RectTransform>();

                // Normalize tile transform
                tileRect.localScale = Vector3.one;
                tileRect.anchorMin = new Vector2(0.5f, 0.5f);
                tileRect.anchorMax = new Vector2(0.5f, 0.5f);
                tileRect.pivot = new Vector2(0.5f, 0.5f);
                tileRect.anchoredPosition = Vector2.zero;

                // 🔥 Match tile size to slot size
                tileRect.sizeDelta = slotRect.sizeDelta;

                CanvasGroup cg = tile.GetComponent<CanvasGroup>();
                if (cg != null)
                    cg.blocksRaycasts = false;

                PlayParticle(correctParticle);
                PlaceValueAudioManager.Instance?.PlaySFX("correct");
                PlaceValueGameEvents.OnPlaceValueTileCorrect?.Invoke();
                return true;
            }

            // ------------------------------------------------
            // WRONG PLACEMENT
            // ------------------------------------------------
            PlayParticle(wrongParticle);
            PlaceValueAudioManager.Instance?.PlaySFX("wrong");
            PlaceValueGameEvents.OnPlaceValueTileWrong?.Invoke();
            return false;
        }

        // ----------------------------------------------------
        // GLOW ANIMATION
        // ----------------------------------------------------

        private void PlayGlow(Color color)
        {
            if (outline == null) return;

            outline.enabled = true;
            outline.effectColor = color;

            LeanTween.value(gameObject, 0f, 1f, glowExpandDuration)
                .setOnUpdate(v =>
                {
                    outline.effectDistance = new Vector2(glowSize * v, glowSize * v);
                })
                .setOnComplete(() =>
                {
                    LeanTween.value(gameObject, 1f, 0f, glowShrinkDuration)
                        .setOnUpdate(v2 =>
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
        // PARTICLES
        // ----------------------------------------------------

        private void PlayParticle(ParticleSystem particle)
        {
            if (particle == null) return;

            ParticleSystem ps = Instantiate(
                particle,
                transform.position + particleOffset,
                Quaternion.identity,
                transform
            );

            ps.Play();

            float life = ps.main.duration + ps.main.startLifetime.constantMax;
            Destroy(ps.gameObject, life);
        }
    }
}
