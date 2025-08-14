using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    [SerializeField]
    private TMP_Text dialogText;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Image leftDialogSprite;
    [SerializeField]
    private Image rightDialogSprite;

    [SerializeField]
    private Color shadeColor = new Color32(100, 100, 100, 255);

    private Coroutine showRoutine;
    private Action onCompleteCallback;

    private EnumAssociatedResourceManager dialogSpriteManager;
    private DialogResourceSO currentDialogSO;
    private float waitTime;
    private bool isNextButtonActive;
    private int curIndex = 0;

    private bool isWaitingForClick = false;

    public void Start()
    {
        UIManager uiManager = GameManager.Instance.uiManager;
        uiManager.RegisterDialogUI(this);
        dialogSpriteManager = GameManager.Instance.enumAssociatedResourceManager;
        panel?.SetActive(false);
        nextButton?.onClick.AddListener(NextDialog);
    }

    public void Init()
    {
        panel?.SetActive(false);
    }

    public void Show(DialogResourceSO dialogResourceSO, bool nextButtonActive, Action onComplete = null)
    {
        if (dialogResourceSO == null || dialogResourceSO.dialogEntryList.Count == 0)
        {
            Debug.LogWarning("[DialogUI] dialogEntryList is empty.");
            return;
        }

        this.currentDialogSO = dialogResourceSO;
        this.onCompleteCallback = onComplete;
        this.isNextButtonActive = nextButtonActive;
        this.waitTime = dialogResourceSO.waitBetweenDialogSecond;
        this.curIndex = 0;

        panel?.SetActive(true);
        nextButton?.gameObject.SetActive(nextButtonActive);

        if (showRoutine != null)
            StopCoroutine(showRoutine);
        showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        while (curIndex < currentDialogSO.dialogEntryList.Count)
        {
            var entry = currentDialogSO.dialogEntryList[curIndex];

            leftDialogSprite.sprite = dialogSpriteManager.GetDialogSprite(entry.leftDialogSpriteType);
            rightDialogSprite.sprite = dialogSpriteManager.GetDialogSprite(entry.rightDialogSpriteType);
            leftDialogSprite.rectTransform.localScale = new Vector3(entry.leftFlip ? -1 : 1, 1, 1);
            rightDialogSprite.rectTransform.localScale = new Vector3(entry.rightFlip ? -1 : 1, 1, 1);
            dialogText.SetText(entry.dialogText);

            // ✅ Shade 적용
            leftDialogSprite.color = entry.leftShade ? shadeColor : Color.white;
            rightDialogSprite.color = entry.rightShade ? shadeColor : Color.white;

            isWaitingForClick = true;
            float timer = 0f;

            while (isWaitingForClick && timer < waitTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            isWaitingForClick = false;
            curIndex++;
        }

        Hide();
    }

    public void NextDialog()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008"); //효과음

        if (!isNextButtonActive || !isWaitingForClick) return;

        isWaitingForClick = false;
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
