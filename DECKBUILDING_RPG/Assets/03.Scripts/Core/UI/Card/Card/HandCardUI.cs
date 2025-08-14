using CardEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCardUI : ACardBaseUI
{
    protected override void Click(PointerEventData eventData)
    {
        Hand hand = zone as Hand;
        if (hand == null)
            return;
        if (!cardController.CanUse(card))
            return;
        Vector2 spawnPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
        Input.mousePosition,
            canvas.worldCamera,
            out spawnPos);

        dragUI = Instantiate(DragPre, canvas.transform);
        dragUI.GetComponent<RectTransform>().anchoredPosition = spawnPos;
        rect = dragUI.GetComponent<RectTransform>();
        dragUI.transform.SetAsLastSibling();
        dragUI.Init(this);
        Selected();
    }
    protected override void ClickEnd(PointerEventData eventData)
    {
        if (dragUI == null)
            return;
        canvasGroup.blocksRaycasts = true;

        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);
        if (mode == CardUIMode.Hand)
            hitCollider = Physics2D.OverlapPoint(worldPoint, layermask);

        if (hitCollider != null)
        {
           
            var target = hitCollider.GetComponent<ATile>();
         
            if (target != null)
            {
                if (mode == CardUIMode.Hand)
                {
                    if (target.IsTargetable(card))
                    {
                        Use(target);
                        Destroy(dragUI.gameObject);
                        UnSelected();
                        return;
                    }
                }
                else
                    Use(target);
            }

        }
        if (mode == CardUIMode.Hand)
            Destroy(dragUI.gameObject);

        UnSelected();
    }

    public void Use(ATile target)
    {
        Hand hand = zone as Hand;
        if (hand == null)
            return;

        cardController.Use(card, target);
    }
}
