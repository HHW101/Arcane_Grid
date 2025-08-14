using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageIndicatorText : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text stageIndicatorText = null;
    private CampaignData campaignData = null;
    private void Start()
    {
        if(this.TryGetComponent<TMP_Text>(out TMP_Text stageIndicatorText))
        {
            this.stageIndicatorText = stageIndicatorText;
        }
        campaignData = GameManager.Instance.gameContext.saveData.campaignData;
        this.stageIndicatorText?.SetText($"Stage : {campaignData.playerStateInCampaign.curStageIndex}");
    }
}
