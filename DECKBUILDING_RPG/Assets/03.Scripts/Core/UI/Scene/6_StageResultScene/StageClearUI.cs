
using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
{
    [SerializeField]
    private Button nextStageButton;
    [SerializeField]
    private Button endCampaignButton;

    public void Start()
    {
        nextStageButton.gameObject.SetActive(false);
        endCampaignButton.gameObject.SetActive(false);
        CampaignManager campaignManager = GameManager.Instance.campaignManager;
        StageManager stageManager = campaignManager.stageManager;
       
        GameManager.Instance.gameContext.saveData.isStarted = false;
        GameManager.Instance.gameContext.saveData.nowReinforceNum = 3;
        if (campaignManager.IsCampaignClear())
        {
            endCampaignButton.gameObject.SetActive(true);
        }
        else
        {
            nextStageButton.gameObject.SetActive(true);
        }
    }
}