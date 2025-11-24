using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TileDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value;
    public int digitIndex; 

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

    public void SetValue(int v)
    {
        value = v;
        GetComponentInChildren<TMP_Text>().text = v.ToString();
    }

    public void SetDigit(int v, int index)
    {
        value = v;
        digitIndex = index;
        GetComponentInChildren<TMP_Text>().text = v.ToString();
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

        AudioManager.Instance.PlaySFX("tile");

        LeanTween.scale(gameObject, originalScale * dragScale, 0.15f);
    }

    // ============================================
    // DRAGGING
    // ============================================
    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;

        // 🔥 TUTORIAL PREVIEW (highlight correct slot while dragging)
        SlotManager.Instance.PreviewTutorial(value);
    }

    // ============================================
    // DRAG END
    // ============================================
    public void OnEndDrag(PointerEventData eventData)
    {
        cg.blocksRaycasts = true;
        cg.interactable = true;
        cg.ignoreParentGroups = false;

        // Stop tutorial preview
        SlotManager.Instance.ClearPreview();

        // 🔥 FIXED SLOT DETECTION — real world solution
        PointerEventData ped = new PointerEventData(EventSystem.current);
        ped.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(ped, results);

        DropSlot slot = null;

        foreach (var r in results)
        {
            slot = r.gameObject.GetComponent<DropSlot>() ??
                   r.gameObject.GetComponentInParent<DropSlot>();

            if (slot != null)
                break;
        }

        // If no slot found → return tile
        if (slot == null)
        {
            StartCoroutine(ReturnAnim());
            return;
        }

        // Try placing tile
        if (!slot.AcceptTile(this))
        {
            StartCoroutine(ReturnAnim());
            return;
        }

        // Correct slot → snap in
        SnapIntoSlot(slot.transform);
    }


    // ============================================
    // SNAP ANIMATION
    // ============================================
    private void SnapIntoSlot(Transform targetSlot)
    {
        RectTransform slotRect = targetSlot.GetComponent<RectTransform>();

        transform.SetParent(targetSlot);

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = Vector2.zero;
    }

    // ============================================
    // RETURN ANIMATION
    // ============================================
    private IEnumerator ReturnAnim()
    {
        transform.SetParent(originalParent);

        LeanTween.move(rect, originalPosition, returnDuration).setEaseOutBack();
        LeanTween.scale(gameObject, originalScale, 0.15f);

        yield return null;
    }
}
