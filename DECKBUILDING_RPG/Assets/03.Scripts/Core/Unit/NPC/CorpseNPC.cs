using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPCEnum;
using System;

public class CorpseNPC : ANPC, ITurnable
{
    //public NpcController controller;

    public StageManager _StageManager => stageManager;

    protected override void Awake()
    {
        base.Awake();
        //controller = GetComponent<NpcController>();

    }

    protected override void Start()
    {
        base.Start();
        interactUI?.Init(npcData);
        interactUI?.gameObject.SetActive(false);
        /*if (controller == null)
            controller = GetComponent<NpcController>();

        //controller.Init(this);*/
    }

    public override void Synchronize()
    {
        //npcData.currentState = controller.currentStateType;
    }

    public override void StartTurn()
    {
        base.StartTurn();
        base.EndTurn();
        //controller.OnTurnStart();
    }

    public override void TakeInteract(Action onComplete)
    {
        Debug.Log($"[{gameObject.name}] 상호작용 발생");
        interactUI.Enable(onComplete);
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
