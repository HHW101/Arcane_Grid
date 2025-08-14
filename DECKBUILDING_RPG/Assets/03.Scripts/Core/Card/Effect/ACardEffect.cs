using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACardEffect : MonoBehaviour
{
    public string nameKey;
    public int value;
    public CardEnum.EffectType type;
    public bool isTargetSelf;
    public int range;
    public CardEffectTemplate template;
    public virtual void Init(CardEffectTemplate template)
    {
        //Debug.Log("실행");
        nameKey = template.nameKey;
        value=template.value;
        type = (CardEnum.EffectType)template.type;

        range= template.range;
        isTargetSelf = template.isTargetSelf;
        this.template = template;
    }
    protected Unit target;
    public virtual void Execute(ATile receiver, Unit user, Action onComplete = null)
    {
        target=null;
        if (isTargetSelf)
            target = user;
        else
            target = receiver.Unit;
    }
}
