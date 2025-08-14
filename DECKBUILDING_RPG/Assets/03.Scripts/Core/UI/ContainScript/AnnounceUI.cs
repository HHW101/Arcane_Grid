using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnounceUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private TMP_Text goalText;

    [SerializeField]
    private TMP_Text pressText;

    [SerializeField]
    private string pressTextDefaultString = "Press Any Key";


    private Coroutine showRoutine;
    private Coroutine hideRoutine;
    private Action onCompleteCallback;

    public void Start()
    {
        UIManager uiManager = GameManager.Instance.uiManager;
        uiManager.RegisterAnnounceUI(this);
        panel?.SetActive(false);
    }

    public void Init()
    {
        panel?.SetActive(false);
    }

    public void Show(AnnounceResourceSO announceResourceSO, string pressText = "Press Any Key", Action onComplete = null)
    {
        if (announceResourceSO == null || announceResourceSO.announceStringList.Count == 0)
        {
            Debug.LogWarning("[AnnounceUI] goalTextList is empty.");
            return;
        }

        this.pressText?.SetText(pressText);
        panel?.SetActive(true);

        onCompleteCallback = onComplete;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ShowRoutine(announceResourceSO, announceResourceSO.waitBeforeHideSecond));
    }

    public void Hide()
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator ShowRoutine(AnnounceResourceSO announceResourceSO, float waitBeforeHideSecond)
    {
        List<string> announceStringList = announceResourceSO.announceStringList;
        for (int i = 0; i < announceStringList.Count; i++)
        {
            goalText?.SetText(announceStringList[i]);

            float timer = 0f;
            bool proceedNext = false;

            while (timer < waitBeforeHideSecond && !proceedNext)
            {
                if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    proceedNext = true;
                }
                else
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            if (i == announceStringList.Count - 1)
            {
                Hide();
                yield break;
            }
        }
    }

    private IEnumerator HideRoutine()
    {
        panel?.SetActive(false);
        yield return null;
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }
}
