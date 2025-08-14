using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DetailInfoUI : MonoBehaviour
{
    [SerializeField] private DetailInfoUIPanel detailInfoUIPanel;
    [SerializeField] private Image panelBackground;
    [SerializeField] private Image getDetailInfo;
    [SerializeField] private RectTransform getDetailInfoRect;

    private void Start()
    {
        UIManager uiManager = GameManager.Instance.uiManager;
        uiManager.RegisterDetailInfoUI(this);
        Init();
    }

    public void Init()
    {
        Hide();
        HideGetDetailInfo();
    }

    private void Update()
    {
        if (detailInfoUIPanel.gameObject.activeSelf)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Hide();
            }
            return;
        }
    }

    public bool IsHide()
    {
        return !getDetailInfo.gameObject.activeSelf;
    }

    public void Show(IShowDetailInfoable showDetailInfoable)
    {
        panelBackground.gameObject.SetActive(true);
        detailInfoUIPanel.gameObject.SetActive(true);

        var panelRect = detailInfoUIPanel.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        Vector2 mouse = Input.mousePosition;
        var canvas = panelRect.GetComponentInParent<Canvas>();
        var canvasRect = canvas != null ? canvas.GetComponent<RectTransform>() : null;

        float scale = (canvas != null) ? canvas.scaleFactor : 1f;
        Vector2 size = panelRect.rect.size * scale;
        Vector2 pivot = panelRect.pivot;
        Vector2 offset = new Vector2(16f, 16f);

        Vector2 pos = mouse + offset;
        float left = pos.x - size.x * pivot.x;
        float right = pos.x + size.x * (1f - pivot.x);
        float bottom = pos.y - size.y * pivot.y;
        float top = pos.y + size.y * (1f - pivot.y);

        if (right > Screen.width) pos.x -= (right - Screen.width);
        if (top > Screen.height) pos.y -= (top - Screen.height);
        if (left < 0f) pos.x += -left;
        if (bottom < 0f) pos.y += -bottom;

        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            panelRect.position = pos;
        }
        else if (canvasRect != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, pos, canvas ? canvas.worldCamera : null, out var local);
            panelRect.anchoredPosition = local;
        }
        else
        {
            panelRect.position = pos;
        }

        detailInfoUIPanel.Init(showDetailInfoable.GetDataForDetailInfoUI());
    }

    public void ShowGetDetailInfo()
    {
        Vector3 mouseScreenPos = Input.mousePosition;

        float offsetX = getDetailInfoRect.rect.width * 0.6f;
        float offsetY = getDetailInfoRect.rect.height * 0.6f;
        Vector3 offset = new Vector3(offsetX, offsetY, 0f);

        getDetailInfo.gameObject.SetActive(true);
        getDetailInfo.transform.position = mouseScreenPos + offset;
    }

    public void HideGetDetailInfo()
    {
        getDetailInfo.gameObject.SetActive(false);
    }

    public void Hide()
    {
        panelBackground.gameObject.SetActive(false);
        detailInfoUIPanel.gameObject.SetActive(false);
    }
}
