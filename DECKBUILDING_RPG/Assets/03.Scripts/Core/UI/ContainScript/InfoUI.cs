using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoUI : MonoBehaviour
{
    [SerializeField] private KeyCode _keyCode;
    [SerializeField] private UnitInfoUI unitInfoUI;
    [SerializeField] private TileInfoUI tileInfoUI;
    public KeyCode keyCode => _keyCode;

    private Action onCompleteCallback;

    private void Start()
    {
        UIManager uiManager = GameManager.Instance.uiManager;
        uiManager.RegisterInfoUI(this);

        unitInfoUI.Hide();
        tileInfoUI.Hide();
    }

    public void Init()
    {
        unitInfoUI.Hide();
        tileInfoUI.Hide();
    }

    private void Update()
    {
        if (unitInfoUI.panel.gameObject.activeSelf || tileInfoUI.panel.gameObject.activeSelf)
        {
            return;
        }

        if (Input.GetKeyDown(keyCode))
        {
            var hovered = GetHoveredShowInfoable();
            if (hovered != null)
            {
                Show(hovered, closeButtonActive: true);
            }
        }
    }

    public void Show(IShowInfoable showInfoable, float waitBeforeHide = 999f, bool closeButtonActive = true, Action onComplete = null)
    {
        onCompleteCallback = onComplete;

        if (showInfoable is Unit unit)
        {
            tileInfoUI.Hide();
            if (unit is APlayer player)
            {
                unitInfoUI.Show(unit.unitData, waitBeforeHide, closeButtonActive, onComplete, player.playerStateInStage);
            }
            else
            {
                unitInfoUI.Show(unit.unitData, waitBeforeHide, closeButtonActive, onComplete);
            }
        }
        else if (showInfoable is ATile tile)
        {
            unitInfoUI.Hide();
            tileInfoUI.Show(tile.tileData, waitBeforeHide, closeButtonActive, onComplete);
        }
        else
        {
            Debug.LogWarning($"[InfoUI] Unknown IShowInfoable type: {showInfoable.GetType().Name}");
        }
    }

    private IShowInfoable GetHoveredShowInfoable()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return null;

        Camera mainCamera = Camera.main;
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

        if (highestObj != null && highestObj.TryGetComponent<IShowInfoable>(out var info))
        {
            return info;
        }

        return null;
    }
}
