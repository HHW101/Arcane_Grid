using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatusSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private DetailInfoConnectedText statusNameText;
    [SerializeField] private TMP_Text remainTurnText;

    private string statusName;
    private string statusDescripion;

    private StatusEffectBook statusEffectBook;
    private StatusEffectEntry statusEffectEntry;
    private StageManager stageManager;

    private DetailInfoData detailInfoData = new DetailInfoData();

    private void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
    }

    public void Init(StatusEffectInstance statusEffect)
    {
        statusEffectBook = GameManager.Instance.campaignManager.stageManager.turnManager.statusEffectBook;
        statusEffectEntry = statusEffectBook.GetEntry(statusEffect.effectType);
        stageManager = GameManager.Instance.campaignManager.stageManager;

        icon.sprite = statusEffectEntry.icon;
        statusName = statusEffectEntry.effectName;
        statusDescripion = statusEffectEntry.description;

        detailInfoData.name = statusName;
        detailInfoData.description = statusDescripion;

        statusNameText.Init(detailInfoData);
        remainTurnText.SetText($"{statusEffect.remainingTurn}턴");
    }
}
