using UnityEngine;
using UnityEngine.UI;

public class LoadCampaignPanel : MonoBehaviour
{
    [SerializeField]
    private Button loadNewCampaignButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private string defaultNextScene;

    private void Start()
    {
        loadNewCampaignButton.onClick.AddListener(OnLoadNewCampaignButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    public void OnLoadNewCampaignButtonClick()
    {
        string sceneToLoad = defaultNextScene;
        SceneLoader.Instance.LoadSceneWithLoadingScreen(sceneToLoad);
    }

    public void OnCancelButtonClick()
    {
        gameObject.SetActive(false);
    }
}