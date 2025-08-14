using System;
using System.Collections.Generic;
using StatusEffectEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffectBook", menuName = "Custom/StatusEffectBook")]
public class StatusEffectBookSO : ScriptableObject
{
    public List<StatusEffectEntry> entries;
}

[Serializable]
public class StatusEffectEntry
{
    public StatusEffectType effectType;
    public string effectName;
    public Sprite icon;
    public int defaultDuration;
    [TextArea] public string description;
}