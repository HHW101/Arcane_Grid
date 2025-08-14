using StatusEffectEnum;
using UnityEngine;

[CreateAssetMenu(menuName = "Status/StatusEffectSO")]
public class StatusEffectSO : ScriptableObject
{
    public StatusEffectType effectType;
    public string effectName;
    public string effectDescription;
    public int duration; 
    
    public virtual void Apply(Unit target)
    {
        GameManager.Instance.campaignManager.stageManager.turnManager
            .AddStatusEffect(target, this, duration);
    }
}

[System.Serializable]
public class AttackEffectData
{
    public GameObject effectPrefab;  
    public GameObject projectilePrefab; 
    public AudioClip sfx;             
}