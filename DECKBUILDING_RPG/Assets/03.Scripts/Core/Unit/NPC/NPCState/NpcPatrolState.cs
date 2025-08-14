using System.Collections;
using NPCEnum;
using UnityEngine;

public class NpcPatrolState : INPCState
{
    private static readonly int Move = Animator.StringToHash("Move");

    public void Enter(NpcController npcController) { }

    public IEnumerator Execute(NpcController npcController, System.Action<NPCStateResult> onStateSignal)
    {
        ANPC npc = npcController.npc;
        var stageManager = npc.StageManager;

        while (true)
        {
            // 감지: 경로 무시, Neutral 제외
            var sensed = npcController.PeekTargetInDetectRange();
            if (sensed != null)
            {
                var targetCoord = (sensed is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)sensed).npcData.hexCoord;
                float dist = npc.npcData.hexCoord.Distance(targetCoord);

                var availableAttack = npc.GetRandomAvailableAttack(sensed);
                bool canSoAttack = (availableAttack != null) && dist <= availableAttack.range;
                bool canBasicAttack =
                    (npc.attackTypeList == null || npc.attackTypeList.Count == 0) &&
                    dist <= npc.npcData.attackRange &&
                    npc.npcData.currentActionPoint >= npc.npcData.actionPointPerAttack &&
                    npc.npcData.currentAttackCount > 0;

                if (canSoAttack || canBasicAttack)
                {
                    onStateSignal(NPCStateResult.ToAttack);
                    yield break;
                }

                if (dist <= npc.npcData.detectRange)
                {
                    onStateSignal(NPCStateResult.ToChase);
                    yield break;
                }
            }

            if (npc.npcData.currentActionPoint < npc.npcData.actionPointPerMove)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            var currentCoord = npc.npcData.hexCoord;
            var candidates = stageManager.GetNpcAroundTile(currentCoord, 5, 5);
            if (candidates == null || candidates.Count == 0)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }
            var randomTarget = candidates[Random.Range(0, candidates.Count)];

            var path = stageManager.FindNpcPath(currentCoord, randomTarget, npc.npcData);
            if (path is not { Count: > 1 })
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            var fromHex = path[0];
            var toHex = path[1];
            var stageData = stageManager.GetCurStageData();
            if (!stageData.tiles.TryGetValue(toHex.ToString(), out var toTileData))
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }
            var modifier = GameManager.Instance.ruleManager.GetMoveSpeedModifier(npc.npcData, toTileData);
            var apCost = npc.npcData.actionPointPerMove * modifier;
            if (npc.npcData.currentActionPoint < apCost)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            var singleStepPath = new System.Collections.Generic.List<HexCoord> { fromHex, toHex };
            npcController.Animator.SetBool(Move, true);
            npc.MoveAlongPath(singleStepPath, drawLine: false);
            yield return new WaitUntil(() => npc.MoveCoroutine == null);
        }
    }

    public void Exit(NpcController npcController)
    {
        npcController.Animator.SetBool(Move, false);
    }
}
