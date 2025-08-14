using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PromptPanelManager : MonoBehaviour
{
    [Header("UI 참조")]
    public GameObject promptPanel;
    public Transform promptTextGroup;
    public GameObject promptTextPrefab;
    public Button closeButton;

    [Header("타이핑 연출 설정")]
    [TextArea(2, 5)]
    public List<string> promptLines;
    public float charInterval = 0.05f;

    private bool skipCurrentLine = false;
    private bool isTyping = false;
    private bool isPromptFinished = false;

    void Start()
    {
        // closeButton.onClick.AddListener(OnPanelClick);
        // closeButton.interactable = false;
        //
        // if (PlayerPrefs.GetInt("PromptPanelShown", 0) == 0)
        // {
             ShowPrompt();
        // }
        // else
        // {
        //     promptPanel.SetActive(false);
        // }
    }

    public void ShowPrompt()
    {
        promptPanel.SetActive(true);
        StartCoroutine(TypePromptRoutine());
    }

    private IEnumerator TypePromptRoutine()
    {
        for (int i = 0; i < promptLines.Count; i++)
        {
            TMP_Text lineText = Instantiate(promptTextPrefab, promptTextGroup).GetComponent<TMP_Text>();
            lineText.text = "";
            skipCurrentLine = false;
            isTyping = true;
            yield return StartCoroutine(TypeLine(promptLines[i], lineText));
            isTyping = false;
        }
        isPromptFinished = true;
        closeButton.interactable = true;
    }

    private IEnumerator TypeLine(string line, TMP_Text textUI)
    {
        int i = 0;
        textUI.text = "";

        while (i < line.Length)
        {
            if (skipCurrentLine)
            {
                textUI.text = line;
                yield break;
            }
            textUI.text += line[i];
            i++;
            yield return new WaitForSeconds(charInterval);
        }
    }

    void Update()
    {
        if (isTyping && Input.GetMouseButtonDown(0))
        {
            skipCurrentLine = true;
        }
    }

    public void OnPanelClick()
    {
        if (!isPromptFinished) return;

        PlayerPrefs.SetInt("PromptPanelShown", 1);
        promptPanel.SetActive(false);
    }
}
