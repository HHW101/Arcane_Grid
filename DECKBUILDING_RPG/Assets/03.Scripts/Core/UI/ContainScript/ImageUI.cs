using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image image;
    [SerializeField] private Button closeButton;

    private Coroutine showRoutine;
    private Action onCompleteCallback;

    public void Start()
    {
        UIManager uiManager = GameManager.Instance.uiManager;
        uiManager.RegisterImageUI(this);
        panel?.SetActive(false);
        closeButton?.onClick.AddListener(OnCloseClicked);
    }

    public void Init()
    {
        panel?.SetActive(false);
    }

    public void Show(ImageResourceSO imageResourceSO, bool closeButtonActive = true, Action onComplete = null)
    {
        if (imageResourceSO == null || imageResourceSO.image == null)
        {
            Debug.LogWarning("[ImageUI] imageResourceSO is null or missing image.");
            return;
        }

        panel?.SetActive(true);
        image.sprite = imageResourceSO.image;
        closeButton?.gameObject.SetActive(closeButtonActive);
        onCompleteCallback = onComplete;

        if (showRoutine != null)
            StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine(imageResourceSO.waitBeforeHideSecond, closeButtonActive));
    }

    private IEnumerator ShowRoutine(float waitTime, bool isCloseButtonActive)
    {
        if (!isCloseButtonActive)
        {
            yield return new WaitForSeconds(waitTime);
            Hide();
        }
    }

    private void OnCloseClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008"); //효과음

        if (closeButton.gameObject.activeSelf)
        {
            Hide();
        }
    }

    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        panel?.SetActive(false);
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
    }
}
