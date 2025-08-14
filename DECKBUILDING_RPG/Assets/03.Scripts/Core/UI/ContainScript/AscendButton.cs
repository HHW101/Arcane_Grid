using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AscendButton : MonoBehaviour
{
    StageManager stageManager;
    private void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        if (gameObject.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        stageManager?.AscendStage();
    }
}
