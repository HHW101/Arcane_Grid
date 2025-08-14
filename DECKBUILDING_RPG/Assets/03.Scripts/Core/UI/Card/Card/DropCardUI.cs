using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropCardUI : ACardBaseUI
{
    Action<ACard> Pushed;
   
    protected override void Click(PointerEventData eventData)
    {
        base.Click(eventData);
        //GameManager.Instance.gameContext.player.cardController.myDeck.Add(card);
        Pushed?.Invoke(card);
        //Destroy(this.gameObject);
        Selected();
    }
    public void OnDisable()
    {
        Pushed = null;
    }
    public Action AddSelected (Action<ACard> action)
    {
        Pushed += action;
        return UnPushed;
    }
    public void UnPushed()
    {
        UnSelected();
    }
}
