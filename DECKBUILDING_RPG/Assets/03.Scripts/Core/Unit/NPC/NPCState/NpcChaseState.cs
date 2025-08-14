using System.Collections;
using System.Collections.Generic;
using NPCEnum;
using UnityEngine;

public class NpcChaseState : INPCState
{
    private static readonly int Move = Animator.StringToHash("Move");

    public void Enter(NpcController npcController) { }

    public IEnumerator Execute(NpcController npcController, System.Action<NPCStateResult> onStateSignal)
    {
        ANPC npc = npcController.npc;
        var stageManager = npc.StageManager;

        while (true)
        {
            var target = npcController.GetTarget();
            if (!target)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            var targetCoord = (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord;
            float dist = npc.npcData.hexCoord.Distance(targetCoord);

            var availableAttack = npc.GetRandomAvailableAttack(target);
            if (availableAttack && dist <= availableAttack.range)
            {
                onStateSignal(NPCStateResult.ToAttack);
                yield break;
            }

            bool canBasicAttackNow =
                (npc.attackTypeList == null || npc.attackTypeList.Count == 0) &&
                dist <= npc.npcData.attackRange &&
                npc.npcData.currentActionPoint >= npc.npcData.actionPointPerAttack &&
                npc.npcData.currentAttackCount > 0;

            if (canBasicAttackNow)
            {
                onStateSignal(NPCStateResult.ToAttack);
                yield break;
            }

            if (dist > npc.npcData.detectRange)
            {
                onStateSignal(NPCStateResult.ToPatrol);
                yield break;
            }
            
            bool canAttackNow = (availableAttack && dist <= availableAttack.range);
            if (dist <= 1 && !canAttackNow)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            if (npc.npcData.currentActionPoint < npc.npcData.actionPointPerMove)
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

            if (!npcController.TryFindBestNormalApproach(target, out var destCoord, out var path))
            {
                onStateSignal(NPCStateResult.EndTurn);
                yield break;
            }

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

            var singleStepPath = new List<HexCoord> { fromHex, toHex };
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
