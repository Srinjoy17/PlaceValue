using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class TileDrag : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int value;

    public float dragScale = 1.15f;       // scale while dragging
    public float snapDuration = 0.15f;    // snap animation
    public float returnDuration = 0.2f;   // bounce return

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

    // ============================================
    // DRAG START
    // ============================================
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = rect.anchoredPosition;

        // IMPORTANT: disable raycasts so slot can be detected
        cg.blocksRaycasts = false;
        cg.interactable = false;
        cg.ignoreParentGroups = true;

        // scale up on drag
        LeanTween.scale(gameObject, originalScale * dragScale, 0.15f);
    }

    // ============================================
    // DRAGGING (pixel perfect follow)
    // ============================================
    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;   // PERFECT for ScreenSpace Overlay
    }

    // ============================================
    // DRAG END
    // ============================================
    public void OnEndDrag(PointerEventData eventData)
    {
        // re-enable for next drag
        cg.blocksRaycasts = true;
        cg.interactable = true;
        cg.ignoreParentGroups = false;

        // detect slot under mouse
        GameObject go = eventData.pointerCurrentRaycast.gameObject;
        DropSlot slot = go?.GetComponent<DropSlot>() ??
                        go?.GetComponentInParent<DropSlot>();

        if (slot == null)
        {
            // nothing under mouse ? return
            StartCoroutine(ReturnAnim());
            return;
        }

        // valid slot but wrong value ? return
        if (!slot.AcceptTile(this))
        {
            StartCoroutine(ReturnAnim());
            return;
        }

        // correct ? snap into slot
        SnapIntoSlot(slot.transform);
    }

    // ============================================
    // SNAP ANIMATION
    // ============================================
    private void SnapIntoSlot(Transform targetSlot)
    {
        // IMPORTANT: snap to the slot RECT, not the slot parent
        RectTransform slotRect = targetSlot.GetComponent<RectTransform>();

        transform.SetParent(targetSlot);

        // THIS LINE FIXES THE SNAP IN MIDDLE ISSUE.
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = Vector2.zero;
    }


    // ============================================
    // RETURN ANIMATION FOR WRONG DROP
    // ============================================
    private IEnumerator ReturnAnim()
    {
        transform.SetParent(originalParent);

        LeanTween.move(rect, originalPosition, returnDuration).setEaseOutBack();
        LeanTween.scale(gameObject, originalScale, 0.15f);

        yield return null;
    }
}
