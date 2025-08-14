using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EnumAssociatedResourceManagerParam
{
    public UnitIdentifierTypeResourceBundleListSO unitIdentifierTypeListSO;
    public TileTypeResourceBundleListSO tileTypeListSO;
    public JobTypeResourceBundleListSO jobTypeListSO;
    public UnitTypeResourceBundleListSO unitTypeListSO;
    public UnitCampTypeResourceBundleListSO unitCampTypeListSO;

    public Sprite notFoundSprite;
    public string notFoundString;
}

public class EnumAssociatedResourceManager : MonoBehaviour
{
    private EnumAssociatedResourceManagerParam param;
    private GameContext gameContext;

    private Dictionary<UnitEnum.UnitIdentifierType, UnitIdentifierTypeResourceBundle> unitIdentifierMap;
    private Dictionary<TileEnum.TileType, TileTypeResourceBundle> tileTypeMap;
    private Dictionary<JobEnum.Job, JobTypeResourceBundle> jobMap;
    private Dictionary<UnitEnum.UnitType, UnitTypeResourceBundle> unitTypeMap;
    private Dictionary<UnitEnum.UnitCampType, UnitCampTypeResourceBundle> unitCampTypeMap;

    public void Init(GameContext gameContext, EnumAssociatedResourceManagerParam param)
    {
        this.gameContext = gameContext;
        this.param = param;

        unitIdentifierMap = param.unitIdentifierTypeListSO.bundleList.ToDictionary(b => b.unitType);
        tileTypeMap = param.tileTypeListSO.bundleList.ToDictionary(b => b.tileType);
        jobMap = param.jobTypeListSO.bundleList.ToDictionary(b => b.job);
        unitTypeMap = param.unitTypeListSO.bundleList.ToDictionary(b => b.unitType);
        unitCampTypeMap = param.unitCampTypeListSO.bundleList.ToDictionary(b => b.unitCampType);
    }

    #region UnitIdentifierType

    public Sprite GetDialogSprite(UnitEnum.UnitIdentifierType type)
    {
        if (type == UnitEnum.UnitIdentifierType.Player)
            return GetDialogSpriteByJob(GetPlayerJob());

        return unitIdentifierMap.TryGetValue(type, out var b) ? b.dialogSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    public Sprite GetInfoSprite(UnitEnum.UnitIdentifierType type)
    {
        if (type == UnitEnum.UnitIdentifierType.Player)
            return GetInfoSpriteByJob(GetPlayerJob());

        return unitIdentifierMap.TryGetValue(type, out var b) ? b.infoSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    public string GetUnitIdentifierDescription(UnitEnum.UnitIdentifierType type)
    {
        if (type == UnitEnum.UnitIdentifierType.Player)
            return GetDescriptionByJob(GetPlayerJob());

        return unitIdentifierMap.TryGetValue(type, out var b) ? b.description ?? param.notFoundString : param.notFoundString;
    }

    public string GetUnitIdentifierName(UnitEnum.UnitIdentifierType type)
    {
        if (type == UnitEnum.UnitIdentifierType.Player)
            return GetNameByJob(GetPlayerJob());

        return unitIdentifierMap.TryGetValue(type, out var b) ? b.name ?? param.notFoundString : param.notFoundString;
    }

    #endregion

    #region Job

    public Sprite GetDialogSpriteByJob(JobEnum.Job job)
    {
        return jobMap.TryGetValue(job, out var b) ? b.dialogSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    public Sprite GetInfoSpriteByJob(JobEnum.Job job)
    {
        return jobMap.TryGetValue(job, out var b) ? b.infoSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    public string GetDescriptionByJob(JobEnum.Job job)
    {
        return jobMap.TryGetValue(job, out var b) ? b.description ?? param.notFoundString : param.notFoundString;
    }

    public string GetNameByJob(JobEnum.Job job)
    {
        return jobMap.TryGetValue(job, out var b) ? b.name ?? param.notFoundString : param.notFoundString;
    }

    #endregion

    #region TileType

    public Sprite GetTileInfoSprite(TileEnum.TileType type)
    {
        return tileTypeMap.TryGetValue(type, out var b) ? b.infoSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    public string GetTileTypeName(TileEnum.TileType type)
    {
        return tileTypeMap.TryGetValue(type, out var b) ? b.name ?? param.notFoundString : param.notFoundString;
    }

    public string GetTileDescription(TileEnum.TileType type)
    {
        return tileTypeMap.TryGetValue(type, out var b) ? b.description ?? param.notFoundString : param.notFoundString;
    }

    #endregion

    #region UnitType

    public string GetUnitTypeName(UnitEnum.UnitType type)
    {
        return unitTypeMap.TryGetValue(type, out var b) ? b.name ?? param.notFoundString : param.notFoundString;
    }

    public string GetUnitTypeDescription(UnitEnum.UnitType type)
    {
        return unitTypeMap.TryGetValue(type, out var b) ? b.description ?? param.notFoundString : param.notFoundString;
    }

    #endregion


    #region UnitCampType

    public Sprite GetObjectTypeMarkerSprite(UnitEnum.UnitCampType type)
    {
        return unitCampTypeMap.TryGetValue(type, out var b) ? b.ObjectTypeMarkerSprite ?? param.notFoundSprite : param.notFoundSprite;
    }

    #endregion

    #region Helpers

    private JobEnum.Job GetPlayerJob()
    {
        return gameContext.player?.playerData?.jobEnum ?? JobEnum.Job.Warrior;
    }

    #endregion
}
