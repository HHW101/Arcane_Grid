using System.Collections.Generic;
using UnityEngine;
using UnitEnum;

public class RuleBook
{
    private HashSet<(UnitEnum.UnitType, TileEnum.TileType)> denySet = new HashSet<(UnitEnum.UnitType, TileEnum.TileType)>();
    private Dictionary<(UnitEnum.UnitType, TileEnum.TileType), int> moveSpeedModifierMap = new Dictionary<(UnitEnum.UnitType, TileEnum.TileType), int>();

    public RuleBook(RuleBookSO ruleBookSO)
    {
        foreach (var entry in ruleBookSO.denyRules)
        {
            if (!denySet.Contains((entry.unitType, entry.tileType)))
            {
                denySet.Add((entry.unitType, entry.tileType));
            }
            else
            {
                Debug.LogWarning($"[RuleBook] Duplicate entry for {entry.unitType +  " : " + entry.tileType} ignored.");
            }
        }
        foreach(var entry in ruleBookSO.moveSpeedModifierRules)
        {
            if (!moveSpeedModifierMap.ContainsKey((entry.unitType, entry.tileType)))
            {
                moveSpeedModifierMap.Add((entry.unitType, entry.tileType), entry.modifier);
            }
            else
            {
                Debug.LogWarning($"[moveSpeedModifierRules] Duplicate entry for {entry.unitType + " : " + entry.tileType} ignored.");
            }
        }
    }

    public bool IsDenied(UnitData unitData, TileData tileData)
    {
        foreach (var unitType in unitData.unitTypeSet)
        {
            if (denySet.Contains((unitType, tileData.tileType)))
                return true;
        }
        return false;
    }

    public int GetMoveSpeedModifier(UnitData unitData, TileData tileData)
    {
        int result = 1;
        foreach (var unitType in unitData.unitTypeSet)
        {
            if (moveSpeedModifierMap.TryGetValue((unitType, tileData.tileType), out int modifier))
                result *= modifier;
        }
        return result;
    }

    public UnitCampType GetUnitCampType(UnitData unitData)
    {
        if(unitData is PlayerData playerData)
        {
            return UnitCampType.Ally;
        }
        if(unitData is NPCData npcData)
        {
            if (npcData.isEnemy)
            {
                return UnitCampType.Enemy;
            }
            else if (npcData.isAttackEnemy)
            {
                return UnitCampType.Ally;
            }
        }
        return UnitCampType.Neutral;
    }
}
