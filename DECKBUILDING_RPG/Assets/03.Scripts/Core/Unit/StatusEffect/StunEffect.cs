using StatusEffectEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : IStatusEffect {

    public StatusEffectType EffectType => StatusEffectType.Stun;
    public static readonly StunEffect Instance = new StunEffect();
    private StunEffect() { }
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
        unit.Stunned();
        GameManager.Instance.particleManager.PlayAnimation("ElectricEffect", unit.transform.position, unit.transform.rotation);
    }
}
