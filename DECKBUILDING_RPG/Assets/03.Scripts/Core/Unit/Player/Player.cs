using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public enum AttackType 
{
    Melee, //근접
    Range, //원거리
    none   // 공격 불가
}

public class Player : APlayer
{
    public HexCoord HexCoord; //클릭한 타일의 Hexcoord
    public LayerMask tileLayer;
    private Unit selectedTarget; 
    public LayerMask enemyLayer;
    public AttackType attackType = AttackType.none;

    protected override void Start()
    {
        base.Start();
    }

    public override List<UnitEnum.UnitType> GetUnitTypeList()
    {
        return playerData.unitTypeList;
    }

    public override bool IsHaveThisType(UnitEnum.UnitType unitType)
    {
        return true;
    }
    /*
    public void OnClickMoveButton()
    {
        HexCoord start = this.playerStateInStage.hexCoord;
        HexCoord destination = HexCoord;
        //Logger.Log($"[Player] 이동 요청: {start} → {destination}");
        AStarPathFinder pathFinder = new AStarPathFinder();
        List<HexCoord> path = pathFinder.FindPathWithWeightedCost(
            this.StageManager.currentLoadedStageData,
            start,
            destination,
            (from, to) => 1f 
        );
        if (path != null && path.Count > 1)
        {
            path.RemoveAt(0); 
            MoveAlongPath(path);
        }
        else
        {
            Logger.LogWarning("[Player] 이동 불가: 유효하지 않은 경로");
        }
    }

    public void OnClickAttackButton()
    {
        if (selectedTarget == null)
        {
            Debug.LogWarning("[Player] 공격 대상이 선택되지 않았습니다.");
            return;
        }

        Debug.Log($"[Player] {selectedTarget.name} 에게 공격을 시도합니다.");
                
        selectedTarget = null;
        EndTurn(); // 공격 후 턴 종료 처리 (필요 시)
    }


    public void MoveAlongPath(List<HexCoord> path)
    {
        StartCoroutine(CoMoveAlongPath(path));
    }
    private IEnumerator CoMoveAlongPath(List<HexCoord> path)
    {
        foreach (HexCoord coord in path)
        {
            move(coord.q, coord.r);
            yield return new WaitForSeconds(0.2f);
        }
        //Logger.Log("[Player] 전체 경로 이동 완료");
        EndTurn(); // 턴 종료 처리
    }
    public override void move(int x, int y)
    {
       
        HexCoord newCoord = new HexCoord(x, y);
        this.playerStateInStage.hexCoord = newCoord;
        StopAllCoroutines(); //이친구를 주석해제하면 한칸씩이동 , 여러 이동 중첩 방지
        StartCoroutine(MoveSmoothly(stageManager.ConvertHexToWorld(newCoord)));
        //Logger.Log($"[Player] ({x}, {y})로 이동 중...");
        ANPC target = FindClosestNpc();
        if (target != null && IstargetAttackRange(target))
        {
            Debug.Log($"가장 가까운 적 {target.name}이 공격 범위 안에 있음!");
            // 공격하거나, UI 표시하거나 등등
        }
        else
        {
            //Debug.Log("공격 가능한 NPC가 없음");
        }
    }
    private IEnumerator MoveSmoothly(Vector3 targetPos)
    {
        float duration = 0.2f;
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    private void Update()
    {
        ClickTile();
        if (Input.GetKeyDown(KeyCode.Space)) //이동테스트용,  씬에 존재하는 이동 버튼에 등록
        {
            OnClickMoveButton();// 이 함수를 등록 , AddListener
        }

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    foreach (ANPC npc in GameManager.Instance.gameContext.npcList)
        //    {
        //        bool inRange = IstargetAttackRange(npc);
        //        Debug.Log($"NPC {npc.name} 공격 가능? : {inRange}");
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.A)) 
        //{
        //    OnClickAttackButton();
        //}

    }


    private void ClickTile()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, tileLayer);
            if (hit.collider != null && hit.collider.GetComponentInParent<ATile>() is ATile tile)
            {
               // Debug.Log("타일 클릭");
                HexCoord = tile.tileData.hexCoord;
                //Debug.Log(HexCoord);
            }


            //아래는 향후 공격대상 선택 구현시 사용가능
            //RaycastHit2D enemyHit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f, enemyLayer);
            //if (enemyHit.collider != null && enemyHit.collider.GetComponent<Unit>() is Unit enemy)
            //{
            //    selectedTarget = enemy;
            //    Debug.Log($"[Player] 공격 대상 선택됨: {enemy.name}");
            //}
        }

    }

    //공격하는 거리 판정
    public bool IstargetAttackRange(Unit target) 
    {
        if (attackType == AttackType.none)
        {
            return false;
        }

        HexCoord myCoord = playerStateInStage.hexCoord;
        HexCoord Targetcoord;
        if (target is ANPC npctarget)
        {

            Targetcoord = npctarget.npcData.hexCoord;
            //Debug.Log(npctarget.name);
        }

        int distance = HexCoord.Distance(myCoord, targetCoord);
        //Debug.Log(distance);
        switch (attackType)
        {
            case AttackType.Melee:
                return distance == 0;
            case AttackType.Range:
                return distance <= 2;
            default:
                return false;
        }

    }

    public void OnClickAttack(Unit target) // 이걸 버튼에 등록해서 사용
    {
        if (!IstargetAttackRange(target))
        {
            Debug.Log("범위 안에 적이없음");
            return;
        }
        target.TakeDamage(dmg);
        EndTurn();
    }

    public ANPC FindClosestNpc() // 기준점을 플레이어에서 가장 가까운 npc로 잡음
    {
        HexCoord myCoord = playerStateInStage.hexCoord;
        ANPC closestNpc = null;
        int closestDistance = int.MaxValue;
        foreach (ANPC npc in GameManager.Instance.gameContext.npcList)
        {
            if (npc == null || npc.npcData == null || npc.npcData.currentHealth <= 0)
                continue;
            HexCoord npcCoord = npc.npcData.hexCoord;
            int distance = HexCoord.Distance(myCoord, npcCoord);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNpc = npc;
            }
        }
        if (closestNpc != null)
        {
            Debug.Log($"[타겟 선택] 가장 가까운 NPC는: {closestNpc.name}, 거리: {closestDistance}");
        }
        return closestNpc;
    }*/

}
