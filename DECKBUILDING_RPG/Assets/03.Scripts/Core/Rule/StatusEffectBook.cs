using System;
using System.Collections.Generic;
using StatusEffectEnum;

public class StatusEffectBook
{
    private Dictionary<StatusEffectType, StatusEffectEntry> entryMap = new();
    private Dictionary<StatusEffectType, IStatusEffect> logicMap = new();

    public StatusEffectBook(StatusEffectBookSO so)
    {
        foreach (var entry in so.entries)
            entryMap[entry.effectType] = entry;

        logicMap[StatusEffectType.Poison] = PoisonEffect.Instance;
        logicMap[StatusEffectType.Burn]   = BurnEffect.Instance;
        logicMap[StatusEffectType.Stun]   = StunEffect.Instance;
    }

    public StatusEffectEntry GetEntry(StatusEffectType type)
        => entryMap.TryGetValue(type, out var entry) ? entry : null;
    public IStatusEffect GetLogic(StatusEffectType type)
        => logicMap.TryGetValue(type, out var logic) ? logic : null;

    public void ApplyEffectToUnit(IStatusEffect effect, Unit unit, int? customTurn = null)
    {
        var exist = unit.StatusEffectList.Find(x => x.effectType == effect.EffectType);
        int defaultTurn = GetEntry(effect.EffectType)?.defaultDuration ?? 1;
        int turn = customTurn ?? defaultTurn;
        if (exist != null)
        {
            exist.remainingTurn = turn;
        }
        else
        {
            unit.StatusEffectList.Add(new StatusEffectInstance(effect.EffectType, turn));
        }
    }
    
}

[Serializable]
public class StatusEffectInstance
{
    public StatusEffectType effectType;
    public int remainingTurn;

    public StatusEffectInstance(StatusEffectType type, int remainingTurn)
    {
        this.effectType = type;
        this.remainingTurn = remainingTurn;
    }
}