using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class CampaignManagerChild
{
    public StageManager stageManager;
}

[Serializable]
public class CampaignManagerParam
{
    public StageManagerParam stageManagerParam;
    public TileMapSO tileMapSO;
    public List<CampaignPreviewSO> campaignPreviewList;
}

public class CampaignManager : MonoBehaviour
{
    private CampaignManagerParam campaignManagerParam = new();

    private readonly CampaignManagerChild campaignManagerChild = new CampaignManagerChild();

    public StageManager stageManager => campaignManagerChild.stageManager;

    private CampaignDataGenerator campaignDataGenerator;
    private GameContext gameContext;
    private List<string> campaignDataPathList;

    public void Init(GameContext gameContext, CampaignManagerParam campaignManagerParam)
    {
        this.gameContext = gameContext;
        this.campaignManagerParam = campaignManagerParam;
        
        campaignDataGenerator = new CampaignDataGenerator(campaignManagerParam.tileMapSO);

        GameObject stageManagerObject = new GameObject("StageManager");
        stageManagerObject.transform.SetParent(this.transform);
        campaignManagerChild.stageManager = stageManagerObject.AddComponent<StageManager>();
        campaignManagerChild.stageManager.Init(gameContext, campaignManagerParam.stageManagerParam);
    }

    public void AddDealDamage(int damage)
    {
        gameContext.saveData.campaignData.dealDamage += damage;
        campaignManagerChild.stageManager.AddDealDamage(damage);
    }
    public void AddEarnGold(int gold)
    {
        gameContext.saveData.campaignData.earnGold += gold;
        campaignManagerChild.stageManager.AddEarnGold(gold);
    }

    [ContextMenu("LoadNewCampaign")]
    public void LoadNewCampaignData()
    {
        SaveData saveData = gameContext.saveData;
        saveData.campaignData = campaignDataGenerator.GenerateNewCampaignData(gameContext.selectedCampaignPreviewSO);
        saveData.isStartedNewCampaign = true;
    }

    public bool IsCampaignClear()
    {
        return gameContext.saveData.campaignData.isClear;
    }

    public bool IsCampaignFail()
    {
        return gameContext.saveData.campaignData.isFail;
    }

    public List<CampaignPreviewSO> GetCampaignPreviewSOList()
    {
        return campaignManagerParam.campaignPreviewList;
    }

    public CampaignData GetCampaignData()
    {
        return gameContext.saveData.campaignData;
    }

    public void SelectCampaignPreviewSO(CampaignPreviewSO campaignPreviewSO)
    {
        gameContext.selectedCampaignPreviewSO = campaignPreviewSO;
    }
}