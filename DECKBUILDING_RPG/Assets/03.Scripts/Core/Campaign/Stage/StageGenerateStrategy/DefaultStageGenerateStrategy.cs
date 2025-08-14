using System.Collections.Generic;
using UnityEngine;

public class DefaultStageGenerateStrategy : AStageGenerateStrategy
{
    public DefaultStageGenerateStrategy(TileMapSO tileMapSO) : base(tileMapSO) { }

    public override StageData GenerateStage(StagePreviewSO preview, StageDataGenerator generator)
    {
        StageData stageData = GenerateHexTileMap(preview);
        stageData.stageProgress = -1;
        stageData.clearCondition = ClearEnum.ClearCondition.KillAllEnemy;
        stageData = PopulateNPCs(stageData, preview.stageNPCTable);
        stageData = BatchPlayer(preview, stageData);
        return stageData;
    }
}