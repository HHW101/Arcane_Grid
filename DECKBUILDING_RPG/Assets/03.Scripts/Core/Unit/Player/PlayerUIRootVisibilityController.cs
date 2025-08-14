using UnityEngine;

public class PlayerUIRootVisibilityController : MonoBehaviour
{
    [SerializeField]
    private GameObject playerUIRoot;

    private StageManager stageManager;

    private void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
    }

    private void Update()
    {
        bool shouldHide =
            stageManager.IsProcessingUnitAct()
            || stageManager.IsStageClear()
            || stageManager.IsStageFail();

        playerUIRoot.SetActive(!shouldHide);
    }
}