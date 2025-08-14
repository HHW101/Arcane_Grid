using UnityEngine;
using UnityEngine.EventSystems;
public class ScrollViewWheelBlocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool IsPointerOverScrollView { get; private set; }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOverScrollView = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOverScrollView = false;
    }
}