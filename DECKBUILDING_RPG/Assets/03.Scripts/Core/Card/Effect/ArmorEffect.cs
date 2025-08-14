using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ArmorEffect : ACardEffect {
    public override void Execute(ATile receiver, Unit user, Action onComplete = null)
    {
        base.Execute(receiver, user, onComplete);
        target.AddArmor(value);
        GameManager.Instance.particleManager.PlayAnimation("ArmorEffect",target.transform.position,transform.rotation);
    }
}
