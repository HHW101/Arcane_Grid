using StatusEffectEnum;

public class PoisonEffect : IStatusEffect
{
    public static readonly PoisonEffect Instance = new PoisonEffect();
    public StatusEffectType EffectType => StatusEffectType.Poison;

    private PoisonEffect() { } 

    public void Apply(Unit unit, int? customTurn = null)
    {
        GameManager.Instance.campaignManager.stageManager.turnManager
            .ApplyStatusEffectToUnit(this, unit, customTurn);
    }

    public void OnTurnStart(Unit unit, StatusEffectInstance instance)
    {
        unit.TakeDamage(2);
        GameManager.Instance.particleManager.PlayAnimation("PoisonEffect", unit.transform.position, unit.transform.rotation);
        
    }

    public void OnRemove(Unit unit, StatusEffectInstance instance)
    {
    }
}
