using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterCorpseInteractUI : AInteractUI
{
    private Action onComplete;
    private CardManager cardManager;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button rootButton;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private CardUIMaker cardRootButtonPrefab;
    private ACard selectedCard;
    Action pushed;
    private List<ACard> cardList = new List<ACard>();

    public void Start()
    {
        closeButton?.onClick.AddListener(OnCloseButtonClicked);
        rootButton?.onClick.AddListener(OnRootButtonClicked);
    }

    public override void Disable()
    {
        this.gameObject.SetActive(false);
        this.onComplete?.Invoke();
        pushed = null;
    }

    public override void Enable(Action onComplete)
    {
        this.gameObject.SetActive(true);
        this.onComplete = onComplete;
        cardManager = GameManager.Instance.cardManager;

   
        foreach (ACard child in cardList)
        {
            GameManager.Instance.poolManager.ReturnObject("CardImageSelect", child.gameObject);
        }
        cardList.Clear();
        for (int i = 0; i < npcData.droppedCardIDList.Count; i++)
        {
            CardSelectObject card = cardManager.GetCardSelectInfoObjectByID(npcData.droppedCardIDList[i]);
            card.transform.SetParent(contentRoot);
            if (card == null) continue;

            cardList.Add(card);

            
        }
    }
    public void selected(ACard card)
    {
      
        selectedCard = card;
        pushed?.Invoke();
        
    }

    public void OnCloseButtonClicked()
    {

        Disable();
    }

    public void OnRootButtonClicked()
    {
        if (GameManager.Instance.cardManager.SelectedCard != null)
            GameManager.Instance.gameContext.player.cardController.myDeck.Add(GameManager.Instance.cardManager.SelectedCard.Data.CloneCardData());
        GameManager.Instance.cardManager.ResetSelectedCard();
        Disable();
    }
}