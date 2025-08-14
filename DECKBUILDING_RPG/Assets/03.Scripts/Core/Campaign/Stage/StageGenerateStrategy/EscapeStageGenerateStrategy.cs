using System.Collections.Generic;

public class EscapeStageGenerateStrategy : AStageGenerateStrategy
{
    public EscapeStageGenerateStrategy(TileMapSO tileMapSO) : base(tileMapSO) { }

    public override StageData GenerateStage(StagePreviewSO preview, StageDataGenerator generator)
    {
        StageData stageData = GenerateHexTileMap(preview);

        string escapeTilePath = tileMapSO.tileMap[TileEnum.TileType.Escape].tileData.prefabPath;

        List<TileData> candidateTiles = new List<TileData>();
        foreach (var tile in stageData.tiles.Values)
        {
            if (tile.hexCoord.q != 0 || tile.hexCoord.r != 0)
            {
                candidateTiles.Add(tile);
            }
        }

        if (candidateTiles.Count > 0)
        {
            System.Random random = new System.Random();
            TileData escapeTile = candidateTiles[random.Next(candidateTiles.Count)];
            escapeTile.prefabPath = escapeTilePath;
            escapeTile.tileType = TileEnum.TileType.Escape;
        }

        stageData.stageProgress = -1;
        stageData.clearCondition = ClearEnum.ClearCondition.Escape;
        stageData = PopulateNPCs(stageData, preview.stageNPCTable);
        stageData = BatchPlayer(preview, stageData);
        return stageData;
    }
}