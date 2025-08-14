using System;
using TileEnum;

[Serializable]
public class TileData
{
    public string prefabPath;
    public HexCoord hexCoord;
    public bool turnEnded = false;
    public TileType tileType;
    public bool turnStarted = false;
    public float objectOnTileWorldPositionOffsetMultiTileRadius = 0.3f;

    public TileData CloneTileData()
    {
        return new TileData
        {
            prefabPath = this.prefabPath,
            hexCoord = this.hexCoord,
            turnEnded = this.turnEnded,
            tileType = this.tileType,
            turnStarted = this.turnStarted,
            objectOnTileWorldPositionOffsetMultiTileRadius = this.objectOnTileWorldPositionOffsetMultiTileRadius
        };
    }
}