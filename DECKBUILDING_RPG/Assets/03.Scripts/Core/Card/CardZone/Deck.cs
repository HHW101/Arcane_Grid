using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : ACardZone
{
    // Start is called before the first frame update


    public void Init()
    {
        GameManager.Instance.cardManager.LoadDeck(this);

        show?.Invoke();
    }
   
    public override void Save()
    {
        
        GameManager.Instance.cardManager.SaveDeck(cards);
    }
    public override void ShowZone()
    {
        base.ShowZone();
        show?.Invoke();
    }
    public override CardData GiveCard()
    {
       show?.Invoke();
        return base.GiveCard();
    }


}
