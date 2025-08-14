using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class CardShowDragAble : ACardShowUI,IDropHandler,IDropCardZoneUI
{
    public CardInfoObject selectedCard;
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;
    public bool isMakingDeck=false;

    [SerializeField]
    Canvas can;
    [SerializeField]
    Image backGround;
    [SerializeField]
    TextMeshProUGUI num;
    public virtual void OnDrop(PointerEventData eventData)
    {
        if(cardManager.SelectedCard != null)
        {
            
            DragableCardInfoObject card = cardManager.SelectedCard as DragableCardInfoObject;
            if (card.show==this)
            {
                GameManager.Instance.audioManager.PlaySfx("Snow 03"); //효과음재생
                return;
            }

            GameManager.Instance.audioManager.PlaySfx("Snow 03");//효과음재생

            card.isDroped = true;
            card.transform.SetParent(content.GetComponent<RectTransform>(), true);
            
            AddCard(card, FindInsertIndex(eventData.position));
            cardManager.ResetSelectedCard();
        }

    }
    public void ClickCard(DragableCardInfoObject card)
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-001");//효과음재생
        card.canvas = can;
    }
    public void RemoveCard(DragableCardInfoObject card)
    {
        

        
        zone.Remove(card.Data);
        cards.Remove(card);
        if (cards.Count != zone.GetCards().Count)
            zone.ForseReset(cards);
        SetCardPos();
        ShowNum();
    }
    public int FindInsertIndex(Vector2 pos)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content, 
            pos,
            null,
            out localPoint);
       
       
          int x =  (int)(localPoint.x / (itemWidth + spacingX));
       
        int y = -(int)(localPoint.y / (itemHeight + spacingY));
        int index = y * columns + x;

        index = Mathf.Min(index, cards.Count);
        return index; 
    }
    public void ShowNum()
    {
        if (!isMakingDeck)
            return;
        if(!cardManager.isDeckCanUse())
             backGround.color = Color.red;
        else backGround.color = Color.white;
        num.text = $"{cardManager.MinDeckNum}/{cards.Count} ";
    }
    public void AddCard(DragableCardInfoObject card,int index)
    {
        
        zone.Add(card.Data);

  
        cards.Insert(index, card);
        SetCardPos();
        ShowNum();
    }
    public void AddCard2(DragableCardInfoObject card)
    {

        zone.Add(card.Data);


        cards.Insert(cards.Count, card);
       // card.MoveCard(FindPosition(cards.Count));
        SetCardPos2();
        ShowNum();
    }
    public void AddCard3(DragableCardInfoObject card)
    {

        zone.Add(card.Data);


     
    }
    public void AddCard(DragableCardInfoObject card)
    {
        AddCard(card, cards.Count);
    }
 
    public override void SetCard()
    {
     
        columns = Mathf.FloorToInt((viewport.rect.width + spacingX) / (itemWidth + spacingX));
        CardInfoObject[] cardss = content.GetComponents<CardInfoObject>();
        foreach (CardInfoObject card in cardss) { 
            card.EndCard();
        }
        for (int i = 0; i < zone.GetCards().Count; i++)
        {
            DragableCardInfoObject cardInfo = cardManager.GetDragCardInfoObjectByCardData(zone.GetCards()[i]);
            RectTransform cardRect = cardInfo.GetComponent<RectTransform>();
            RectTransform contentRect = content.GetComponent<RectTransform>();
            cardRect.SetParent(contentRect, false);
            cardRect.anchoredPosition = FindPosition(i);
            cards.Add(cardInfo);
        }
    

    }
    protected override void Update()
    {

         base.Update();
     
            
          
        
       
    }


}
