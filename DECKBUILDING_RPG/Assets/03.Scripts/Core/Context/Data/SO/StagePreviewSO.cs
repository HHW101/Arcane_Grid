using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StagePreview", menuName = "Custom/StagePreview")]
public class StagePreviewSO : ScriptableObject
{
    public int radius;
    public StageEnum.StageType stageType;
    public StageNPCTable stageNPCTable;

    public List<ElevationSeedRule> elevationSeedRules;
    public List<ElevationRangeToTileType> elevationMap;

    public int cliffHeightThreshold = 2;
    public bool allowWater = true;
    public bool allowCliff = true;
    public bool allowBridge = true;

    [Header("player coord fixed?")]
    public bool isPlayerCoordFixed = false;
    public HexCoord playerHexCoord = new HexCoord(0, 0);
}


[Serializable]
public class ElevationSeed
{
    public HexCoord coord;
    public int elevation;
    public int spreadRange;
}

[Serializable]
public class ElevationSeedRule
{
    public int elevation;
    public int count;
    public int spreadRange;
    public int receiveRange = 0;
}

[Serializable]
public class ElevationRangeToTileType
{
    public int minHeight;
    public int maxHeight;
    public TileEnum.TileType tileType;
}