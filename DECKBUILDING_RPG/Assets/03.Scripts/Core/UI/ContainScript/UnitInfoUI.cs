using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoUI : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image sprite;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TextFlowLayout typeText;
    [SerializeField] private Transform statusScrollViewContentRoot;
    [SerializeField] private GameObject statusSlotPrefab;
    [SerializeField] private List<StatusSlot> statusSlotList;
    [SerializeField] private List<UnitTypeSlot> unitTypeSlotList;

    private Coroutine showRoutine;
    private Coroutine hideRoutine;
    private Action onCompleteCallback;

    public void Show(UnitData unitData, float waitBeforeHide = 999f, bool closeButtonActive = true, Action onComplete = null, PlayerStateInStage playerStateInStage = null)
    {
        panel?.SetActive(true);
        closeButton?.gameObject.SetActive(closeButtonActive);
        onCompleteCallback = onComplete;

        closeButton?.onClick.RemoveAllListeners();
        closeButton?.onClick.AddListener(Hide);

        EnumAssociatedResourceManager enumAssociatedResourceManager = GameManager.Instance.enumAssociatedResourceManager;

        string unitName = enumAssociatedResourceManager.GetUnitIdentifierName(unitData.unitIdentifierType);
        string unitDescription = enumAssociatedResourceManager.GetUnitIdentifierDescription(unitData.unitIdentifierType);

        typeText.ClearAllTexts();
        for (int i = 0; i < unitData.unitTypeList.Count; i++)
        {
            string typeName = enumAssociatedResourceManager.GetUnitTypeName(unitData.unitTypeList[i]);
            string typeDescription = enumAssociatedResourceManager.GetUnitTypeDescription(unitData.unitTypeList[i]);
            DetailInfoData detailInfoData = new DetailInfoData();
            detailInfoData.name = typeName;
            detailInfoData.description = typeDescription;
            if (i < unitData.unitTypeList.Count - 1)
            {
                detailInfoData.name += ",";
            }
            typeText.AddDetailInfoConnectedText(detailInfoData);
        }

        nameText?.SetText(unitName);
        descriptionText?.SetText(unitDescription);

        var spriteResource = GameManager.Instance.enumAssociatedResourceManager
            .GetInfoSprite(unitData.unitIdentifierType);
        sprite.sprite = spriteResource;

        float currentHp = 0;
        float maxHp = 0;

        if (unitData is PlayerData playerData)
        {
            currentHp = playerData.currentHealth;
            maxHp = playerData.maxHealth;
            SetStatusList(playerStateInStage.statusEffectList);
        }
        else if (unitData is NPCData npcData)
        {
            currentHp = npcData.currentHealth;
            maxHp = npcData.maxHealth;
            SetStatusList(npcData.statusEffectList);
        }

        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }
        showRoutine = StartCoroutine(ShowRoutine(waitBeforeHide, closeButtonActive));
    }

    private IEnumerator ShowRoutine(float waitTime, bool isCloseButtonActive)
    {
        if (!isCloseButtonActive)
        {
            yield return new WaitForSeconds(waitTime);
            Hide();
        }
    }

    public void Hide()
    {
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideRoutine());
    }

    private IEnumerator HideRoutine()
    {
        panel?.SetActive(false);
        onCompleteCallback?.Invoke();
        onCompleteCallback = null;
        yield break;
    }

    private void SetStatusList(List<StatusEffectInstance> statusEffects)
    {
        foreach (var slot in statusSlotList)
        {
            Destroy(slot.gameObject);
        }
        statusSlotList.Clear();

        foreach (var effect in statusEffects)
        {
            var go = Instantiate(statusSlotPrefab, statusScrollViewContentRoot);
            var slot = go.GetComponent<StatusSlot>();
            slot.Init(effect);
            statusSlotList.Add(slot);
        }
    }
}
