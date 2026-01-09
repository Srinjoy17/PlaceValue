using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Eduzo.Games.PlaceValue
{
    public class PlaceValueCustomNumberUI : MonoBehaviour
    {
        public GameObject panel;

        [Header("Assign exactly 5 InputFields (order matters)")]
        public List<TMP_InputField> inputFields;

        private int activeIndex = 0;

        void Awake()
        {
            panel.SetActive(false);
        }

        void OnEnable()
        {
            panel.SetActive(false);
        }

        // --------------------------------------------------
        // OPEN FROM PRACTICE / TEST
        // --------------------------------------------------
        public void Open()
        {
            panel.SetActive(true);

            for (int i = 0; i < inputFields.Count; i++)
            {
                inputFields[i].text = "";
                inputFields[i].transform.parent.gameObject.SetActive(false);
            }

            activeIndex = 0;
            inputFields[0].transform.parent.gameObject.SetActive(true);

            EventSystem.current.SetSelectedGameObject(inputFields[0].gameObject);
            inputFields[0].ActivateInputField();
        }

        // --------------------------------------------------
        // SPACE → NEXT INPUT
        // --------------------------------------------------
        void Update()
        {
            if (!panel.activeSelf) return;

            if (Input.GetKeyDown(KeyCode.Space))
                ActivateNextInput();
        }

        void ActivateNextInput()
        {
            if (activeIndex >= inputFields.Count - 1)
                return;

            activeIndex++;

            GameObject row = inputFields[activeIndex].transform.parent.gameObject;
            row.SetActive(true);

            Canvas.ForceUpdateCanvases();

            EventSystem.current.SetSelectedGameObject(inputFields[activeIndex].gameObject);
            inputFields[activeIndex].ActivateInputField();
        }

        // --------------------------------------------------
        // OK BUTTON  (FINAL, CORRECT)
        // --------------------------------------------------
        public void Submit()
        {
            List<int> customNumbers = new List<int>();

            for (int i = 0; i <= activeIndex; i++)
            {
                if (int.TryParse(inputFields[i].text, out int value))
                    customNumbers.Add(value);
            }

            if (customNumbers.Count == 0)
                return;

            // Start session with entered numbers
            PlaceValueGameSessionManager.Instance.StartSession(
                PlaceValueGameModeManager.CurrentMode,
                customNumbers
            );

            panel.SetActive(false);

            // Switch UI to gameplay
            PlaceValueUIFlowManager.Instance.ShowGameplay();

            // 🔥 IMPORTANT: explicitly start the game AFTER input
            PlaceValueGameManager gm = FindAnyObjectByType<PlaceValueGameManager>();
            if (gm != null)
                gm.StartGame();
        }


    }
}
