using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionCancelUI : MonoBehaviour
{
    [SerializeField]
    public Button cancelButton;
    public StageManager stageManager;
    public Action onCancelButtonClicked;
    public void Init(Action cancelButtonClickedAction)
    {
        this.onCancelButtonClicked = cancelButtonClickedAction;
        stageManager = GameManager.Instance.campaignManager.stageManager;

        cancelButton.onClick.AddListener(OnCancelButtonClicked);
    }

    private void OnCancelButtonClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-080");
        stageManager.CancelAction();
        onCancelButtonClicked?.Invoke();
    }

    private void Update()
    {
    }
}