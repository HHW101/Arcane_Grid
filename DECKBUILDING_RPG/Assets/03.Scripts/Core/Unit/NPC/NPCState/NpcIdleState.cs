using System;
using NPCEnum;
using System.Collections;
using UnityEngine;
using System.Linq; // All 메서드 사용을 위해 필요

public class NpcIdleState : INPCState
{
    public void Enter(NpcController npcController)
    {
    }

    public IEnumerator Execute(NpcController npcController, Action<NPCStateResult> onStateSignal)
    {
        ANPC npc = npcController.npc;
        var target = npcController.GetTarget();

        if (!target)
        {
            onStateSignal?.Invoke(NPCStateResult.ToPatrol);
            yield break;
        }

        var availableAttack = npc.GetRandomAvailableAttack(target);
        float dist = npc.npcData.hexCoord.Distance(
            (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord);

        if (availableAttack && dist <= availableAttack.range)
        {
            onStateSignal(NPCStateResult.ToAttack);
            yield break;
        }

        var noValidSo = (npc.attackTypeList == null || npc.attackTypeList.Count == 0 || npc.attackTypeList.All(at => !at));
        if (noValidSo &&
            dist <= npc.npcData.attackRange &&
            npc.npcData.currentActionPoint >= npc.npcData.actionPointPerAttack &&
            npc.npcData.currentAttackCount > 0)
        {
            onStateSignal(NPCStateResult.ToAttack);
            yield break;
        }

        if (dist <= npc.npcData.detectRange)
        {
            onStateSignal(NPCStateResult.ToChase);
            yield break;
        }

        onStateSignal(NPCStateResult.ToPatrol);
    }

    public void Exit(NpcController npcController) { }
}