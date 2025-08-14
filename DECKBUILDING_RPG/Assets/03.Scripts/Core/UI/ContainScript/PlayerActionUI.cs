using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActionUI : MonoBehaviour
{
    [SerializeField]
    public Button moveButton;
    [SerializeField]
    public Button attackButton;
    [SerializeField]
    public Button cardButton;
    [SerializeField]
    public Button interactButton;
    public StageManager stageManager;
    public Action onActionButtonClicked;
    public void Init(Action actionButtonClickedAction)
    {
        this.onActionButtonClicked = actionButtonClickedAction;
        stageManager = GameManager.Instance.campaignManager.stageManager;

        moveButton.onClick.AddListener(OnMoveButtonClicked);
        attackButton.onClick.AddListener(OnAttackButtonClicked);
        cardButton.onClick.AddListener(OnUseCardButtonClicked);
        interactButton.onClick.AddListener(OnInteractButtonClicked);
    }

    private void OnMoveButtonClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        stageManager.ReadyToMove();
        onActionButtonClicked?.Invoke();
    }

    private void OnAttackButtonClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        stageManager.ReadyToAttack();
        onActionButtonClicked?.Invoke();
    }

    private void OnUseCardButtonClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        stageManager.ReadyToUseCard();
        onActionButtonClicked?.Invoke();
    }

    private void OnInteractButtonClicked()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-010");
        stageManager.ReadyToInteract();
        onActionButtonClicked?.Invoke();
    }
}