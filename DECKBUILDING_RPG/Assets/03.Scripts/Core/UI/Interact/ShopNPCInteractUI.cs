using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPCInteractUI : AInteractUI
{
    [SerializeField] private Transform contentRoot;
    //[SerializeField] private CardUIMaker cardUIMakerPrefab;
    [SerializeField] private ShopCardUI shopCardUIPrefab;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button closeButton;

    private List<ShopCardItem> currentShopItems = new List<ShopCardItem>();
    private List<ShopCardUI> shopCardUIs = new List<ShopCardUI>();

    private ACard selectedCard; 
    private Action onComplete;

    private ShopNPC currentShopNPC;
    private CardManager cardManager;

    //private Action clickedCardAction;

    private void Awake()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(OnBuyButtonClicked);
        }
    }

    public override void Enable(Action onCompleteCallback)
    {
        gameObject.SetActive(true);
        onComplete = onCompleteCallback;
    }

    public void OpenShop(List<ShopCardItem> shopItemsToSell, ShopNPC shopNPC, Action onCompleteCallback = null)
    {
        gameObject.SetActive(true);
        onComplete = onCompleteCallback;
        currentShopNPC = shopNPC;

        if (GameManager.Instance != null)
        {
            cardManager = GameManager.Instance.cardManager;
        }


        currentShopItems = new List<ShopCardItem>(shopItemsToSell);

        foreach (ShopCardUI existingShopCardUI in shopCardUIs) 
        {
            if (existingShopCardUI != null)
            {
                existingShopCardUI.OnCardSelected -= selected;
                Destroy(existingShopCardUI.gameObject);
            }
        }

        shopCardUIs.Clear();
        //clickedCardAction = null;

        foreach (ShopCardItem shopItem in currentShopItems)
        {
            if (shopItem.Card == null || shopItem.Quantity <= 0) continue;

            if (shopCardUIPrefab != null)
            {
                ShopCardUI shopCardUIInstance = Instantiate(shopCardUIPrefab, contentRoot);

                shopCardUIInstance.Init(shopItem.Card, shopItem.Card.Price, shopItem.Quantity);

                shopCardUIInstance.OnCardSelected += selected;
                shopCardUIs.Add(shopCardUIInstance);
            }

        }

        selectedCard = null;
        if (buyButton != null)
        {
            buyButton.interactable = false;
        }

    }

    public override void Disable()
    {
        gameObject.SetActive(false);
        selectedCard = null;
        onComplete?.Invoke();
        //clickedCardAction = null;
        foreach (ShopCardUI shopCardUI in shopCardUIs) 
        {
            if (shopCardUI != null)
            {
                shopCardUI.OnCardSelected -= selected;
                Destroy(shopCardUI.gameObject);
            }
        }
        shopCardUIs.Clear();
    }

    public void selected(ACard card)
    {
        Logger.Log($"카드 선택: {card.Name}");
        selectedCard = card;

        if (buyButton != null)
        {
            buyButton.interactable = true;
        }
        foreach (ShopCardUI shopCardUI in shopCardUIs) 
        {
            if (shopCardUI != null)
            {
                shopCardUI.SetSelectionHighlight(shopCardUI.CardData == card);
            }
        }
    }

    public void OnBuyButtonClicked()
    {
        if (selectedCard == null)
        {
            Logger.Log("구매할 카드를 선택해주세요");
            return;
        }

        int cardPrice = selectedCard.Price;

        if (GameManager.Instance != null && GameManager.Instance.gameContext != null &&
            GameManager.Instance.gameContext.player != null)
        {
            if (GameManager.Instance.gameContext.player.playerData.SpendCoin(cardPrice))
            {
                //GameManager.Instance.gameContext.player.cardController.myHand.Add(selectedCard);
                if (GameManager.Instance.gameContext.player.cardController.MyHand.GetCards().Count < 5) 
                {
                    GameManager.Instance.gameContext.player.cardController.MyHand.Add(selectedCard.Data.CloneCardData());
                    Logger.Log($"[{selectedCard.Name}] 구매 성공! 패에 추가되었습니다.");
                }
                else // 손이 가득 찼으면 덱에 추가
                {
                    GameManager.Instance.gameContext.player.cardController.myDeck.Add(selectedCard.Data.CloneCardData());
                    Logger.Log($"[{selectedCard.Name}] 구매 성공! 패가 가득 차 덱에 추가되었습니다.");
                }

                currentShopNPC.OnCardPurchased(selectedCard);
                
                RefreshShopUI();
            }
            else
            {
                Logger.Log("골드가 부족합니다!");
            }
        }


        selectedCard = null;
        if (buyButton != null)
        {
            buyButton.interactable = false;
        }
    }

    private void RefreshShopUI()
    {
        if (currentShopNPC != null)
        {
            OpenShop(currentShopNPC.GetShopItems(), currentShopNPC, onComplete);
        }
    }
    public void OnCloseButtonClicked()
    {
        Disable();
    }

    private void OnDestroy()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(OnCloseButtonClicked);
        }
        if (buyButton != null)
        {
            buyButton.onClick.RemoveListener(OnBuyButtonClicked);
        }

        foreach (ShopCardUI shopCardUI in shopCardUIs) 
        {
            if (shopCardUI != null)
            {
                shopCardUI.OnCardSelected -= selected;
                Destroy(shopCardUI.gameObject);
            }
        }
        shopCardUIs.Clear(); 
    }
}