using StatusEffectEnum;

public interface IStatusEffect
{
    StatusEffectType EffectType { get; }
    void Apply(Unit unit, int? customTurn = null);         
    void OnTurnStart(Unit unit, StatusEffectInstance instance);
    void OnRemove(Unit unit, StatusEffectInstance instance);
}