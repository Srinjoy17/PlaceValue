using UnityEngine;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueSlotManager : MonoBehaviour
    {
        public static PlaceValueSlotManager Instance;

        public bool tutorialMode = false;

        public GameObject slotTenThousands;
        public GameObject slotThousands;
        public GameObject slotHundreds;
        public GameObject slotTens;
        public GameObject slotOnes;

        private PlaceValueDropSlot[] slots;

        void Awake()
        {
            Instance = this;

            slots = new PlaceValueDropSlot[]
            {
                slotTenThousands.GetComponent<PlaceValueDropSlot>(),
                slotThousands.GetComponent<PlaceValueDropSlot>(),
                slotHundreds.GetComponent<PlaceValueDropSlot>(),
                slotTens.GetComponent<PlaceValueDropSlot>(),
                slotOnes.GetComponent<PlaceValueDropSlot>()
            };
        }

        // ----------------------------------------------------
        // SETUP SLOTS FOR CURRENT QUESTION
        // ----------------------------------------------------
        public void SetupSlots(int[] digits)
        {
            int len = digits.Length;
            int start = slots.Length - len;

            // Reset all slots
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].SetExpectedValue(-999);
                slots[i].slotIndex = -1;
            }

            // Assign expected values
            for (int i = 0; i < len; i++)
            {
                int slotID = start + i;

                slots[slotID].SetExpectedValue(digits[i]);
                slots[slotID].slotIndex = slotID; // place index
            }
        }

        // ----------------------------------------------------
        // 🔥 SAFE TUTORIAL PREVIEW (BY PLACE INDEX)
        // ----------------------------------------------------
        public void PreviewTutorialByIndex(int placeIndex)
        {
            if (!tutorialMode) return;

            for (int i = 0; i < slots.Length; i++)
            {
                if (i == placeIndex)
                    slots[i].ShowTutorialGlow();
                else
                    slots[i].ClearTutorialGlow();
            }
        }

        // ----------------------------------------------------
        // CLEAR PREVIEW
        // ----------------------------------------------------
        public void ClearPreview()
        {
            foreach (PlaceValueDropSlot s in slots)
                s.ClearTutorialGlow();
        }
    }
}
