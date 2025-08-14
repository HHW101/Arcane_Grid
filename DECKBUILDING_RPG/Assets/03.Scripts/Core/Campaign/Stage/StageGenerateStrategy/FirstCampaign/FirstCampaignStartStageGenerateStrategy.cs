using System.Collections.Generic;
using UnityEngine;

public class FirstCampaignStartStageGenerateStrategy : AStageGenerateStrategy
{
    public FirstCampaignStartStageGenerateStrategy(TileMapSO tileMapSO) : base(tileMapSO) { }

    public override StageData GenerateStage(StagePreviewSO preview, StageDataGenerator generator)
    {
        StageData stageData = GenerateHexTileMap(preview);
        stageData.stageType = StageEnum.StageType.FirstCampaignStart;
        stageData.stageProgress = -1;
        stageData = BatchPlayer(preview, stageData);
        return PopulateNPCs(stageData, preview.stageNPCTable);
    }
}