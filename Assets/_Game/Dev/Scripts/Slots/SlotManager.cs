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
        // ORDER IS VERY IMPORTANT (left → right)
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
        // Disable all slots before activating required ones
        slotTenThousands.SetActive(false);
        slotThousands.SetActive(false);
        slotHundreds.SetActive(false);
        slotTens.SetActive(false);
        slotOnes.SetActive(false);

        int len = digits.Length;

        // RIGHT ALIGN DIGITS IN SLOTS
        // Example:
        // digits = {4, 3} (len=2)
        // startSlotID = 5 - 2 = 3
        // slot 3 = Tens → 4
        // slot 4 = Ones → 3
        int startSlotID = 5 - len;

        for (int i = 0; i < len; i++)
        {
            int slotID = startSlotID + i; // correct mapping for your UI

            GameObject slotObj = GetSlotObject(slotID);
            slotObj.SetActive(true);

            slotScripts[slotID].SetExpectedValue(digits[i]);
        }
    }

    // Helper to get slot by index
    private GameObject GetSlotObject(int id)
    {
        switch (id)
        {
            case 0: return slotTenThousands;
            case 1: return slotThousands;
            case 2: return slotHundreds;
            case 3: return slotTens;
            case 4: return slotOnes;
        }
        return null;
    }
}
