using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableCardInfoObject : CardInfoObject, IDragHandler,IPointerDownHandler, IPointerUpHandler
{
    public Vector2 orginalPos;
    public bool isDroped;
    public Transform parent;

    public Canvas canvas;
    public IDropCardZoneUI show;
    protected bool canClicked = true;
    public bool CanClicked {  get { return canClicked; } }

    public bool isSelected=false;
    
    protected override void OnEnable()
    {
        base.OnEnable();
       
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

    

    }
    public void OnDrag(PointerEventData eventData)
    {
        
        if (!canClicked)
            return;
        transform.position = eventData.position;
        rect.pivot = new Vector2(0.5f, 0.5f);
       
   
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
    
        if(!canClicked) 
            return;
    if (canClicked)
        GameManager.Instance.cardManager.SelectCard(this);
        isDroped = false;
        orginalPos = rect.anchoredPosition;
        Canvas can = GetComponentInParent<Canvas>();

        parent = this.transform.parent;
       show =  GetComponentInParent<IDropCardZoneUI>();
        show.ClickCard(this);
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;

    }
    
    

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canClicked)
            return;
        if (isSelected)
        {
            return;
        }
        rect.pivot = new Vector2(0.0f, 1.0f);
        StartCoroutine(ResetPostion());
      

    }
    IEnumerator ResetPostion()
    {
        canClicked = false;
        yield return new WaitForSeconds(0.1f);
        canvasGroup.blocksRaycasts = true;
        if (!isDroped)
        {
            GameManager.Instance.cardManager.ResetSelectedCard();
            transform.SetParent(parent);
            MoveCardRect(orginalPos,false);
           
            yield return new WaitForSeconds(2.0f);
            
           
        }
        else
        {
            show.RemoveCard(this);
        }
        canClicked = true;
    }
    
}
