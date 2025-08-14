using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public DifficultLevel difficultLevel = DifficultLevel.Easy;
    public string curSceneName;
    public int KillCount = 0;
    public PlayerData playerData = null;
    public CampaignData campaignData = new CampaignData();
    public Dictionary<string, SceneData> sceneDatas = new Dictionary<string, SceneData>();
    public bool isStarted = false;
    public int nowReinforceNum = 3;
    public bool isStartedNewCampaign = false;
}

public enum DifficultLevel
{
    Easy,
    Normal,
    Hard,
    Impossible
}