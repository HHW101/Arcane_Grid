using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public abstract class AStageGenerateStrategy
{
    protected readonly TileMapSO tileMapSO;
    protected RuleManager ruleManager;
    protected GameManager gameManager;

    public AStageGenerateStrategy(TileMapSO tileMapSO)
    {
        this.tileMapSO = tileMapSO;
        this.ruleManager = GameManager.Instance.ruleManager;
        this.gameManager = GameManager.Instance;
    }

    public abstract StageData GenerateStage(StagePreviewSO preview, StageDataGenerator generator);

    protected virtual StageData GenerateHexTileMap(StagePreviewSO preview)
    {
        int radius = preview.radius;
        StageData stageData = new StageData();

        List<HexCoord> allCoords = new();
        for (int q = -radius + 1; q <= radius - 1; q++)
        {
            int r1 = Mathf.Max(-radius + 1, -q - radius + 1);
            int r2 = Mathf.Min(radius - 1, -q + radius - 1);
            for (int r = r1; r <= r2; r++)
            {
                HexCoord coord = new(q, r);
                allCoords.Add(coord);
            }
        }

        var seeds = GenerateElevationSeeds(preview, allCoords);

        foreach (var coord in allCoords)
        {
            float totalWeight = 0f;
            float weightedSum = 0f;

            foreach (var seed in seeds)
            {
                int dist = HexCoord.Distance(coord, seed.coord);
                if (dist > seed.spreadRange) continue;
                float weight = 1f / (dist + 1);
                totalWeight += weight;
                weightedSum += seed.elevation * weight;
            }

            int elevation = totalWeight > 0 ? Mathf.RoundToInt(weightedSum / totalWeight) : 2;
            var tileType = GetTileTypeFromElevation(preview, elevation);
            TileData tileData = tileMapSO.tileMap[tileType].tileData.CloneTileData();

            string key = coord.ToString();
            tileData.hexCoord = coord;

            stageData.tiles[key] = tileData;
            stageData.tileDataQueue.Enqueue(tileData);
        }

        Dictionary<HexDirection, (int dq, int dr)> dirs = new()
        {
            { HexDirection.TopRight,    (1, -1) },
            { HexDirection.Right,       (1,  0) },
            { HexDirection.BottomRight, (0,  1) },
            { HexDirection.BottomLeft,  (-1, 1) },
            { HexDirection.Left,        (-1, 0) },
            { HexDirection.TopLeft,     (0, -1) }
        };

        foreach (var key in stageData.tiles.Keys)
        {
            HexCoord coord = HexCoord.FromString(key);
            var conn = new Dictionary<HexDirection, string>();

            foreach (var (dir, offset) in dirs)
            {
                var neighbor = new HexCoord(coord.q + offset.dq, coord.r + offset.dr);
                string neighborKey = neighbor.ToString();
                if (stageData.tiles.ContainsKey(neighborKey))
                    conn[dir] = neighborKey;
            }

            stageData.tileConnections[key] = conn;
        }

        return stageData;
    }

    protected virtual StageData BatchPlayer(StagePreviewSO preview, StageData stageData)
    {
        PlayerStateInStage playerStateInStage = stageData.playerStateInStage;
        PlayerData playerData = gameManager.gameContext.saveData.playerData;

        var npcBlocked = BuildNPCOccupiedSet(stageData);

        if (preview.isPlayerCoordFixed)
        {
            if (!npcBlocked.Contains(preview.playerHexCoord)
                && !IsUnitOnTile(stageData, preview.playerHexCoord))
            {
                playerStateInStage.hexCoord = preview.playerHexCoord;
                return stageData;
            }

            var altTiles = GetValidSpawnableTiles(stageData, playerData);
            altTiles.RemoveAll(t => npcBlocked.Contains(t.hexCoord));
            altTiles.Shuffle(new Random());
            if (altTiles.Count > 0)
            {
                playerStateInStage.hexCoord = altTiles[0].hexCoord;
            }
            else
            {
                Debug.LogWarning("[BatchPlayer] No valid alternative spawn (fixed coord conflicted with NPC).");
            }
            return stageData;
        }

        List<TileData> spawnableTiles = GetValidSpawnableTiles(stageData, playerData);
        spawnableTiles.RemoveAll(t => t.hexCoord.q == 0 && t.hexCoord.r == 0);
        spawnableTiles.RemoveAll(t => npcBlocked.Contains(t.hexCoord));

        Random random = new();
        spawnableTiles.Shuffle(random);

        int index = spawnableTiles.FindIndex(t => ruleManager.CanUnitEnterTile(playerData, t));
        if (index == -1)
        {
            Debug.LogWarning("[BatchPlayer] player can't be batched: No valid spawnable Tiles.");
            return stageData;
        }

        var tile = spawnableTiles[index];
        playerStateInStage.hexCoord = tile.hexCoord;
        return stageData;
    }

    protected List<ElevationSeed> GenerateElevationSeeds(StagePreviewSO preview, List<HexCoord> allCoords)
    {
        List<ElevationSeed> seeds = new();
        HashSet<HexCoord> blockedCoords = new();
        Random rng = new();

        foreach (var rule in preview.elevationSeedRules)
        {
            int placed = 0;
            int attempts = 0;

            while (placed < rule.count && attempts < 1000)
            {
                int index = rng.Next(allCoords.Count);
                var coord = allCoords[index];

                if (blockedCoords.Contains(coord))
                {
                    attempts++;
                    continue;
                }

                seeds.Add(new ElevationSeed
                {
                    coord = coord,
                    elevation = rule.elevation,
                    spreadRange = rule.spreadRange
                });

                placed++;

                foreach (var other in allCoords)
                {
                    if (HexCoord.Distance(coord, other) <= rule.receiveRange)
                        blockedCoords.Add(other);
                }
            }
        }

        return seeds;
    }

    protected TileEnum.TileType GetTileTypeFromElevation(StagePreviewSO preview, int elevation)
    {
        foreach (var map in preview.elevationMap)
        {
            if (elevation >= map.minHeight && elevation <= map.maxHeight)
                return map.tileType;
        }
        return TileEnum.TileType.DesertDirt;
    }
    protected virtual StageData PopulateNPCs(StageData stageData, StageNPCTable table)
    {
        Random random = new();

        var npcBlocked = BuildNPCOccupiedSet(stageData);

        foreach (var entry in table.fixedNPCPrefaEntries)
        {
            if (entry.prefab == null) continue;

            var npc = entry.prefab.GetComponent<ANPC>();
            if (npc == null) continue;

            var npcData = npc.npcData.CloneNPCData();

            if (entry.isNPCCoordFixed)
            {
                if (npcBlocked.Contains(entry.npcHexCoord) || IsUnitOnTile(stageData, entry.npcHexCoord))
                {
                    var alt = GetValidSpawnableTiles(stageData, npc.npcData);
                    alt.RemoveAll(t => npcBlocked.Contains(t.hexCoord));
                    alt.Shuffle(random);
                    if (alt.Count > 0)
                    {
                        npcData.hexCoord = alt[0].hexCoord;
                        Debug.LogWarning($"[PopulateNPCs] Fixed NPC overlapped; moved to {npcData.hexCoord} instead.");
                    }
                    else
                    {
                        Debug.LogWarning("[PopulateNPCs] No valid tile for overlapping fixed NPC. Skipped.");
                        continue;
                    }
                }
                else
                {
                    npcData.hexCoord = entry.npcHexCoord;
                }
            }
            else
            {
                List<TileData> spawnableTiles = GetValidSpawnableTiles(stageData, npc.npcData);
                spawnableTiles.RemoveAll(t => npcBlocked.Contains(t.hexCoord));
                if (spawnableTiles.Count == 0) continue;

                spawnableTiles.Shuffle(random);
                npcData.hexCoord = spawnableTiles[0].hexCoord;
            }

            stageData.npcDataQueue.Enqueue(npcData);
            npcBlocked.Add(npcData.hexCoord);
        }

        int totalWeight = 0;
        foreach (var entry in table.randomNPCPrefabEntries)
            totalWeight += entry.weight;

        for (int i = 0; i < table.randomSpawnCount; i++)
        {
            int roll = random.Next(totalWeight);
            int acc = 0;
            GameObject selected = null;

            foreach (var entry in table.randomNPCPrefabEntries)
            {
                acc += entry.weight;
                if (roll < acc)
                {
                    selected = entry.prefab;
                    break;
                }
            }

            var npc = selected?.GetComponent<ANPC>();
            if (npc == null) continue;

            List<TileData> spawnableTiles = GetValidSpawnableTiles(stageData, npc.npcData);
            spawnableTiles.RemoveAll(t => npcBlocked.Contains(t.hexCoord));
            if (spawnableTiles.Count == 0) continue;

            spawnableTiles.Shuffle(random);

            var npcData = npc.npcData.CloneNPCData();
            npcData.hexCoord = spawnableTiles[0].hexCoord;
            stageData.npcDataQueue.Enqueue(npcData);
            npcBlocked.Add(npcData.hexCoord);
        }

        return stageData;
    }

    protected List<TileData> GetValidSpawnableTiles(StageData stageData, UnitData unitData)
    {
        List<TileData> result = new List<TileData>();
        StageManager stageManager = gameManager.campaignManager.stageManager;

        var npcBlocked = BuildNPCOccupiedSet(stageData);

        foreach (var tile in stageData.tiles.Values)
        {
            if (tile.hexCoord.q == 0 && tile.hexCoord.r == 0) continue;
            if (npcBlocked.Contains(tile.hexCoord)) continue;
            if (stageManager.IsUnitOnTileInStageData(tile.hexCoord, stageData)) continue;

            if (ruleManager.CanUnitEnterTile(unitData, tile))
            {
                result.Add(tile);
            }
        }

        return result;
    }

    protected HashSet<HexCoord> BuildNPCOccupiedSet(StageData stageData)
    {
        var blocked = new HashSet<HexCoord>();
        if (stageData?.npcDataQueue != null)
        {
            foreach (var npcData in stageData.npcDataQueue)
                blocked.Add(npcData.hexCoord);
        }
        return blocked;
    }

    protected bool IsUnitOnTile(StageData stageData, HexCoord coord)
    {
        StageManager stageManager = gameManager.campaignManager.stageManager;
        return stageManager.IsUnitOnTileInStageData(coord, stageData);
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list, System.Random rng)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}
