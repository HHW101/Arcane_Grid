using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateNewCampaignTrigger : MonoBehaviour
{
    [SerializeField]
    private bool autoDestroy = false;
    // Start is called before the first frame update
    private void Start()
    {
        Logger.Log("[GenrateNewCampaignTrigger] called.");
        GameManager.Instance.campaignManager.LoadNewCampaignData();
        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }
}
