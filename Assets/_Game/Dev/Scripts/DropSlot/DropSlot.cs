using UnityEngine;
using UnityEngine.UI;

namespace Eduzo.Games.PlaceValue
{
    public class DropSlot : MonoBehaviour
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
        // 🔥 PARTICLE EFFECTS (NEW)
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
        // TILE DROP LOGIC (UNCHANGED)
        // ----------------------------------------------------

        public bool AcceptTile(TileDrag tile)
        {
            if (isFilled) return false;

            if (ExpectedValue < 0)
            {
                PlayGlow(new Color(1f, 0.2f, 0.2f));
                PlayParticle(wrongParticle);
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

            // ------------------------------------------------
            // CORRECT PLACEMENT
            // ------------------------------------------------
            if (valueMatches && indexMatches)
            {
                isFilled = true;

                tile.transform.SetParent(transform);

                RectTransform r = tile.GetComponent<RectTransform>();
                NormalizeRect(r, new Vector2(70, 70));

                CanvasGroup cg = tile.GetComponent<CanvasGroup>();
                if (cg != null) cg.blocksRaycasts = false;

                // 🔥 PLAY CORRECT PARTICLE
                PlayParticle(correctParticle);

                AudioManager.Instance?.PlaySFX("correct");
                GameEvents.OnTileCorrect?.Invoke();
                return true;
            }

            // ------------------------------------------------
            // WRONG PLACEMENT
            // ------------------------------------------------
            PlayParticle(wrongParticle);
            AudioManager.Instance?.PlaySFX("wrong");
            GameEvents.OnTileWrong?.Invoke();
            return false;
        }

        // ----------------------------------------------------
        // CONFIGURABLE GLOW (LEAN TWEEN)
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
        // 🔥 PARTICLE PLAY (NEW)
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
