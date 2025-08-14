using UnityEngine;
using UnityEngine.UI;

public class StageResultSceneUI : MonoBehaviour
{
    [SerializeField]
    private StageClearUI clearUI;
    [SerializeField]
    private StageFailUI failUI;
    [SerializeField]
    private ResultPanel resultPanel;

    public void Start()
    {
        clearUI.gameObject.SetActive(false);
        failUI.gameObject.SetActive(false);
        CampaignManager campaignManager = GameManager.Instance.campaignManager;
        StageManager stageManager = campaignManager.stageManager;
        if (stageManager.CanStageFail())
        {
            failUI.gameObject.SetActive(true);
        }
        else if (stageManager.CanStageClear())
        {
            clearUI.gameObject.SetActive(true);
        }
        resultPanel.Init(stageManager.GetCurStageData().dealDamage, stageManager.GetCurStageData().earnGold);

        GameManager.Instance.audioManager.PlayBGMIndex(6);
    }
}