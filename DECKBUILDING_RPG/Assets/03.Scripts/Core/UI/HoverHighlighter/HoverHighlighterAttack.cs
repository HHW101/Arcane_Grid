using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
//
public class HoverHighlighterAttack : MonoBehaviour
{
    private Camera mainCamera;
    private StageManager stageManager;
    private GameContext gameContext;
    private PlayerStateInStage playerStateInStage;
    private PlayerData playerData;

    private GameObject currentHovered = null;
    private List<ANPC> highlightedNPCs = new();
    private AttackInfo attackInfo;

    [SerializeField]
    private TMP_Text nextAttackInfoText;

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
        gameObject.SetActive(hoverHighlight == HoverHighlightEnum.HoverHighlight.Attack);
    }

    private void OnEnable()
    {
        RefreshAttackableTargets();
    }

    private void OnDisable()
    {
        ClearCurrentHighlight();
        ClearAttackableHighlights();
    }

    void Update()
    {
        if (stageManager.IsNPCListUpdateRequested())
        {
            stageManager.ProcessNPCListUpdateRequest();
            RefreshAttackableTargets();
        }
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

    private void RefreshAttackableTargets()
    {
        ClearAttackableHighlights();
        var attackableNPCs = stageManager.GetAttackableNPCList();
        if(attackableNPCs == null)
        {
            return;
        }
        attackInfo = gameContext.player.GetNextAttackInfo();

        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append("공격 타입 : ");
        stringBuilder.Append(attackInfo.isMelee ?
            "근접 공격" : "탄환 발사 공격");
        stringBuilder.Append("\n");

        stringBuilder.Append(attackInfo.isMelee ?
            "" : $"사용 탄환 : {attackInfo.bulletName}\n");

        stringBuilder.Append($"사거리 : {attackInfo.range}\n");

        stringBuilder.Append($"데미지 : {attackInfo.damage + attackInfo.offsetBulletDamage}");
        stringBuilder.Append(attackInfo.isMelee ?
            "" : $" + {attackInfo.offsetBulletDamage}(탄환 데미지)");
        stringBuilder.Append("\n");

        stringBuilder.Append(attackInfo.radius > 0 ?
            $"공격 범위: {attackInfo.radius}\n" : "");

        stringBuilder.Append(attackInfo.pushForce > 0 ?
            $"밀치기 힘: {attackInfo.pushForce}\n" : "");

        nextAttackInfoText.SetText(stringBuilder.ToString());

        foreach (var npc in attackableNPCs)
            {
                npc.HighlightOn();
                highlightedNPCs.Add(npc);
            }
    }

    private void ClearAttackableHighlights()
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
            int range = attackInfo.range;

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
