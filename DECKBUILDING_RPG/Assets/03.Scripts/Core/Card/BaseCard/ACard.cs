using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ACard : MonoBehaviour
{

    protected CardData data;
    public CardData Data {  get { return data; } }
    [SerializeField]
    protected CardTemplate template;
    public CardTemplate Template { get { return template; } }
    protected bool isVisible;
    protected List<ACardEffect> effects = new List<ACardEffect>();
    public string Name { get { return template.nameKey; } }
    public string FlavorText { get { return template.flavorTextKey; } }
    public int Value { get { return calculateValue<AttackEffect>(); } }
    protected int value;
    public int Cost { get { return cost; } }
    protected int cost;
    public int ID { get { return id; } }
    protected int id;
    public int Price { get { return price; } }
    protected int price;
    private int range;
    public int Range { get { return range; } }
    public int index;
    protected bool isMine=false;
    protected bool isEnemy =false;
    public bool isFusion=false;
    protected virtual void OnEnable()
    {
        isFusion =false;
    }
    public void ChangeFData(List<FusionCardEffectData> datas)
    {
        data.fusioneffects = datas;
        data.isFusion = isFusion;
    }
    public void ResetEffects()
    {

        foreach (var effect in effects)
            Destroy(effect);
        effects.Clear();
    }
    protected virtual void OnDisable()
    {
        ACardEffect[] lastEffects = GetComponents<ACardEffect>();
        foreach (ACardEffect effect in lastEffects)
            Destroy(effect);
    }

    public virtual void Fusion()
    {
        isFusion = true;
        data.isFusion = isFusion;
        Init(Template);

    }
    public void AddEffect(ACardEffect effect)
    {
       
        if(effect.isTargetSelf)
            isMine = true;
        else
            isEnemy = true;
        effects.Add(effect);
    }

    public string makeDescription()
    {
        string text = null;
        if (isFusion)
            text += "융합\n";
        text += makeDesc<AttackEffect>("공격");
        text += makeDesc<ArmorEffect>("방어");
        text += makeDesc<HealEffect>("회복");
        text += makeDesc<StatusEffectEffect>();
        text += makeDesc<MoveEffect>("이동");
        text += makeDesc<DrawEffect>("카드를 획득");
       

     
        return text;
    }
    public string makeDesc<T>(string name) where T : ACardEffect
        {
        string text = ""; 
        int value = calculateValue<T>();
        if(value > 0)
            text += $"{value}만큼 {name}합니다.\n";
        return text;
    }
    public string makeDesc<T>() where T : ACardEffect
    {
        string text = "";
        Dictionary<string, string> status = calculateValue2<T>();
        foreach(var pair  in status)
            text += $"{pair.Value}만큼 {pair.Key}을 입힙니다.\n";
        return text;
    }
    public string makeShortDesc<T>() where T : ACardEffect
    {
        string text = "";

        Dictionary<string, string> status = calculateValue2<T>();
        foreach (var pair in status)
        {
            text += $"{pair.Key} {pair.Value}\n";
        }
 
    
        return text;
    }
    public string makeShortDesc<T>(string name) where T : ACardEffect
    {
        string text = "";
        int value = calculateValue<T>();
        if (value > 0)
            text += $"{name} {value}\n";
        return text;
    }

    public string makeShortDescription()
    {
        string text = null;
        if (isFusion)
            text += "융합\n";
        text += makeShortDesc<AttackEffect>("공격");
        text += makeShortDesc<ArmorEffect>("방어");
        text += makeShortDesc<HealEffect>("회복");
        text += makeShortDesc<StatusEffectEffect>();
        text += makeShortDesc<MoveEffect>("이동");
        text += makeShortDesc<DrawEffect>("드로우");

        return text;
    }
    public virtual Dictionary<string,string> calculateValue2<T>() where T : ACardEffect
    {
        Dictionary<string, string> status = new Dictionary<string,string>();
        foreach (ACardEffect effect in effects)
        {
            if (effect is T)
                status.Add(effect.nameKey,effect.value.ToString());


        }
        return status;
    }
    public virtual void Init(CardTemplate cardTemplate)
    {
        isVisible = false;
       value=calculateValue<AttackEffect>();
        cost = cardTemplate.cost;
        template=cardTemplate;
        price = cardTemplate.price;
        range = calculateMaxRange<ACardEffect>();
        id = cardTemplate.ID;
        if (!isFusion)
            data = new CardData();
        
        data.cardTemplate = template;

         data.id = cardTemplate.ID;


    }
    public void SetFusionData(CardData _data)
    {
        data = new CardData();
        data.fusioneffects = _data.fusioneffects;
        data.cardTemplate = _data.cardTemplate;
        template = _data.cardTemplate;
        data.isFusion = true;
    }
    public virtual void Selected()
    {

    }
    public virtual void UnSelected()
    {

    }
    public virtual int calculateValue<T>() where T : ACardEffect
    {
        int val = 0;
        foreach (ACardEffect effect in effects) {
            if (effect is T)
                val += effect.value;
        }
        return val;
    }

    public virtual int calculateMaxRange<T>() where T : ACardEffect
    {
        int val = 0;
        foreach (ACardEffect effect in effects)
        {
            if (effect is T)
                val = Mathf.Max(val, effect.range);
        }
        return val;
    }
    public virtual void Use(ATile receiver, Unit user) {
        APlayer player = user as APlayer;
 
        if (player!=null)
        {
            player.UseTechnicalPoint(cost);
        }
        if(template.isGun)
        {
            user.LoadGun(this);
            return;
        }
        foreach (ACardEffect effect in effects) { 
            effect.Execute(receiver,user);
        }
        
    }
    public virtual void End()
    {
        Destroy(gameObject);
    }
    public virtual bool CanUse(Unit user)
    {
        APlayer player = user as APlayer;
  
        List<ATile> RangeTile = GameManager.Instance.campaignManager.stageManager.GetCardUsableTileList(this);

        bool isTarget=false;
        foreach (ATile rangeTile in RangeTile) { 
            if(rangeTile.Unit!=null)
                isTarget = true;
        
        }
        if(!isTarget)
            return false;
        if (player != null)
        {
            return player.CanUseTechnicalPoint(cost);
        }

        return true;
    }
   
    public virtual bool CanUse(ATile receiver,Unit user)
    {
        APlayer player = user as APlayer;
        if (receiver == null)
            return player.CanUseGun();
        if(receiver.Unit==null)
            return false;
        List<ATile> RangeTile = GameManager.Instance.campaignManager.stageManager.GetCardUsableTileList(this);

        if (!RangeTile.Contains(receiver))
            return false;
          
        if (player!=null)
        {
            if (!player.CanUseTechnicalPoint(cost))
                return false;
        }
        bool isSelf = receiver.Unit == user;


        if (isSelf)
        {
            return isMine;
        }

        return isEnemy;
    }

    public abstract void Show();
    public abstract void Delete();
}
