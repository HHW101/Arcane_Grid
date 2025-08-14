using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACardComboSystem : MonoBehaviour
{
    List<ACard> cards;
    ACard baseCard;

    public void Init(ACard card)
    {
        cards = new List<ACard>();
        cards.Add(card);
        baseCard = card;
    }
    public bool CanAdd(ACard card)
    {
        if(baseCard.Cost < card.Cost) 
            return false;
        return true;
    }
    public void Add(ACard card)
    {
        cards.Add(card);
    }

}
