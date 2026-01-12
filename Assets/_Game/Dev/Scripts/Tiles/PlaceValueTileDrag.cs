using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueTileDrag : MonoBehaviour,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public int value;        // digit value (0–9)
        public int digitIndex;   // place index (0–4)

        public float dragScale = 1.15f;
        public float snapDuration = 0.15f;
        public float returnDuration = 0.2f;

        private Canvas canvas;
        private CanvasGroup cg;
        private RectTransform rect;

        private Transform originalParent;
        private Vector2 originalPosition;
        private Vector3 originalScale;

        void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
            cg = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>();
            originalScale = transform.localScale;
        }

        // ------------------------------------------------
        // SET DIGIT (ONLY PLACE DIGITS ARE SET)
        // ------------------------------------------------
        public void SetDigit(int v, int index)
        {
            value = v;
            digitIndex = index;

            TMP_Text txt = GetComponentInChildren<TMP_Text>();
            txt.text = v.ToString();

            Debug.Log(
                $"SET DIGIT → Tile:{gameObject.name} | Value:{v} | Index:{index} | TextNow:{txt.text}"
            );
        }


        // ============================================
        // DRAG START
        // ============================================
        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;
            originalPosition = rect.anchoredPosition;

            cg.blocksRaycasts = false;
            cg.interactable = false;
            cg.ignoreParentGroups = true;

            PlaceValueAudioManager.Instance.PlaySFX("tile");

            LeanTween.scale(gameObject, originalScale * dragScale, 0.15f);
        }

        // ============================================
        // DRAGGING
        // ============================================
        public void OnDrag(PointerEventData eventData)
        {
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;

            // ✅ SAFE TUTORIAL PREVIEW (BY PLACE INDEX)
            PlaceValueSlotManager.Instance.PreviewTutorialByIndex(digitIndex);
        }

        // ============================================
        // DRAG END
        // ============================================
        public void OnEndDrag(PointerEventData eventData)
        {
            cg.blocksRaycasts = true;
            cg.interactable = true;
            cg.ignoreParentGroups = false;

            PlaceValueSlotManager.Instance.ClearPreview();

            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, results);

            PlaceValueDropSlot slot = null;

            foreach (var r in results)
            {
                slot = r.gameObject.GetComponent<PlaceValueDropSlot>() ??
                       r.gameObject.GetComponentInParent<PlaceValueDropSlot>();

                if (slot != null)
                    break;
            }

            if (slot == null || !slot.AcceptTile(this))
            {
                StartCoroutine(ReturnAnim());
                return;
            }

            SnapIntoSlot(slot.transform);
        }

        // ============================================
        private void SnapIntoSlot(Transform targetSlot)
        {
            transform.SetParent(targetSlot);

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            rect.anchoredPosition = Vector2.zero;
        }

        // ============================================
        private IEnumerator ReturnAnim()
        {
            transform.SetParent(originalParent);

            LeanTween.move(rect, originalPosition, returnDuration).setEaseOutBack();
            LeanTween.scale(gameObject, originalScale, 0.15f);

            yield return null;
        }
    }
}
