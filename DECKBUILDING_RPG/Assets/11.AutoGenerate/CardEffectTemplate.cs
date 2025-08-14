using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardEffectTemplate
{
public int ID;
public string nameKey;
public int type;
public int value;
public int range;
public bool isTargetSelf;
    public CardEffectTemplate Clone()
    {
        return new CardEffectTemplate { ID = ID, nameKey = nameKey, type = type, value = value ,isTargetSelf=isTargetSelf,range=range};
    }
}
