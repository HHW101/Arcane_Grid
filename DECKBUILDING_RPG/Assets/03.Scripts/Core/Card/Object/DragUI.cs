using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 lastMousePosition;
    [SerializeField]
    ACardShowUI cardShowUI;
    private Vector2 velocity;
    public void OnBeginDrag(PointerEventData eventData)
    {
        cardShowUI.dragging = true;
        lastMousePosition = eventData.position;

    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - lastMousePosition;
        lastMousePosition = eventData.position;


        cardShowUI.content.anchoredPosition += new Vector2(0, delta.y);
        cardShowUI.ClampPosition();
        cardShowUI.velocity = delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cardShowUI.dragging = false;
    }
   

}
