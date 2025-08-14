using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public abstract class BaseCardController : MonoBehaviour
{
    public Deck myDeck;
    public virtual Hand MyHand { get; set; }
    public TrashZone myTrashZone;
    protected ACard selectedCard;
    public ACard SelectedCard {  get { return selectedCard; } }
    protected bool isPlaying = false;
    public bool IsPlaying { get { return isPlaying; } }
   protected bool isStarted = true;
    public virtual void Init(GameObject unit)
    {
        myDeck = unit.AddComponent<Deck>();
        // MyHand = unit.AddComponent<Hand>();
        myTrashZone = unit.AddComponent<TrashZone>();
        
    }
    public void SetSelectedCard(ACard card)
    {
        selectedCard = card;
        GameManager.Instance.cardManager.SelectCard(selectedCard);
    }
    public void UnSelectedCard()
    {
        selectedCard = null;
        GameManager.Instance.cardManager.ResetSelectedCard();
    }
    public virtual bool CanUse(ACard card)
    {
        return true;
    }
    public virtual void DrawCard()
    {
        if(myDeck.GetCards().Count == 0)
            ResetTrashZone();
        CardData card = myDeck.GiveCard();
        if(MyHand.GetCards().Count>8 )
            myTrashZone.Add(card);
        else
            MyHand.Add(card);
    }
    public virtual AGun AddGun()
    {

        AGun gun = new GameObject("Gun").AddComponent<AGun>();
        return gun;
    }
    public virtual void ThrowCard()
    {
      
        int count = MyHand.GetCards().Count;
        for (int i = count; i >= 0; i--)
        {
             myTrashZone.Add(MyHand.GiveCard());

        }
 
    }
    public void ResetDeck()
    {
        isStarted = true;
        int count = MyHand.GetCards().Count;
        for (int i = count; i >= 0; i--)
        {
            myDeck.Add(MyHand.GiveCard());

        }
        count = myTrashZone.GetCards().Count;
        for (int i = count; i >= 0; i--)
        {
            myDeck.Add(myTrashZone.GiveCard());

        }
    
    }
    public virtual void ResetTrashZone()
    {
        while (myTrashZone.GetCards().Count > 0)
        {
            myDeck.Add(myTrashZone.GiveCard());
        }
    }
    public virtual void Use(ACard card, ATile target)
    {
        
    }

}
