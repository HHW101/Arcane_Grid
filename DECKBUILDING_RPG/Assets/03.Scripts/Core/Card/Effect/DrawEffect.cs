using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawEffect : ACardEffect
{
    public override void Execute(ATile receiver, Unit user, Action onComplete = null)
    {
        base.Execute(receiver, user, onComplete);
        for(int i=0;i<value;i++)
            target.cardController.DrawCard();
    }
}
