using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//
public class HoverHighlighterInteract : MonoBehaviour
{
    private Camera mainCamera;
    private StageManager stageManager;
    private GameContext gameContext;
    private PlayerStateInStage playerStateInStage;
    private PlayerData playerData;

    private GameObject currentHovered = null;
    private List<ANPC> highlightedNPCs = new();

    private void Awake()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        gameContext = GameManager.Instance.gameContext;

        var campaignData = gameContext.saveData.campaignData;
        var playerStateInCampaign = campaignData.playerStateInCampaign;
        var stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];

        playerStateInStage = stageData.playerStateInStage;
        playerData = gameContext.saveData.playerData;

        mainCamera = Camera.main;
    }

    public void SetState(HoverHighlightEnum.HoverHighlight hoverHighlight)
    {
        gameObject.SetActive(hoverHighlight == HoverHighlightEnum.HoverHighlight.Interact);
    }

    private void OnEnable()
    {
        RefreshInteractableTargets();
    }

    private void OnDisable()
    {
        ClearCurrentHighlight();
        ClearInteractableHighlights();
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
            if (hit.collider == null)
            {
                continue;
            }

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
            ApplyHoverHighlight(currentHovered);
        }
    }

    private void RefreshInteractableTargets()
    {
        ClearInteractableHighlights();
        var interactableNPCs = stageManager.GetInteractableNPCList();

        foreach (var npc in interactableNPCs)
        {
            npc.HighlightOn();
            highlightedNPCs.Add(npc);
        }
    }

    private void ClearInteractableHighlights()
    {
        foreach (var npc in highlightedNPCs)
        {
            npc.HighlightOff();
        }
        highlightedNPCs.Clear();
    }

    private void ClearCurrentHighlight()
    {
        if (currentHovered == null)
            return;

        if (currentHovered.TryGetComponent<Unit>(out var unit))
        {
            if (highlightedNPCs.Contains(unit as ANPC))
            {
                unit.HighlightOn();
            }
            else
            {
                unit.HighlightOff();
            }
        }

        currentHovered = null;
    }

    private void ApplyHoverHighlight(GameObject obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj.TryGetComponent<Unit>(out var unit))
        {
            HexCoord playerCoord = playerStateInStage.hexCoord;
            HexCoord targetCoord;

            if (unit is ANPC npc)
            {
                targetCoord = npc.npcData.hexCoord;
            }
            else
            {
                return;
            }

            int distance = HexCoord.Distance(playerCoord, targetCoord);
            int range = playerData.interactRange;

            if (distance <= range)
            {
                unit.HighlightTargetPossibleOn();
            }
            else
            {
                unit.HighlightTargetImpossibleOn();
            }
        }
    }
}
