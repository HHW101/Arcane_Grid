using UnityEngine;
using UnityEngine.UI;

public class StageFailUI : MonoBehaviour
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
        if (campaignManager.IsCampaignFail())
        {
            endCampaignButton.gameObject.SetActive(true);
        }
        else
        {
            nextStageButton.gameObject.SetActive(true);
        }
    }
}