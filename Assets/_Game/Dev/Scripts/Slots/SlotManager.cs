using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance;

    public bool tutorialMode = false;

    public GameObject slotTenThousands;
    public GameObject slotThousands;
    public GameObject slotHundreds;
    public GameObject slotTens;
    public GameObject slotOnes;

    private DropSlot[] slots;

    void Awake()
    {
        Instance = this;

        slots = new DropSlot[]
        {
            slotTenThousands.GetComponent<DropSlot>(),
            slotThousands.GetComponent<DropSlot>(),
            slotHundreds.GetComponent<DropSlot>(),
            slotTens.GetComponent<DropSlot>(),
            slotOnes.GetComponent<DropSlot>()
        };
    }

    public void SetupSlots(int[] digits)
    {
        int len = digits.Length;
        int start = 5 - len;

        // Reset all slots first
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetExpectedValue(-999);
            slots[i].slotIndex = -1;
        }

        // Assign expected digit + correct slotIndex
        for (int i = 0; i < len; i++)
        {
            int slotID = start + i;

            slots[slotID].SetExpectedValue(digits[i]);
            slots[slotID].slotIndex = i;  // <--- SUPER IMPORTANT FIX
        }
    }

    // Tutorial glow preview
    public void PreviewTutorial(int number)
    {
        if (!tutorialMode) return;

        foreach (DropSlot s in slots)
        {
            if (s.ExpectedValue == number)
                s.ShowTutorialGlow();
            else
                s.ClearTutorialGlow();
        }
    }

    public void ClearPreview()
    {
        foreach (DropSlot s in slots)
            s.ClearTutorialGlow();
    }
}
