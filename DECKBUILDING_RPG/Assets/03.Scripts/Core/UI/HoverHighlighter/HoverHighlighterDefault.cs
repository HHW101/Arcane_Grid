using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHighlighterDefault : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject currentHovered = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void SetState(HoverHighlightEnum.HoverHighlight hoverHighlight)
    {
        gameObject.SetActive(hoverHighlight == HoverHighlightEnum.HoverHighlight.Default);
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (HpUI.Instance != null && !HpUI.IsPointerOverSelf())
            {
                ClearCurrentHighlight();
                return;
            }
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        GameObject highestObj = null;
        int highestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            SpriteRenderer renderer = hit.collider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;
            }
        }

        if (highestObj != currentHovered)
        {
            ClearCurrentHighlight();
            currentHovered = highestObj;
            ApplyHighlight(currentHovered);
        }
    }

    void ClearCurrentHighlight()
    {
        if (currentHovered == null) return;

        if (currentHovered.TryGetComponent<ATile>(out var tile))
        {
            tile.HighlightOff();
        }
        if (currentHovered.TryGetComponent<Unit>(out var unit))
        {
            unit.HighlightOff();
        }
        currentHovered = null;
    }

    void ApplyHighlight(GameObject obj)
    {
        if (obj == null) return;

        if (obj.TryGetComponent<ATile>(out var tile))
        {
            tile.HighlightOn();
        }
        if (obj.TryGetComponent<Unit>(out var unit))
        {
            unit.HighlightOn();
        }
    }

    private void OnDisable()
    {
        ClearCurrentHighlight();
    }
}
