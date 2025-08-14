using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AStageStoryProgressStrategy
{
    protected GameContext gameContext;
    protected StageManager stageManager;
    protected StageStoryManager stageStoryManager;
    protected UIManager uiManager;
    public AStageStoryProgressStrategy(GameContext gameContext)
    {
        this.gameContext = gameContext;
        this.stageManager = GameManager.Instance.campaignManager.stageManager;
        this.stageStoryManager = stageManager.stageStoryManager;
        this.uiManager = GameManager.Instance.uiManager;
    }
    public abstract int NextProgress(int curProgress);

    public abstract void EnterNextProgress(int curProgress, Action onComplete);

    public virtual IEnumerator EnterNextProgressRoutine(int curProgress, Action onComplete)
    {
        StageData stageData = stageManager.GetCurStageData();
        stageData.stageProgress = curProgress + 1;

        StageEnum.StageType stageType = stageData.stageType;
        StageStorySO stageStorySO = stageStoryManager.GetStageStorySO(stageType);

        if (stageStorySO == null || stageStorySO.phaseStoryList == null || curProgress >= stageStorySO.phaseStoryList.Count)
        {
            Logger.LogWarning($"[StageStory] No phaseStory for stageType: {stageType}, progress: {curProgress}");
            onComplete?.Invoke();
            yield break;
        }

        curProgress = stageData.stageProgress;
        StageMonoPhaseStory phaseStory = stageStorySO.phaseStoryList[curProgress];


        if (phaseStory.narrativeResourceSO != null)
        {
            foreach (var resource in phaseStory.narrativeResourceSO)
            {
                bool isDone = false;

                if (resource is AnnounceResourceSO announce)
                {
                    gameContext.announceUI.Show(announce, onComplete: () => { isDone = true; });
                }
                else if (resource is ImageResourceSO image)
                {
                    gameContext.imageUI.Show(image, true, () => { isDone = true; });
                }
                else if (resource is DialogResourceSO dialog)
                {
                    gameContext.dialogUI.Show(dialog, true, () => { isDone = true; });
                }
                else
                {
                    Logger.LogWarning($"[StageStory] Unknown NarrativeResourceSO type: {resource.GetType()}");
                    isDone = true;
                }

                yield return new WaitUntil(() => isDone);
            }
        }


        /*if (phaseStory.announceResourceSO != null)
        {
            bool isAnnounceDone = false;
            gameContext.announceUI.Show(phaseStory.announceResourceSO, onComplete: () => { isAnnounceDone = true; });

            yield return new WaitUntil(() => isAnnounceDone);
        }*/

        onComplete?.Invoke();
    }

    public void AnnounceClear(Action onComplete)
    {
        StageData stageData = stageManager.GetCurStageData();
        StageStorySO stageStorySO = stageStoryManager.GetStageStorySO(stageData.stageType);
        StageMonoPhaseStory clearStoryList = stageStoryManager.GetStageStorySO(stageData.stageType).clearStoryList[stageData.stageProgress];
        AnnounceResourceSO announceResourceSO = clearStoryList.announceResourceSO;
        uiManager.Announce(announceResourceSO, onComplete: onComplete);
        onComplete?.Invoke();
    }

    public void AnnounceFail(Action onComplete)
    {
        StageData stageData = stageManager.GetCurStageData();
        StageStorySO stageStorySO = stageStoryManager.GetStageStorySO(stageData.stageType);
        StageMonoPhaseStory failStoryList = stageStoryManager.GetStageStorySO(stageData.stageType).failStoryList[stageData.stageProgress];
        AnnounceResourceSO announceResourceSO = failStoryList.announceResourceSO;
        uiManager.Announce(announceResourceSO, onComplete: onComplete);
        onComplete?.Invoke();
    }

    public void AnnounceCurProgress(Action onComplete)
    {
        StageData stageData = stageManager.GetCurStageData();
        StageStorySO stageStorySO = stageStoryManager.GetStageStorySO(stageData.stageType);
        StageMonoPhaseStory phaseStoryList = stageStoryManager.GetStageStorySO(stageData.stageType).phaseStoryList[stageData.stageProgress];
        AnnounceResourceSO announceResourceSO = phaseStoryList.announceResourceSO;
        uiManager.Announce(announceResourceSO, onComplete: onComplete);
    }

    public abstract bool CanEnterNextProgress(int curProgress);

    public abstract bool CanStageClear(int curProgress);
    public abstract bool CanStageFail(int curProgress);
}