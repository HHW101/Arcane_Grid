using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayCardObject : ACardObject, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    CardInfoObject cardInfo;
    [SerializeField]
    protected float scaleValue = 1.2f;
    private Vector3 orginalScale;
    public bool NotDragable = false;
    protected override void OnEnable()
    {
        base.OnEnable();
        
        cardInfo.gameObject.SetActive(false);
    }
    public override void InitAfterSetParent()
    {
        base.InitAfterSetParent();
        orginalScale = transform.localScale;
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {

        //this.gameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        if(GameManager.Instance.cardManager.SelectedCard!=null)
            return;
        //if(CanUse(GameManager.Instance.gameContext.player))
          //  return;

   
        ShowDetail();
        if (NotDragable)
            return;
        StartCoroutine(ChangeScale(true));

    }
    public override void Selected()
    {

        base.Selected();
        UnShowDetail();

    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        UnShowDetail();
        if (NotDragable)
            return;
        //this.gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        StartCoroutine(ChangeScale(false));
    
   
    }
    protected virtual void ShowDetail()
    {
        cardInfo.gameObject.SetActive(true);
        cardInfo.InfoInit(img,Name,makeShortDescription(),cost,value);
        
    }
    public override void UnSelected()
    {
        
        base.UnSelected();
    }
   
    protected virtual void UnShowDetail()
    {
        cardInfo.gameObject.SetActive(false);
    }
    IEnumerator ChangeScale(bool isBig)
    {
        float t = 0;
        Vector3 nowScale = transform.localScale;
        Vector3 targetScale = isBig
         ? orginalScale * scaleValue
         : orginalScale;


        while (t < 0.05f)
        {
            t += StageTime.Instance.deltaTime;
            transform.localScale = Vector3.Lerp(nowScale, targetScale, t / 0.05f);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
