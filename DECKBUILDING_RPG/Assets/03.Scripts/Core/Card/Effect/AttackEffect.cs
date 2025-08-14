using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : ACardEffect
{

    public override void Execute(ATile receiver,Unit user, Action onComplete = null)
    {
        base.Execute(receiver, user, onComplete);
        target.TakeDamage(value,null);
    }
}
