using UnityEngine;
using UnityEngine.UI;

public class CampaignEnterSceneUI : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.PlayBGMIndex(5);
        }
    }
}