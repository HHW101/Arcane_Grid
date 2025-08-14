using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName ="CampaignPreview", menuName ="Custom/CampaignPreview")]
public class CampaignPreviewSO: ScriptableObject
{
    public List<FloorStageTable> floorStageTableList;
    public string campaignName;
    public string campaignDescription;
    public Sprite campaignSprite;
    public CampaignEnum.CampaignTypeEnum campaignTypeEnum = CampaignEnum.CampaignTypeEnum.Default;
}

[Serializable]
public class FloorStageTable
{
    public int floorStageCount;
    public List<WeightedStageEntry> weightedStageEntryList;
}

[Serializable]
public class WeightedStageEntry
{
    public StagePreviewSO stagePreviewSO;
    public int weight;
}