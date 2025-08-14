using UnityEngine;

public class LoadCampaignButton : MonoBehaviour
{
    [SerializeField]
    private GameObject loadCampaignPanel;
    [SerializeField]
    private string defaultNextScene;

    public void OnClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");

        SaveManager saveManager = GameManager.Instance.saveManager;
        if (saveManager.IsHaveLastCampaignData())
        {
            string sceneToLoad = saveManager.LastSavedScene() ?? defaultNextScene;
            SceneLoader.Instance.LoadSceneWithLoadingScreen(sceneToLoad);
        }
        else
        {
            loadCampaignPanel.SetActive(true);
        }
    }
}