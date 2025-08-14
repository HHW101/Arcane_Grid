using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopCardUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI cardCostText;
    [SerializeField] private TextMeshProUGUI cardQuantityText;
    [SerializeField] private CardUIMaker cardUI;
    [SerializeField] private Transform cardDisplayContainer;

    private ACardBaseUI aCardBaseUI;
    private ACard cardData;
    private CanvasGroup cardBaseCanvasGroup;

    public event Action<ACard> OnCardSelected;
    public ACard CardData => cardData;

    private CanvasGroup shopCardUICanvasGroup;

    private void Awake()
    {
        shopCardUICanvasGroup = GetComponent<CanvasGroup>();
        if (shopCardUICanvasGroup == null)
        {
            shopCardUICanvasGroup = gameObject.AddComponent<CanvasGroup>();
            Logger.LogWarning($"[ShopCardUI:{gameObject.name}] CanvasGroup 컴포넌트가 없어 새로 추가했습니다.");
        }

        shopCardUICanvasGroup.interactable = true;
        shopCardUICanvasGroup.blocksRaycasts = true;

        //수량 UI 표시X
        if (cardQuantityText != null)
        {
            cardQuantityText.gameObject.SetActive(false);
        }
    }

    public void Init(ACard card, int price, int quantity)
    {
        cardData = card;

        if (cardUI != null && cardDisplayContainer != null)
        {
            if (aCardBaseUI != null && aCardBaseUI.gameObject.scene.isLoaded)
            {
                SetRaycastTargetOnCardUIRoot(aCardBaseUI.gameObject, true);

                DropCardUI existingDropCardUI = aCardBaseUI.gameObject.GetComponent<DropCardUI>();
                if (existingDropCardUI != null)
                {
                    existingDropCardUI.enabled = true;
                }

                CanvasGroup existingCardRootCanvasGroup = aCardBaseUI.gameObject.GetComponent<CanvasGroup>();
                if (existingCardRootCanvasGroup != null)
                {
                    existingCardRootCanvasGroup.interactable = true;
                    existingCardRootCanvasGroup.blocksRaycasts = true;
                }

                Destroy(aCardBaseUI.gameObject);
                aCardBaseUI = null;
                cardBaseCanvasGroup = null;
            }

            aCardBaseUI = cardUI.makeCardUI(card, null, null, CardEnum.CardUIMode.Drop);

            if (aCardBaseUI != null)
            {
                aCardBaseUI.transform.SetParent(cardDisplayContainer, false);
                aCardBaseUI.transform.localPosition = Vector3.zero;
                aCardBaseUI.transform.localScale = Vector3.one;
                aCardBaseUI.transform.SetAsLastSibling();

                GameObject cardUIRootObject = aCardBaseUI.gameObject;

                cardBaseCanvasGroup = cardUIRootObject.GetComponent<CanvasGroup>();
                cardBaseCanvasGroup.alpha = 1f;
                cardBaseCanvasGroup.interactable = true;
                cardBaseCanvasGroup.blocksRaycasts = true;

                SetRaycastTargetOnCardUIRoot(cardUIRootObject, false);

                DropCardUI dropCardUIComponent = cardUIRootObject.GetComponent<DropCardUI>();
                dropCardUIComponent.enabled = false;
            }

        }

        if (cardCostText != null) cardCostText.text = $"{price}Coin";
        //UpdateQuantity(quantity);
        UpdateVisualState(quantity);

        SetSelectionHighlight(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardData != null)
        {
            OnCardSelected?.Invoke(cardData);
        }
    }

    public void SetSelectionHighlight(bool isSelected)
    {
        if (cardBaseCanvasGroup != null)
        {
            cardBaseCanvasGroup.alpha = isSelected ? 0.7f : 1.0f;
        }
    }

    public void UpdateQuantity(int newQuantity)
    {
        if (cardQuantityText != null)
        {
            cardQuantityText.text = $"x{newQuantity}";
            cardQuantityText.gameObject.SetActive(newQuantity > 0);
        }
       UpdateVisualState(newQuantity);
    }

    public void UpdateVisualState(int currentQuantity)
    {
        bool isSoldOut = currentQuantity <= 0;

        if (shopCardUICanvasGroup != null)
        {
            shopCardUICanvasGroup.interactable = !isSoldOut;
            shopCardUICanvasGroup.blocksRaycasts = !isSoldOut;
        }

        if (cardCostText != null) cardCostText.color = isSoldOut ? Color.gray : Color.black;
        //if (cardQuantityText != null) cardQuantityText.color = isSoldOut ? Color.gray : Color.black;
    }

    private void SetRaycastTargetOnCardUIRoot(GameObject cardUIRootObject, bool state)
    {
        if (cardUIRootObject == null) return;

        Image imageComponent = cardUIRootObject.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.raycastTarget = state;
        }
    }

    private void OnDisable()
    {
        if (aCardBaseUI != null)
        {
            SetRaycastTargetOnCardUIRoot(aCardBaseUI.gameObject, true);

            DropCardUI dropCardUIComponent = aCardBaseUI.gameObject.GetComponent<DropCardUI>();
            if (dropCardUIComponent != null)
            {
                dropCardUIComponent.enabled = true;
            }

            CanvasGroup cardRootCanvasGroup = aCardBaseUI.gameObject.GetComponent<CanvasGroup>();
            if (cardRootCanvasGroup != null)
            {
                cardRootCanvasGroup.interactable = true;
                cardRootCanvasGroup.blocksRaycasts = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (aCardBaseUI != null)
        {
            Destroy(aCardBaseUI.gameObject);
            aCardBaseUI = null;
            cardBaseCanvasGroup = null;
        }
    }
}