using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : ACardZone
{

    public int nowNum = 5;



    public override CardData GiveCard(CardData card)
    {
        CardData cardT= base.GiveCard(card);
        show?.Invoke();
        return cardT;
    }
    public override void Add(CardData card)
    {
        if(cards.Count<=5)
        base.Add(card);
        show?.Invoke();
    }
    public override void Save()
    {

        GameManager.Instance.cardManager.SaveHand(cards);
    }
    public override CardData GiveCard()
    {

        CardData card= base.GiveCard();
        show?.Invoke();
        return card;
    }
   
    public override void ShowZone()
    {
        base.ShowZone();
        show?.Invoke();
    }
 
    
 
 
}
