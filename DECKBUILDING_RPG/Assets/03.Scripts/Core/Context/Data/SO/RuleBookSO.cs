using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "RuleBook", menuName = "Custom/RuleBook")]
public class RuleBookSO : ScriptableObject
{
    public List<UnitTileDenyEntry> denyRules;
    public List<UnitTileMoveSpeedModifierEntry> moveSpeedModifierRules;
}

[Serializable]
public class UnitTileDenyEntry
{
    public UnitEnum.UnitType unitType;
    public TileEnum.TileType tileType;
}

[Serializable]
public class UnitTileMoveSpeedModifierEntry
{
    public UnitEnum.UnitType unitType;
    public TileEnum.TileType tileType;
    public int modifier;
}