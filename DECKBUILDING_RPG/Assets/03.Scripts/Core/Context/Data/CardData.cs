using NPCEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class FusionCardEffectData
{
    public CardEffectTemplate template;
    public FusionCardEffectData(CardEffectTemplate template)
    {
        this.template = template;
    }
}
[Serializable]
public class CardData
{
    public int id;

    public CardTemplate cardTemplate;
    public List<FusionCardEffectData> fusioneffects;
    public bool isFusion=false;
    //public string path = "Assets/02.Prefabs/Card/CardObject.prefab";
    public CardData CloneCardData()
    {
        if (id !=-1)
            LoadCard();
        return new CardData { id = this.id,cardTemplate = this.cardTemplate, 
            fusioneffects = this.fusioneffects != null ? 
            new List<FusionCardEffectData>(this.fusioneffects) : new List<FusionCardEffectData>() ,
        isFusion = this.isFusion};
    }

    public void LoadCard()
    {
      
        cardTemplate = GameManager.Instance.dataManager.GetTemplate<CardTemplate>(id);

    }
    
}
