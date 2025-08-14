using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitTypeSlot : MonoBehaviour
{
    [SerializeField] private DetailInfoConnectedText typeNameText;

    private string typeName;

    private EnumAssociatedResourceManager enumAssociatedResourceManager;
    private StageManager stageManager;

    private DetailInfoData detailInfoData = new DetailInfoData();

    private void Start()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        enumAssociatedResourceManager = GameManager.Instance.enumAssociatedResourceManager;
    }

    public void Init(UnitEnum.UnitType unitType)
    {
        detailInfoData.name = enumAssociatedResourceManager.GetUnitTypeName(unitType);
        detailInfoData.description = enumAssociatedResourceManager.GetUnitTypeDescription(unitType);

        typeName = detailInfoData.name;

        typeNameText.Init(detailInfoData);
    }
}
