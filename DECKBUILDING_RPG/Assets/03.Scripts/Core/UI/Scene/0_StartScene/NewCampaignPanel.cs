using UnityEngine;
using UnityEngine.UI;

public class NewCampaignPanel : MonoBehaviour
{
    [SerializeField]
    private Button loadNewCampaignButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private string defaultNextScene;

    public void Start()
    {
        loadNewCampaignButton.onClick.AddListener(OnLoadNewCampaignButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    public void OnLoadNewCampaignButtonClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        string sceneToLoad = defaultNextScene;
        //새로운 시작 작동할 메서드를 여기에 추가하는 게 좋을지도.
        GameManager.Instance.gameContext.saveData.nowReinforceNum = 3;
        GameManager.Instance.gameContext.saveData.isStarted = false;
        SceneLoader.Instance.LoadSceneWithLoadingScreen(sceneToLoad);
    }

    public void OnCancelButtonClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        gameObject.SetActive(false);
    }
}