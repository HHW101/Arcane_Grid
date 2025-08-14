
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class UIManagerParam
{
}


public class UIManager : MonoBehaviour
{
    UIManagerParam uiManagerParam;

    private GameContext gameContext;

    public void Init(GameContext gameContext, UIManagerParam uiManagerParam)
    {
        this.uiManagerParam = uiManagerParam;
        this.gameContext = gameContext;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        HideUI();
    }

    public void OnSceneLoaded(Scene scene)
    {
        HideUI();
    }
    
    public void HideUI()
    {
        HideGetDetailInfo();
        HideHpUI();
    }

    public void RegisterAnnounceUI(AnnounceUI announceUI)
    {
        gameContext.announceUI = announceUI;
    }

    public void UnRegisterAnnounceUI()
    {
        gameContext.announceUI = null;
    }

    public void Announce(AnnounceResourceSO announceResourceSO = null, string pressText = null, Action onComplete = null)
    {
        if (gameContext.announceUI != null)
        {
            gameContext.announceUI.Show(announceResourceSO, pressText, onComplete);
        }
    }

    public bool IsAnnounceEnded()
    {
        return gameContext.isInputLocked == false;
    }

    public void RegisterTurnStartUI(TurnStartUI turnStartUI)
    {
        gameContext.turnStartUI = turnStartUI;
    }

    public void ShowPlayerTurnStart(Action onComplete = null)
    {
        gameContext.turnStartUI?.RegisterPlayerTurnStartAnimationEndCallback(onComplete);
        gameContext.turnStartUI?.PlayerTurnStartAnimationStart();
    }

    public void ShowNPCTurnStart(Action onComplete = null)
    {
        gameContext.turnStartUI?.RegisterNPCTurnStartAnimationEndCallback(onComplete);
        gameContext.turnStartUI?.NPCTurnStartAnimatonStart();
    }

    public void RegisterDialogUI(DialogUI dialogUI)
    {
        gameContext.dialogUI = dialogUI;
    }

    public void UnRegisterDialogUI()
    {
        gameContext.announceUI = null;
    }

    public void ShowDialog(DialogResourceSO dialogResourceSO = null, bool nextButtonActive = true, Action onComplete = null)
    {
        if (gameContext.dialogUI != null)
        {
            gameContext.dialogUI.Show(dialogResourceSO, nextButtonActive, onComplete);
        }
    }

    public void RegisterImageUI(ImageUI imageUI)
    {
        gameContext.imageUI = imageUI;
    }

    public void UnRegisterImageUI()
    {
        gameContext.imageUI = null;
    }

    public void ShowImage(ImageResourceSO imageResourceSO = null, bool closeButtonActive = true, Action onComplete = null)
    {
        if (gameContext.imageUI != null)
        {
            gameContext.imageUI.Show(imageResourceSO, closeButtonActive, onComplete);
        }
    }

    public void RegisterInfoUI(InfoUI infoUI)
    {
        gameContext.infoUI = infoUI;
    }

    public void UnRegisterInfoUI()
    {
        gameContext.infoUI = null;
    }

    public void ShowInfo(IShowInfoable showInfoable, bool closeButtonActive = true, Action onComplete = null)
    {
        if (gameContext.infoUI != null)
        {
            gameContext.infoUI.Show(showInfoable, 999f, closeButtonActive, onComplete);
        }
    }

    public KeyCode GetShowInfoKeyCode()
    {
        if (gameContext.infoUI != null)
        {
            return gameContext.infoUI.keyCode;
        }
        return default;
    }

    public void RegisterDetailInfoUI(DetailInfoUI detailInfoUI)
    {
        gameContext.detailInfoUI = detailInfoUI;
    }

    public void UnRegisterDetailInfoUI()
    {
        gameContext.detailInfoUI = null;
    }

    public void ShowDetailInfo(IShowDetailInfoable showDetailInfoable)
    {
        if (gameContext.detailInfoUI != null)
        {
            gameContext.detailInfoUI.Show(showDetailInfoable);
        }
    }

    public void ShowGetDetailInfo()
    {
        if (gameContext.detailInfoUI != null)
        {
            gameContext.detailInfoUI.ShowGetDetailInfo();
        }
        else
        {
            Logger.Log("detailInfoUI null");
        }
    }

    public bool IsHideGetDetailInfo()
    {
        if (gameContext.detailInfoUI != null)
        {
            return gameContext.detailInfoUI.IsHide();
        }
        return false;
    }

    public void HideGetDetailInfo()
    {
        if (gameContext.detailInfoUI != null)
        {
            gameContext.detailInfoUI.HideGetDetailInfo();
        }
    }

    public void RegisterHpUI(HpUI hpUI)
    {
        gameContext.hpUI = hpUI;
    }

    public void UnRegisterHpUI()
    {
        gameContext.hpUI = null;
    }

    public void ShowHpUI(UnitData unitData)
    {
        if(gameContext.hpUI != null)
        {
            gameContext.hpUI.Show(unitData);
        }
    }

    public void HideHpUI()
    {
        if(gameContext.hpUI != null)
        {
            gameContext.hpUI.Hide();
        }
    }
}