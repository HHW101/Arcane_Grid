using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageNPCTable
{
    public List<FixedNPCEntry> fixedNPCPrefaEntries = new List<FixedNPCEntry>();

    public List<WeightedNPCEntry> randomNPCPrefabEntries = new List<WeightedNPCEntry>();

    public int randomSpawnCount = 10;
}

[Serializable]
public class WeightedNPCEntry
{
    public GameObject prefab;
    public int weight;
}

[Serializable]
public class FixedNPCEntry
{
    public GameObject prefab;

    [Header("npc coord fixed?")]
    public bool isNPCCoordFixed = true;
    public HexCoord npcHexCoord = new HexCoord(0, 0);
}