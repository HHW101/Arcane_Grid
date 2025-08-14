using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardSetData
{
    public List<CardData> deck;
    public List<CardData> pack;
    public List<CardData> trashZone;
    public List<CardData> hand;
    public List<CardData> gun;
    public CardSetData CloneCardSetData()
    {
        foreach (CardData cardData in deck) {
            cardData.LoadCard();
        }
        foreach (CardData cardData in pack)
            {  cardData.LoadCard(); }
        return new CardSetData
        {
            deck = this.deck != null ? new List<CardData>(this.deck) : new List<CardData>(),
            pack = this.pack != null ? new List<CardData>(this.pack) : new List<CardData>(),
            trashZone = this.trashZone != null ? new List<CardData>(this.trashZone) : new List<CardData>(),
            hand = this.hand != null ? new List<CardData>(this.hand) : new List<CardData>(),
            gun = this.gun != null ? new List<CardData>(this.gun) : new List<CardData>()
        };
    }
}
