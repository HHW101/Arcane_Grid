using UnityEngine;

public class StartSceneUI : MonoBehaviour
{
    [SerializeField]
    CurSceneNameText curSceneNameText;
    [SerializeField]
    LoadSceneButton loadSceneButton;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.audioManager != null)
        {
            GameManager.Instance.audioManager.PlayBGMIndex(0);
            Logger.Log("브금실행");
        }
    }
}