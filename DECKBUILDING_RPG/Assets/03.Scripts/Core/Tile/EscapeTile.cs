using System;
using UnityEngine;

public class EscapeTile : ATile
{
    [SerializeField]
    private StageEscapeUI stageEscapeUI;

    protected override void Start()
    {
        base.Start();
        stageEscapeUI.Disable();
    }

    public override bool isTarget(ACard card, Unit unit)
    {
        return true;
    }

    public override void ReactAfterUnitEnterThisTile(Unit unit, Action onComplete)
    {
        Logger.Log($"[{gameObject.name}] EscapeTile : {unit.gameObject.name} entered this tile");

        if (unit is APlayer player)
        {
            stageEscapeUI.Enable(onComplete, player);
        }
        else
        {
            onComplete?.Invoke();
        }
    }
}
