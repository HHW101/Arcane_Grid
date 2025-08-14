using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMoveTrigger : MonoBehaviour
{
    void Start()
    {
        CampaignData campaignData = GameManager.Instance.campaignManager.GetCampaignData();
        campaignData.playerStateInCampaign.curStageIndex = campaignData.playerStateInCampaign.nextStageIndex;
    }
}
