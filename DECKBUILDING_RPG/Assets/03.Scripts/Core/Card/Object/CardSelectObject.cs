using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectObject : CardInfoObject, IPointerDownHandler
{
    protected override void OnEnable()
    {
        base.OnEnable();
        if(GameManager.Instance.cardManager != null)
        GameManager.Instance.cardManager.OnSelected.AddListener(UnSelected);
    }
    protected override void OnDisable()
    {
        canvasGroup.alpha = 1.0f;
        if (GameManager.Instance.cardManager!= null)
            GameManager.Instance.cardManager.OnSelected.RemoveListener(UnSelected);
        base.OnDisable();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Selected();
    }
    public override void Selected()
    {
        base.Selected();
        GameManager.Instance.cardManager.SelectCard(this);
        canvasGroup.alpha = 0.5f;
    }
    public override void UnSelected()
    {
        base.Selected();
        if(GameManager.Instance.cardManager.SelectedCard!=this)
        canvasGroup.alpha = 1.0f;
    }
}
