using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearStageUI : MonoBehaviour
{
    [SerializeField]
    private Button clearStageButton;
    [SerializeField]
    private string nextScene;
    private StageManager stageManager;

    private bool wasProcessing;
    public void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        //clearStageButton.onClick.AddListener(OnClickClearStageButton);
        wasProcessing = stageManager.IsProcessingUnitAct();
    }
    private void OnClickClearStageButton()
    {
        GameManager.Instance.audioManager.PlaySfx("Click-008");
        //stageManager.ForceClearCurStage();
        //SceneManager.LoadScene(nextScene);
    }
    private void Update()
    {
        if (wasProcessing && !stageManager.IsProcessingUnitAct())
        {
            clearStageButton.gameObject.SetActive(true);
        }
        else if(!wasProcessing && stageManager.IsProcessingUnitAct())
        {
            clearStageButton.gameObject.SetActive(false);
        }
        wasProcessing = stageManager.IsProcessingUnitAct();
    }
}