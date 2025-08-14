using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignScrollView : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Prefab & Parent")]
    [SerializeField] private GameObject campaignSlotPrefab;
    [SerializeField] private Transform campaignSlotParent;

    [SerializeField] private TMP_Text campaignNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image campaignImage;

    private List<CampaignSlot> campaignSlotList = new List<CampaignSlot>();


    void Start()
    {
        List<CampaignPreviewSO> campaignPreviewSOList = GameManager.Instance.campaignManager.GetCampaignPreviewSOList();

        foreach (var campaignPreviewSO in campaignPreviewSOList)
        {
            GameObject slotObj = Instantiate(campaignSlotPrefab, campaignSlotParent);
            if (slotObj.TryGetComponent<CampaignSlot>(out CampaignSlot campaignSlot))
            {
                campaignSlot.Init(campaignPreviewSO, campaignNameText, descriptionText, campaignImage);
                campaignSlotList.Add(campaignSlot);
            }
        }

        if (campaignSlotList.Count > 0)
        {
            campaignSlotList[0].Click();
        }

    }
}
