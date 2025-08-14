using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CardEnum;
using System.Linq;

public abstract class ACardZone : MonoBehaviour
{
    public Action show;
    [SerializeField]
    protected List<CardData> cards = new List<CardData>();  
    protected BaseCardController controller;
    public virtual void Init(BaseCardController cardController)
    {
        controller = cardController;
    }
    public virtual void LoadCards(List<CardData> cards)
    {
        this.cards = new List<CardData>(cards);
    }
    public virtual void Add(CardData card)
    {
        if (card == null)
            return;
        cards.Add(card);
       
        //card.transform.SetParent(transform);
        Save();

    }
    public virtual void Sort(CardEnum.SortMode sortMode)
    {
        switch (sortMode)
        {
            case CardEnum.SortMode.Cost:
                CostSort();
                break;
            case CardEnum.SortMode.Value:
                ValueSort();
                break;
            case CardEnum.SortMode.Range:
                RangeSort();
                break;

        }
        show?.Invoke();
    }
    protected void CostSort()
    {
        //cards = cards.OrderBy(card => card.Cost).ToList();
    }
    protected void RangeSort()
    {
        //cards = cards.OrderBy(card => card.Range).ToList();
    }
    protected virtual void ValueSort()
    {
        //cards = cards.OrderBy(card => card.Value).ToList();
    }
    public virtual void ShowZone()
    {
        Save();
    }
    public virtual CardData GiveCard()
    {
        if(cards.Count ==0)
            return null;
        CardData card = cards[0];
        cards.Remove(cards[0]);
        Save();
        return card;
    }
    public virtual void Save()
    {

    }
    public virtual CardData GiveCard(CardData card)
    {
        if(cards.Contains(card))
            Remove(card);
    
        return card;
    }
    public virtual void Shuffle()
    {

        for (int i = 0; i < cards.Count; i++)
        {
            int randomIdx = UnityEngine.Random.Range(0, cards.Count);
            CardData card = cards[i];
            cards[i] = cards[randomIdx];
            cards[randomIdx] = card;
        }
        Save();
        show?.Invoke();
    }
    public virtual void Remove(CardData card) { cards.Remove(card); Save(); }
    public virtual IReadOnlyList<CardData> GetCards() { return cards; 
    }
    public virtual void ForseReset(List<CardInfoObject> Acards)
    {
        cards.Clear();
        foreach (ACard card in Acards)
        {
            cards.Add(card.Data);
        }
    }
    public virtual void ForseReset(List<ACardObject> Acards)
    {
        cards.Clear();
        foreach (ACard card in Acards)
        {
            cards.Add(card.Data);
        }
    }
    public virtual CardData SelectCard(int index)
    {
        return cards[index];
    }
}
