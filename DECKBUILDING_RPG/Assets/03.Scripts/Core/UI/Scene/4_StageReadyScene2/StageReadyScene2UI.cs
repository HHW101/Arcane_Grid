using UnityEngine;
using UnityEngine.UI;

public class StageReadyScene2UI : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.PlayBGMIndex(5);
        }
    }
}