using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageEscapeUI : MonoBehaviour
{
    [SerializeField] private Button escapeButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private string nextScene;

    private APlayer player;
    private Action onComplete;

    private void Start()
    {
        escapeButton?.onClick.AddListener(OnClickEscapeButton);
        closeButton?.onClick.AddListener(OnClickCloseButton);
    }
    public void Enable(Action onComplete, APlayer player)
    {
        player.cardController.ResetDeck();
        GameManager.Instance.gameContext.saveData.isStarted = false;
        this.gameObject.SetActive(true);
        this.onComplete = onComplete;
        this.player = player;
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
        this.onComplete?.Invoke();
        this.onComplete = null;
    }

    private void OnClickEscapeButton()
    {
        StageManager stageManager = GameManager.Instance.campaignManager.stageManager;
        stageManager.ForceClearCurStage();
        SceneManager.LoadScene(nextScene);
        player?.SetMoveInterrupt(true);
        player?.RecoverWhenEscapeStage();
        Disable();
    }

    private void OnClickCloseButton()
    {
        Disable();
    }
}
