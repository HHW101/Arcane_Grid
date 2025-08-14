using System.Collections;
using NPCEnum;
using UnityEngine;

public class NpcAttackState : INPCState
{
    private static readonly int Attack = Animator.StringToHash("Attack");

    public void Enter(NpcController npcController) { }

    public IEnumerator Execute(NpcController npcController, System.Action<NPCStateResult> onStateSignal)
    {
        ANPC npc = npcController.npc;
        var target = npcController.GetTarget();

        if (!target)
        {
            onStateSignal(NPCStateResult.ToPatrol);
            yield break;
        }

        var availableAttack = npc.GetRandomAvailableAttack(target);
        float dist = npc.npcData.hexCoord.Distance(
            (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord);

        bool canUseSoAttack = (availableAttack) && dist <= availableAttack.range;
        bool canBasicAttack = (npc.attackTypeList == null || npc.attackTypeList.Count == 0)
                                && dist <= npc.npcData.attackRange
                                && npc.npcData.currentActionPoint >= npc.npcData.actionPointPerAttack
                                && npc.npcData.currentAttackCount > 0;

        if (!canUseSoAttack && !canBasicAttack)
        {
            onStateSignal(NPCStateResult.EndTurn);
            yield break;
        }
        
        npcController.Animator.SetBool(Attack, true);
        npc.Attack(target);

        var target1 = target;
        yield return new WaitUntil(() => npc.AttackCoroutine == null || target1 == null);

        target = npcController.GetTarget();
        if (!target)
        {
            onStateSignal(NPCStateResult.ToPatrol);
            yield break;
        }

        var nextAvailable = npc.GetRandomAvailableAttack(target);
        dist = npc.npcData.hexCoord.Distance(
            (target is APlayer p2) ? p2.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord);

        bool nextCanUseSoAttack = (nextAvailable != null) && dist <= nextAvailable.range;
        bool nextCanBasicAttack = (npc.attackTypeList == null || npc.attackTypeList.Count == 0)
                                    && dist <= npc.npcData.attackRange
                                    && npc.npcData.currentActionPoint >= npc.npcData.actionPointPerAttack
                                    && npc.npcData.currentAttackCount > 0;

        if (npc.npcData.currentAttackCount <= 0 ||
            npc.npcData.currentActionPoint <= 0 ||
            (!nextCanUseSoAttack && !nextCanBasicAttack))
        {
            onStateSignal(NPCStateResult.EndTurn);
        }
        else
        {
            onStateSignal(NPCStateResult.ToAttack);
        }
    }

    public void Exit(NpcController npcController)
    {
        npcController.Animator.SetBool(Attack, false);
    }
}
