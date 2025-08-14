using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TileInfoUI : MonoBehaviour
{
    [SerializeField] public GameObject panel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image sprite;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Transform statusScrollViewContentRoot;
    [SerializeField] private GameObject statusSlotPrefab;
    [SerializeField] private List<StatusSlot> statusSlotList;

    private Coroutine showRoutine;
    private Coroutine hideRoutine;
    private Action onCompleteCallback;

    public void Show(TileData tileData, float waitBeforeHide = 999f, bool closeButtonActive = true, Action onComplete = null, PlayerStateInStage playerStateInStage = null)
    {
        EnumAssociatedResourceManager enumAssociatedResourceManager = GameManager.Instance.enumAssociatedResourceManager;
        panel?.SetActive(true);
        closeButton?.gameObject.SetActive(closeButtonActive);
        onCompleteCallback = onComplete;

        closeButton?.onClick.RemoveAllListeners();
        closeButton?.onClick.AddListener(Hide);

        string tileName = enumAssociatedResourceManager.GetTileTypeName(tileData.tileType);
        string tileDescription = enumAssociatedResourceManager.GetTileDescription(tileData.tileType);

        nameText?.SetText(tileName);
        descriptionText?.SetText(tileDescription);

        var spriteResource = enumAssociatedResourceManager.GetTileInfoSprite(tileData.tileType);
        sprite.sprite = spriteResource;

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
