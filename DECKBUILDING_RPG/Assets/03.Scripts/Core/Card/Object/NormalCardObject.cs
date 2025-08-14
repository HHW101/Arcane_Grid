using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalCardObject : PlayCardObject
{
    [SerializeField]
    private ArrowCard arrowCard;
    protected ArrowCard arrow;
    [SerializeField]
    private string preName;
    public override void Init(CardTemplate cardTemplate)
    {
        base.Init(cardTemplate);
        if(arrow ==null)
            arrow = Instantiate(arrowCard);
        arrow.gameObject.SetActive(false);
    
    }
    public override void Selected()
    {
        arrow.gameObject.SetActive(true);
        arrow.Init(transform.position, transform.position);
        canvasGroup.alpha = 0.5f;
    }
    public override void Use(ATile receiver, Unit user)
    {
        base.Use(receiver, user);
        if (preName == "CardObject")
        {
            arrow.Shoot(receiver.transform.position);
        }
    }
    public override void Drag(Vector2 mousePos, Camera cam)
    {
        base.Drag(mousePos, cam);
        if (arrowCard == null )
                return;

        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);
        arrow.DrawArrow(worldPos);
    }
    public override void End()
    {
        
        //Destroy(arrow.gameObject);
        base.End();
        arrow.RealEnd();
    }
    public override void EndCard()
    {
      
        GameManager.Instance.cardManager.PoolBackCard(this, preName);
        //Destroy(arrow.gameObject);
        arrow.RealEnd();
    }
    public override void UnSelected()
    {
        arrow.End();
        //cardArrow = null;
        canvasGroup.alpha = 1.0f;
    }
}
