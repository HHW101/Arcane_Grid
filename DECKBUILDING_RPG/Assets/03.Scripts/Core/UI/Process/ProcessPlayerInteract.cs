using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProcessPlayerInteract : MonoBehaviour
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
        if (!stageManager.IsReadyToInteract() || stageManager.IsProcessingUnitAct())
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
            TryClickNPC();
        }
    }

    private void TryClickNPC()
    {
        player = gameContext.player;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);

        GameObject highestObj = null;
        int highestOrder = int.MinValue;
        ANPC targetNPC = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            var renderer = hit.collider.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;

                if (highestObj.TryGetComponent<ANPC>(out var npc))
                {
                    targetNPC = npc;
                }
            }
        }

        if (targetNPC == null)
        {
            return;
        }

        if (!gameContext.lastSearchInteractableNPC.Contains(targetNPC))
        {
            return;
        }

        stageManager.StartProcessingUnitAct();
        StartCoroutine(InteractNPCWithCallback(targetNPC));
    }


    private IEnumerator InteractNPCWithCallback(ANPC npc)
    {
        bool isInteractDone = false;
        if (player.CanUseActionPoint(player.playerData.actionPointPerInteract))
        {
            player.UseActionPoint(player.playerData.actionPointPerInteract);
        }
        else
        {
            yield break;
        }
        player.Interact(npc, () =>
        {
            isInteractDone = true;
        });

        yield return new WaitUntil(() => isInteractDone);

        player.UseActionPoint(player.playerData.actionPointPerInteract);

        stageManager.EndProcessingUnitAct();
    }
}
