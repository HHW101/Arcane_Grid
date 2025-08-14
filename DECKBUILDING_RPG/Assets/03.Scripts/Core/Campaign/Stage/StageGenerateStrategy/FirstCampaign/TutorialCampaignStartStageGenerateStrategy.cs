using System.Collections.Generic;
using UnityEngine;

public class TutorialCampaignStartStageGenerateStrategy : AStageGenerateStrategy
{
    public TutorialCampaignStartStageGenerateStrategy(TileMapSO tileMapSO) : base(tileMapSO) { }

    public override StageData GenerateStage(StagePreviewSO preview, StageDataGenerator generator)
    {
        StageData stageData = GenerateHexTileMap(preview);
        stageData.stageType = StageEnum.StageType.TutorialCampaignStart;
        stageData.stageProgress = -1;
        stageData = BatchPlayer(preview, stageData);
        return PopulateNPCs(stageData, preview.stageNPCTable);
    }
}