using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CardEnum;
public class StatusEffectEffect : ACardEffect
{
    private StatusEffectEnum.StatusEffectType effectType;
    private TurnManager turnManager;
    public override void Init(CardEffectTemplate template)
    {
        base.Init(template);
        effectType = (StatusEffectEnum.StatusEffectType)((int)type - 10);
        turnManager = GameManager.Instance.campaignManager.stageManager.turnManager;
    

    }
    // Start is called before the first frame update
    public override void Execute(ATile receiver, Unit user, Action onComplete = null)
    {
        base.Execute(receiver, user, onComplete);
        turnManager.AddStatusEffect(target, effectType);
  
        //GameManager.Instance.particleManager.PlayAnimation("posion",)
    }
}
