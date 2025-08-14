using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProcessPlayerMove : MonoBehaviour
{
    private StageManager stageManager;
    private GameContext gameContext;
    private APlayer player;

    private void Awake()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        gameContext = GameManager.Instance.gameContext;
    }

    private void Update()
    {
        if (!stageManager.IsReadyToMove() || stageManager.IsProcessingUnitAct())
        {
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (HpUI.Instance != null && !HpUI.IsPointerOverSelf())
            {
                return;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryClickTile();
        }
    }

    private void TryClickTile()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        player = gameContext.player;

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        GameObject highestObj = null;
        int highestOrder = int.MinValue;
        ATile selectedTile = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            var renderer = hit.collider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;

                if (highestObj.TryGetComponent<ATile>(out var foundTile))
                {
                    selectedTile = foundTile;
                }
            }
        }

        if (selectedTile == null || !gameContext.lastSearchReachableTile.Contains(selectedTile))
        {
            return;
        }

        List<ATile> path = gameContext.lastSearchShortestPath;
        if (path == null || path.Count == 0)
        {
            return;
        }

        stageManager.StartProcessingUnitAct();
        StartCoroutine(MovePlayerAlongPath(path));
    }

    private IEnumerator MovePlayerAlongPath(List<ATile> path)
    {
        RuleManager ruleManager = GameManager.Instance.ruleManager;
        List<ATile> _path = new List<ATile>(path);

        foreach (var tile in _path)
        {
            if (!ruleManager.CanUnitEnterTile(player.playerData, tile.tileData))
            {
                break;
            }

            int modifier = ruleManager.GetMoveSpeedModifier(player.playerData, tile.tileData);
            if (modifier < 1) modifier = 1;

            int apCost = player.playerData.actionPointPerMove * modifier;

            if (player.CanUseActionPoint(apCost))
            {
                player.UseActionPoint(apCost);
            }
            else
            {
                break;
            }

            bool moveDone = false;
            player.Move(tile.tileData.hexCoord, () =>
            {
                moveDone = true;
            });

            yield return new WaitUntil(() => moveDone);

            if (player.IsMoveInterrupted())
            {
                stageManager.EndProcessingUnitAct();
                yield break;
            }
        }

        stageManager.EndProcessingUnitAct();
    }
}
