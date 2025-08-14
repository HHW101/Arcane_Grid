using System.Collections.Generic;
using System;

[Serializable]
public class CampaignData
{
    public List<StageData> stageDataList = new List<StageData>();
    public PlayerStateInCampaign playerStateInCampaign = new PlayerStateInCampaign();
    public bool isClear = false;
    public bool isFail = false;
    public int dealDamage = 0;
    public int earnGold = 0;
    public CampaignEnum.CampaignTypeEnum campaignTypeEnum = CampaignEnum.CampaignTypeEnum.Default;
}

[Serializable]
public class PlayerStateInCampaign
{
    public int curStageIndex = -1;
    public int nextStageIndex = -1;
}