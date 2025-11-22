using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public GameObject slotTenThousands; // index 0
    public GameObject slotThousands;    // index 1
    public GameObject slotHundreds;     // index 2
    public GameObject slotTens;         // index 3
    public GameObject slotOnes;         // index 4

    private DropSlot[] slotScripts;

    void Awake()
    {
        slotScripts = new DropSlot[]
        {
            slotTenThousands.GetComponent<DropSlot>(), // 0
            slotThousands.GetComponent<DropSlot>(),    // 1
            slotHundreds.GetComponent<DropSlot>(),     // 2
            slotTens.GetComponent<DropSlot>(),         // 3
            slotOnes.GetComponent<DropSlot>()          // 4
        };
    }

    public void SetupSlots(int[] digits)
    {
        // DO NOT disable slots anymore!
        // All slots are always visible.

        int len = digits.Length;

        // RIGHT ALIGN like before
        int startSlotID = 5 - len;

        // First reset ALL expected values (so old values won't interfere)
        for (int i = 0; i < 5; i++)
        {
            slotScripts[i].SetExpectedValue(-999); // impossible value
        }

        // Now correctly assign expected digits
        for (int i = 0; i < len; i++)
        {
            int slotID = startSlotID + i;
            slotScripts[slotID].SetExpectedValue(digits[i]);
        }
    }
}
