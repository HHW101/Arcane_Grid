using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileTypeEntry
{
    public TileEnum.TileType tileType;
    public ATile tilePrefab;
}

[CreateAssetMenu(fileName = "TileMap", menuName = "Custom/TileMap")]
public class TileMapSO : ScriptableObject
{
    public List<TileTypeEntry> tileList;

    private Dictionary<TileEnum.TileType, ATile> tileMapCache;

    public Dictionary<TileEnum.TileType, ATile> tileMap
    {
        get
        {
            if (tileMapCache == null)
            {
                tileMapCache = new Dictionary<TileEnum.TileType, ATile>();
                foreach (var entry in tileList)
                {
                    if (!tileMapCache.ContainsKey(entry.tileType))
                    {
                        tileMapCache.Add(entry.tileType, entry.tilePrefab);
                    }
                }
            }
            return tileMapCache;
        }
    }
}