using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageNodeUI : MonoBehaviour
{
    [SerializeField] private GameObject highlightObject;
    [SerializeField] private TMP_Text currentStageText;

    [SerializeField] private TMP_Text selectableStageText;
    [SerializeField] private Button button;

    private int stageIndex;
    private System.Action<int> onClickCallback;

    private void Awake()
    {
        highlightObject.SetActive(false);
        button.interactable = false;
    }

    public void Init(int stageIndex, bool isSelectable, bool isCurrentStage, System.Action<int> onClick)
    {
        this.stageIndex = stageIndex;
        this.onClickCallback = onClick;

        highlightObject.SetActive(isSelectable);
        button.interactable = isSelectable;
        selectableStageText.gameObject.SetActive(isSelectable);

        currentStageText.gameObject.SetActive(isCurrentStage);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.audioManager.PlaySfx("Clicks-010");
            onClickCallback?.Invoke(stageIndex);
        });
    }
}