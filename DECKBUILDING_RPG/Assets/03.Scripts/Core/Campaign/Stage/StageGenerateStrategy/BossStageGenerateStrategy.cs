public class BossStageGenerateStrategy : AStageGenerateStrategy
{
    public BossStageGenerateStrategy(TileMapSO tileMapSO) : base(tileMapSO) { }

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
