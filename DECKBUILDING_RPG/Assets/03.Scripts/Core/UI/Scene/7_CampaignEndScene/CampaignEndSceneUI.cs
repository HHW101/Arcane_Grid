using UnityEngine;
using UnityEngine.UI;

public class CampaignEndSceneUI : MonoBehaviour
{
    [SerializeField]
    CampaignClearUI campaignClearUI;
    [SerializeField]
    CampaignFailUI campaignFailUI;
    [SerializeField]
    private ResultPanel resultPanel;

    public void Start()
    {
        campaignFailUI.gameObject.SetActive(false);
        campaignClearUI.gameObject.SetActive(false);
        CampaignManager campaignManager = GameManager.Instance.campaignManager;
        if (campaignManager.IsCampaignClear())
        {
            campaignClearUI.gameObject.SetActive(true);
        }
        else if (campaignManager.IsCampaignFail())
        {
            campaignFailUI.gameObject.SetActive(true);
        }
        resultPanel.Init(campaignManager.GetCampaignData().dealDamage, campaignManager.GetCampaignData().earnGold);
        SaveManager saveManager = GameManager.Instance.saveManager;
        saveManager.SetHaveLastCampaignData(false);
    }
}