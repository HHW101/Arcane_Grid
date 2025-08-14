
using System.Collections.Generic;
using System;
using UnityEngine;
using UnitEnum;

[Serializable]
public class RuleManagerParam
{
    public RuleBookSO ruleBookSO;
}

public class RuleManager : MonoBehaviour
{
    private RuleManagerParam ruleManagerParam = new();
    private RuleBook ruleBook;

    private GameContext gameContext;

    #region Init
    public void Init(GameContext gameContext, RuleManagerParam ruleManagerParam)
    {
        this.gameContext = gameContext;
        this.ruleManagerParam = ruleManagerParam;

        if (ruleManagerParam.ruleBookSO != null)
        {
            ruleBook = new RuleBook(ruleManagerParam.ruleBookSO);
        }
    }
    #endregion

    public bool CanUnitEnterTile(UnitData unitData, TileData tileData)
    {
        if (ruleBook == null)
            return true;

        return !ruleBook.IsDenied(unitData, tileData);
    }

    public int GetMoveSpeedModifier(UnitData unitData, TileData tileData)
    {
        if (ruleBook == null)
            return 1;

        return ruleBook.GetMoveSpeedModifier(unitData, tileData);
    }

    public UnitCampType GetUnitCampType(UnitData unitData)
    {
        return ruleBook.GetUnitCampType(unitData);
    }
}
