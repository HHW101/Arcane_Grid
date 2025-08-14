using StatusEffectEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect :IStatusEffect
{
    public StatusEffectType EffectType => StatusEffectType.Burn;
    public static readonly BurnEffect Instance = new BurnEffect();
    private BurnEffect() { }
    public void Apply(Unit unit, int? customTurn = null)
    {
        GameManager.Instance.campaignManager.stageManager.turnManager
            .ApplyStatusEffectToUnit(this, unit, customTurn);
    }

    public void OnRemove(Unit unit, StatusEffectInstance instance)
    {
     
    }

    public void OnTurnStart(Unit unit, StatusEffectInstance instance)
    {
         unit.TakeDamage(2);
        GameManager.Instance.particleManager.PlayAnimation("FireEffect",unit.transform.position,unit.transform.rotation);
    }


}
