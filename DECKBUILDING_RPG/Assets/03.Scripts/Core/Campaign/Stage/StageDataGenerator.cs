using StageEnum;
using System.Collections.Generic;
using UnityEngine;

public class StageDataGenerator
{
    private readonly Dictionary<StageEnum.StageType, AStageGenerateStrategy> strategyMap;

    public StageDataGenerator(TileMapSO tileMapSO)
    {
        strategyMap = new Dictionary<StageEnum.StageType, AStageGenerateStrategy>
        {
            { StageEnum.StageType.Default, new DefaultStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.Escape, new EscapeStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.Boss, new BossStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.FirstCampaignStart, new FirstCampaignStartStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.TutorialCampaignStart , new DefaultStageGenerateStrategy(tileMapSO)},
            { StageEnum.StageType.TutorialCampaignEscape, new EscapeStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.TutorialCampaignRest, new EscapeStageGenerateStrategy(tileMapSO) },
            { StageEnum.StageType.TutorialCampaignBattle, new DefaultStageGenerateStrategy(tileMapSO) }
        };
    }

    public StageData GenerateStageData(StagePreviewSO preview)
    {
        if (!strategyMap.TryGetValue(preview.stageType, out var strategy))
        {
            Debug.LogError($"[StageDataGenerator] No strategy found for type {preview.stageType}");
            return null;
        }
        StageData stageData = strategy.GenerateStage(preview, this);
        stageData.stageType = preview.stageType;
        return stageData;
    }
}
