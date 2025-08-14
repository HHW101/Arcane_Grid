using StageEnum;
using System;
using System.Collections;
using UnityEngine;

public class BossStageStoryProgresStrategy : AStageStoryProgressStrategy
{
    public BossStageStoryProgresStrategy(GameContext gameContext) : base(gameContext)
    {
    }

    public override bool CanEnterNextProgress(int curProgress)
    {
        if (curProgress == -1)
        {
            return true;
        }
        return false;
    }

    public override bool CanStageClear(int curProgress)
    {
        StageData stageData = stageManager.GetCurStageData();
        if (stageData.isClear)
        {
            return true;
        }
        if (gameContext.enemyCount == 0)
        {
            return true;
        }
        return false;
    }

    public override bool CanStageFail(int curProgress)
    {
        StageData stageData = stageManager.GetCurStageData();
        if (stageData.isFail)
        {
            return true;
        }
        if (gameContext.saveData.playerData.isDead)
        {
            return true;
        }
        return false;
    }

    public override void EnterNextProgress(int curProgress, Action onComplete)
    {
        StageData stageData = stageManager.GetCurStageData();
        stageData.stageProgress = curProgress + 1;
        stageManager.StartCoroutine(EnterNextProgressRoutine(curProgress, onComplete));
    }

    public override IEnumerator EnterNextProgressRoutine(int curProgress, Action onComplete)
    {
        return base.EnterNextProgressRoutine(curProgress, onComplete);
    }

    public override int NextProgress(int curProgress)
    {
        StageData stageData = stageManager.GetCurStageData();
        return stageData.stageProgress + 1;
    }
}