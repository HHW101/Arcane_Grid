using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class HoverHighlighterMove : MonoBehaviour
{
    private Camera mainCamera;
    private StageManager stageManager;
    private PlayerStateInStage playerStateInStage;
    private PlayerData playerData;

    private List<ATile> highlightedTiles = new();
    private HexCoord lastCoord;

    private ATile lastHoveredTile;
    private List<ATile> lastPathTiles = new();
    private GameContext gameContext;

    [SerializeField]
    private Sprite reachableTileMarkerSprite;

    [SerializeField]
    private Sprite reachablePathMarkerSprite;
    [SerializeField]
    private Sprite unreachablePathMarkerSprite;

    [SerializeField]
    private Sprite reachablePathDestMarkerSprite;
    [SerializeField]
    private Sprite unreachablePathDestMarkerSprite;

    [SerializeField]
    private Sprite noneSprite;

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
        gameObject.SetActive(hoverHighlight == HoverHighlightEnum.HoverHighlight.Move);
    }

    private void OnEnable()
    {
        RefreshReachableTiles();
    }

    private void OnDisable()
    {
        ClearHoverHighlight();
        ClearReachableTiles();
    }

    private void Update()
    {
        if (!playerStateInStage.hexCoord.Equals(lastCoord))
        {
            RefreshReachableTiles();
        }
        HandleTileHover();
    }

    private void HandleTileHover()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            ClearHoverHighlight();
            return;
        }

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        GameObject highestObj = null;
        int highestOrder = int.MinValue;
        ATile targetTile = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;

            var renderer = hit.collider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;

                if (highestObj.TryGetComponent<ATile>(out var foundTile))
                {
                    targetTile = foundTile;
                }
            }
        }

        if (targetTile == null)
        {
            ClearHoverHighlight();
            return;
        }

        if (targetTile == lastHoveredTile)
            return;

        ClearHoverHighlight();
        lastHoveredTile = targetTile;

        var path = stageManager.FindShortestPathAndRecordInGameContext(targetTile.tileData.hexCoord, playerData);
        int maxDistance = playerData.actionPoint / playerData.actionPointPerMove;
        var startTile = stageManager.FindTileByCoord(playerStateInStage.hexCoord);

        if (path.Count > 0)
        {
            ATile prevTileBeforeDest = startTile;
            for (int i = 0; i < path.Count - 1; i++)
            {
                if (path.Count <= maxDistance)
                {
                    if (i == 0)
                    {
                        path[0].ShowPathMarker(startTile, path[1], true);
                    }
                    else
                    {
                        path[i].ShowPathMarker(path[i - 1], path[i + 1], true);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        path[0].ShowPathMarker(startTile, path[1], false);
                    }
                    else
                    {
                        path[i].ShowPathMarker(path[i - 1], path[i + 1], false);
                    }
                }
                lastPathTiles.Add(path[i]);
                prevTileBeforeDest = path[i];
            }
            if (path.Count <= maxDistance)
            {
                path[path.Count - 1].SetIsReachableDestMarkerSprite(reachablePathDestMarkerSprite, prevTileBeforeDest);
            }
            else
            {
                path[path.Count - 1].SetIsReachableDestMarkerSprite(unreachablePathDestMarkerSprite, prevTileBeforeDest);
            }
            lastPathTiles.Add(path[path.Count - 1]);
        }
    }

    private void ClearHoverHighlight()
    {
        foreach (var tile in lastPathTiles)
        {
            if (gameContext.lastSearchReachableTile.Contains(tile))
            {
                tile.SetIsReachableMarkerSprite(reachableTileMarkerSprite);
                tile.HidePathMarker();
            }
            else
            {
                tile.SetIsReachableMarkerSprite(noneSprite);
                tile.HidePathMarker();
            }
        }
        lastPathTiles.Clear();

        lastHoveredTile = null;
    }

    private void RefreshReachableTiles()
    {
        ClearReachableTiles();

        var reachableTiles = stageManager.GetReachableTileList();
        gameContext.lastSearchReachableTile = reachableTiles;

        foreach (var tile in reachableTiles)
        {
            tile.SetIsReachableMarkerSprite(reachableTileMarkerSprite);
            highlightedTiles.Add(tile);
        }

        lastCoord = playerStateInStage.hexCoord;
    }

    private void ClearReachableTiles()
    {
        foreach (var tile in highlightedTiles)
        {
            tile.SetIsReachableMarkerSprite(noneSprite);
            tile.HidePathMarker();
        }
        highlightedTiles.Clear();
    }
}
