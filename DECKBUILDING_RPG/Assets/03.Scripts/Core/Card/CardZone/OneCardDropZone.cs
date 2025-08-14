using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OneCardDropZone : MonoBehaviour, IDropCardZoneUI,IDropHandler,IPointerDownHandler
{
    ReinforceCard cardzone;

    public void Init(ReinforceCard card)
    {
        cardzone = card;
    }
    public void RemoveCard(DragableCardInfoObject card)
    {
        cardzone.ResetCard2(this);
 
    }
    public void OnDrop(PointerEventData eventData)
    {


        if (GameManager.Instance.cardManager.SelectedCard != null)
        {
   
            if (cardzone.GetCard(this) != null)
            {
                if( cardzone.GetCard(this) == GameManager.Instance.cardManager.SelectedCard)
                    return;
                BackCard();
            }
            GameManager.Instance.audioManager.PlaySfx("Snow 03");
            cardzone.SetCard(GameManager.Instance.cardManager.SelectedCard, this);
            var droppedCard = cardzone.GetCard(this) as DragableCardInfoObject;
         
     


            droppedCard.isDroped = true;
            droppedCard.forseStop = true;
         
            droppedCard.transform.SetParent(this.transform,true);
            GameManager.Instance.cardManager.ResetSelectedCard();
            droppedCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(160, 100);
            //droppedCard.ForseStop();
        
        
            droppedCard.GetComponent<RectTransform>().anchoredPosition = new Vector2(160, 100);
        }
    }
    public void BackCard()
    {

       cardzone.ResetCard(cardzone.GetCard(this));
    }

    public void ClickCard(DragableCardInfoObject card)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
     
    }
}
