using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tile : ATile
{
    public override bool isTarget(ACard card, Unit unit)
    {
        return true;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }
}
