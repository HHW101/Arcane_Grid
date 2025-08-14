using StageEnum;
using System;
using UnityEngine;

[Serializable]
public class StageStoryManagerParam
{
    public StageStoryBookSO announceBookSO;
}

public class StageStoryManager : MonoBehaviour
{
    private GameContext gameContext;
    private StageStoryManagerParam stageStoryManagerParam;
    private StageStoryBook announceBook;
    private StageStoryProgressor stageStoryProgressor;

    public void Init(GameContext gameContext, StageStoryManagerParam param)
    {
        this.gameContext = gameContext;
        this.stageStoryManagerParam = param;
        this.announceBook = new StageStoryBook(stageStoryManagerParam.announceBookSO);
        this.stageStoryProgressor = new StageStoryProgressor(gameContext);

    }

    public StageStorySO GetStageStorySO(StageType stageType)
    {
        return announceBook.Get(stageType);
    }

    public bool TryGetStageStorySO(StageType stageType, out StageStorySO announceSO)
    {
        return announceBook.TryGet(stageType, out announceSO);
    }

    public void SelectStrategyForCurStageType()
    {
        StageType curStageType = GameManager.Instance.campaignManager.stageManager.GetCurStageData().stageType;
        stageStoryProgressor.SelectStrategyForCurStageType((StageType)curStageType);
    }

    public void AnnounceCurProgress(Action onComplete)
    {
        stageStoryProgressor.AnnounceCurProgress(onComplete);
    }

    public bool CanEnterNextProgress()
    {
        int stageProgress = GameManager.Instance.campaignManager.stageManager.GetCurStageData().stageProgress;
        return stageStoryProgressor.CanEnterNextProgress(stageProgress);
    }

    public void EnterNextProgress(Action onComplete)
    {
        int stageProgress = GameManager.Instance.campaignManager.stageManager.GetCurStageData().stageProgress;
        stageStoryProgressor.EnterNextProgress(stageProgress, onComplete);
    }
}
