using StageEnum;
using System;
using System.Collections.Generic;

public class StageStoryProgressor
{
    private readonly Dictionary<StageType, AStageStoryProgressStrategy> stageStoryProgressStrategyMap;

    private AStageStoryProgressStrategy curStageTypeProgressStrategy = null;

    public StageStoryProgressor(GameContext gameContext)
    {
        stageStoryProgressStrategyMap = new Dictionary<StageType, AStageStoryProgressStrategy>
        {
            {StageType.Default, new DefaultStageStoryProgresStrategy(gameContext)},
            {StageType.Escape, new EscapeStageStoryProgresStrategy(gameContext)},
            {StageType.Boss, new BossStageStoryProgresStrategy(gameContext)},
            {StageType.FirstCampaignStart, new FirstCampaignStartStageStoryProgresStrategy(gameContext) },
            {StageType.TutorialCampaignStart , new TutorialCampaignStartStageStoryProgrssStrategy(gameContext)}
        };
    }

    public void SelectStrategyForCurStageType(StageType stageType)
    {
        if (stageStoryProgressStrategyMap.ContainsKey(stageType))
        {
            curStageTypeProgressStrategy = stageStoryProgressStrategyMap[stageType];
            Logger.Log($"[StageStoryProgressor] selected cur stage type progress strategy : {stageType}");
        }
        else
        {
            Logger.Log($"[StageStoryProgressor] not include cur stage type progress strategy : {stageType}");
        }
    }

    public void AnnounceCurProgress(Action onComplete)
    {
        if(curStageTypeProgressStrategy == null)
        {
            onComplete?.Invoke();
        }
        curStageTypeProgressStrategy?.AnnounceCurProgress(onComplete);
    }

    public bool CanEnterNextProgress(int curStageProgress)
    {
        if(curStageTypeProgressStrategy == null)
        {
            return false;
        }
        return curStageTypeProgressStrategy.CanEnterNextProgress(curStageProgress);
    }

    public void EnterNextProgress(int curStageProgress, Action onComplete)
    {
        if (curStageTypeProgressStrategy == null)
        {
            onComplete?.Invoke();
        }
        curStageTypeProgressStrategy?.EnterNextProgress(curStageProgress, onComplete);
    }
}