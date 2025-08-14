using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PackCardUI : ACardBaseUI
{

    protected override void Click(PointerEventData eventData)
    {
        zoneUI.CardDescriptionUI.show(card);
        isDrag = true;
        rect.SetAsLastSibling();
        transform.SetParent(canvas.transform, true);
        canvasGroup.blocksRaycasts = false;
    }
    protected override void ClickEnd(PointerEventData eventData)
    {
        if (!isDrag) return;

        isDrag = false;
        canvasGroup.blocksRaycasts = true;
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };


        EventSystem.current.RaycastAll(pointerData, results);

        bool droppedOnUI = false;

        foreach (RaycastResult result in results)
        {
 
            var dropZone = result.gameObject.GetComponent<IDropZoneUI>();
            if (dropZone != null)
            {
                dropZone.OnDrop(card);
                zone.Remove(card.Data);
                zone.ShowZone();
                droppedOnUI = true;
                Debug.Log("드롭 성공: " + result.gameObject.name);
                Destroy(this);
               
                break;
            }
        }

        if (!droppedOnUI)
        {
            zone.ShowZone();
        }

    }
}
