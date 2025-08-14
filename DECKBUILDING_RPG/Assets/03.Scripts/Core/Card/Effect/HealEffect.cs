using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class HealEffect : ACardEffect
{
    public override void Execute(ATile receiver, Unit user, Action onComplete = null)
    {
        base.Execute(receiver, user, onComplete);
        target.Heal( value);
    }
}
