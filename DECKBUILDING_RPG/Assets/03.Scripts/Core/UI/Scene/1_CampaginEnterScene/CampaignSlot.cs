using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CampaignSlot : MonoBehaviour
{
    // Start is called before the first frame update
    private CampaignPreviewSO campaignPreviewSO;
    [SerializeField]
    private TMP_Text slotText;
    private TMP_Text campaignNameText;
    private TMP_Text descriptionText;
    private Image campaignImage;
    [SerializeField]
    private Button button;
    private CampaignManager campaignManager;

    public void Init(CampaignPreviewSO campaignPreviewSO, TMP_Text campaignNameText, TMP_Text descriptionText, Image campaignImage)
    {
        slotText?.SetText(campaignPreviewSO.campaignName);
        this.campaignPreviewSO = campaignPreviewSO;
        this.campaignNameText = campaignNameText;
        this.descriptionText = descriptionText;
        this.campaignImage = campaignImage;
        button?.onClick.AddListener(OnClick);
        campaignManager = GameManager.Instance.campaignManager;
    }

    public void OnClick()
    {
        campaignNameText.SetText(campaignPreviewSO.campaignName);
        descriptionText.SetText(campaignPreviewSO.campaignDescription);
        campaignManager.SelectCampaignPreviewSO(campaignPreviewSO);
        if (campaignImage != null)
        {
            campaignImage.sprite = campaignPreviewSO.campaignSprite;
        }
    }

    public void Click()
    {
        OnClick();
    }
}
