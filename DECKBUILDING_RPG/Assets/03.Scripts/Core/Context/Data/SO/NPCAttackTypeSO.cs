using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum NPCAttackCategory { Melee, Ranged, Skill, Special }

public enum SpecialAttackType { None, JumpAttack, TeleportAttack }

[CreateAssetMenu(menuName = "Attack/NPCAttackTypeSO")]
public class NPCAttackTypeSO : ScriptableObject
{
    [Header("기본 데이터")]
    public string attackName;
    public string description;
    public NPCAttackCategory category;
    public SpecialAttackType specialType;
    public int damage = 10;
    public float range = 1.0f;
    public int apCost = 2;
    public float cooldown = 0; 

    [Header("연출/이펙트")]
    public AttackEffectData effects;
    public virtual bool UseDefaultEffect => true;

    [Header("상태이상")]
    public List<StatusEffectSO> statusEffects;

    public virtual IEnumerator ExecuteAttackCoroutine(ANPC npc, Unit target)
    {
        PlayAttackEffects(npc, target);
        target.TakeDamage(damage, null);
        ApplyStatusEffects(target);
        yield break;
    }

    protected virtual void PlayAttackEffects(ANPC npc, Unit target)
    {
        if (effects != null && effects.projectilePrefab)
            GameObject.Instantiate(effects.projectilePrefab, npc.transform.position, Quaternion.identity);

        if (effects != null && effects.effectPrefab)
            GameObject.Instantiate(effects.effectPrefab, target.transform.position, Quaternion.identity);

        if (effects != null && effects.sfx)
            AudioSource.PlayClipAtPoint(effects.sfx, target.transform.position);
    }

    protected virtual void ApplyStatusEffects(Unit target)
    {
        if (statusEffects == null) return;
        foreach (var se in statusEffects)
            se.Apply(target);
    }
}