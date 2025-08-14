using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class DeckUI : AZoneBaseUI,IDropZoneUI
{
    //[SerializeField]
    private Deck deck;

   
  
  
   public void Init(Deck _deck)
    {
        this.deck = _deck;
        deck.show += Show;

    }
   
    private void OnDisable()
    {
        deck.show -= Show;

    }

    public override void Show()
    {

        Reset();
        foreach (CardData cardData in deck.GetCards())
        {
            ACard card = GameManager.Instance.cardManager.GetCardByCardData<ACard>(cardData.CloneCardData());
            CardUIMaker ui = Instantiate(CardUIPrefab, this.transform, true);

           ACardBaseUI uibase= ui.makeCardUI(card,deck,this, CardEnum.CardUIMode.Deck);
            cards.Add(uibase);

        }
    }
    protected override void ChangeMode()
    {
        base.ChangeMode();
        deck.Sort(sortMode);
    }

    public override void OnDrop(ACard card)
    {
        deck.Add(card.Data);
        Show();
       
    }
    public override void SaveZone(ACard card)
    {
        deck.Save();
    }
}
