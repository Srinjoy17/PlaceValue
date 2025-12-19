using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CustomNumberUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Assign exactly 5 InputFields (order matters)")]
    public List<TMP_InputField> inputFields;

    private int activeIndex = 0;
    private GameMode selectedMode;

    void Start()
    {
        panel.SetActive(false);

        // Disable all rows initially
        for (int i = 0; i < inputFields.Count; i++)
            inputFields[i].transform.parent.gameObject.SetActive(false);
    }

    // --------------------------------------------------
    // OPEN FROM PRACTICE / TEST BUTTON
    // --------------------------------------------------
    public void Open(GameMode mode)
    {
        selectedMode = mode;
        panel.SetActive(true);

        // Reset all fields
        for (int i = 0; i < inputFields.Count; i++)
        {
            inputFields[i].text = "";
            inputFields[i].transform.parent.gameObject.SetActive(false);
        }

        // Activate first input
        activeIndex = 0;
        inputFields[0].transform.parent.gameObject.SetActive(true);

        EventSystem.current.SetSelectedGameObject(inputFields[0].gameObject);
        inputFields[0].ActivateInputField();
    }

    // --------------------------------------------------
    // SPACE → ACTIVATE NEXT INPUT
    // --------------------------------------------------
    void Update()
    {
        if (!panel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ActivateNextInput();
        }
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

        Debug.Log("Activated Input Index: " + activeIndex);
    }

    // --------------------------------------------------
    // OK BUTTON
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
        {
            Debug.LogWarning("No custom numbers entered");
            return;
        }

        // 🔥 MOST IMPORTANT LINE
        GameModeManager.CurrentMode = selectedMode;

        // Start session
        GameSessionManager.Instance.StartSession(selectedMode, customNumbers);

        SceneManager.LoadScene("GameScene");
    }
}
