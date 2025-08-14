using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStageTrigger : MonoBehaviour
{
    [SerializeField]
    private bool autoDestroy = false;
    // Start is called before the first frame update
    private void Start()
    {
        Logger.Log("[LoadStageTrigger] called.");
        GameManager.Instance.campaignManager.stageManager.LoadStage();
        GameManager.Instance.gameContext.ClearAllSelecting();
        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }
}
