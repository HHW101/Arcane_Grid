using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnIndicatorText : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text turnIndicatorText = null;
    private CampaignData campaignData = null;
    private void Start()
    {
        if(this.TryGetComponent<TMP_Text>(out TMP_Text turnIndicatorText))
        {
            this.turnIndicatorText = turnIndicatorText;
        }
        campaignData = GameManager.Instance.gameContext.saveData.campaignData;
    }

    // Update is called once per frame
    private void Update()
    {
        this.turnIndicatorText?.SetText($"Turn : {campaignData.stageDataList[campaignData.playerStateInCampaign.curStageIndex].curTurn}");
    }
}
