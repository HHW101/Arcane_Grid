using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPCEnum;
using System;
using System.Linq;

public class NPC : ANPC,ITurnable
{
    public NpcController controller;

    public StageManager _StageManager => stageManager;

    public override void TakeInteract(Action onComplete)
    {
        Debug.Log($"[{gameObject.name}] 상호작용 발생");
        interactUI.Enable(onComplete);
    }

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<NpcController>();

    }

    protected override void Start()
    {
        base.Start();

        if (controller == null)
            controller = GetComponent<NpcController>();

        controller.Init(this);
    }

    public override void Synchronize()
    {
        npcData.currentState = controller.currentStateType;
        if (npcData != null)
        {
            npcData.statusEffectList = _statusEffects
                .Select(inst => new StatusEffectInstance(inst.effectType, inst.remainingTurn))
                .ToList();
        }
    }

    public override void StartTurn()
    {
        base.StartTurn();
        controller.OnTurnStart();
    }

    public override void EndTurn()
    {
        base.EndTurn();
    }

    public override void RefillTurn()
    {
        base.RefillTurn();
    }

}
