using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Events;
using UnityEngine.UI;

[SerializeField]
public class CardManagerParam
{
   // [SerializeField] public int MinDeckNum = 10;

}
public class CardManager : MonoBehaviour
{
    [SerializeField] private CardManagerParam cardManagerParam = new CardManagerParam();
    ACard selectedCard;
    public Action<ACard> showCardInfo;
    public Action hideCardInfo;
    [HideInInspector]
    public UnityEvent OnSelected;
    [HideInInspector]
    public UnityEvent UnSelected;
    protected PoolManager poolManager;
    public ACard SelectedCard { get { return selectedCard; } }
    protected ATile selectedTarget;
    public ATile Target { get { return selectedTarget; } }
   // private CardManagerParam cardManagerParam;
    public int MinDeckNum { get { return 10; } }
    public bool isDeckCanUse() {
      //  Debug.Log(GameManager.Instance.gameContext.saveData.playerData);
       
      if (GameManager.Instance.gameContext.saveData.playerData.cards.deck != null)
                   if( GameManager.Instance.gameContext.saveData.playerData.cards.deck.Count >=10)
                    return true;
        
        return false;
      


    }
    public bool CanMakeDeck()
    {
        //  Debug.Log(GameManager.Instance.gameContext.saveData.playerData);
        List<CardData> deck = GameManager.Instance.gameContext.saveData.playerData.cards.deck;
        List<CardData> pack = GameManager.Instance.gameContext.saveData.playerData.cards.pack;
        if ( deck== null||pack==null)
            return false;
        if (deck.Count+pack.Count >= MinDeckNum-1)
                return true;

        return false;



    }
    private List<Sprite> sprites;
    Dictionary<string, Sprite> spriteDict;
    public void Init(PoolManager poolManager, CardManagerParam param)
    {
        this.poolManager = poolManager;
        cardManagerParam = param;
        sprites = Resources.LoadAll<Sprite>("Portraits/CardIcon").ToList();

       spriteDict  = sprites.ToDictionary(s => s.name, s => s);
    }
    public Sprite GetSprite(int id)
    {
        return spriteDict[id.ToString()];
    }
    
    protected void Awake()
    {

        

    }
    public Action AddHandUI;
    public Action onClick;
    public void OnDisable()
    {
        onClick = null;
    }
    public Hand AddHand()
    {
        Hand hand;
        hand = GameManager.Instance.gameContext.player.cardController.MyHand;

        return hand;
    }

    public void AddCardToPlayerDeck()
    {
        //if (selectedCard != null)
            //GameManager.Instance.gameContext.player.cardController.myDeck.Add(selectedCard);

    }
    #region Selecte
    public void ResetSelectedCard()
    {

        selectedCard = null;
        hideCardInfo?.Invoke();
        UnSelected?.Invoke();
    }
    public void SelectCard(ACard card)
    {

        selectedCard = card;
        showCardInfo?.Invoke(card);
        OnSelected?.Invoke();
    }
    public void ResetSelectTarget()
    {
        selectedTarget = null;
    }
    public void SetSelectTarget(ATile tile)
    {
     
        selectedTarget = tile;
    }
    #endregion
#region SaveLoad
    public void SaveDeck(List<CardData> cards)
    {
       // Debug.Log("덱 저장" + cards.Count);
        GameManager.Instance.gameContext.saveData.playerData.cards.deck = new List<CardData>(cards);
    }
    public void SavePack(List<CardData> cards)
    {

        GameManager.Instance.gameContext.saveData.playerData.cards.pack = new List<CardData>(cards);
      
    }
    public void SaveHand(List<CardData> cards)
    {

        GameManager.Instance.gameContext.saveData.playerData.cards.hand = new List<CardData>(cards);
    }
    public void SaveTrashZone(List<CardData> cards)
    {
        GameManager.Instance.gameContext.saveData.playerData.cards.trashZone  = new List<CardData>(cards);
    }
    public void LoadDeck(Deck cardzone)
    {


  
        List<CardData> data = new List<CardData>(GameManager.Instance.gameContext.saveData.playerData.cards.deck);

        cardzone.LoadCards(data);

    }
    public void LoadPack(Pack cardzone)
    {



        List<CardData> data = new List<CardData>(GameManager.Instance.gameContext.saveData.playerData.cards.pack);
        cardzone.LoadCards(data);

    }
    public void LoadHand(Hand cardzone)
    {
        List<CardData> data = new List<CardData>(GameManager.Instance.gameContext.saveData.playerData.cards.hand);
        cardzone.LoadCards(data);

    }
    public void LoadTrashZone(TrashZone cardzone)
    {

        List<CardData> data = new List<CardData>(GameManager.Instance.gameContext.saveData.playerData.cards.trashZone);
        cardzone.LoadCards(data);
    }
    public void LoadGun(AGun gun)
    {

        List<CardData> data = new List<CardData>(GameManager.Instance.gameContext.saveData.playerData.cards.gun);
           gun.LoadData(data);
    }
    #endregion
    #region LoadCard
  
    #endregion
    #region data
    public void PoolBackCard(ACardObject card,string key)
    {
        poolManager.ReturnObject(key, card.gameObject);
    }
    //카드 추가 카드 데이터로 
    public ACard GetCardByCardData<T>(CardData data) where T : ACard
    {
        
        CardTemplate cardTemplate = data.cardTemplate;
       
        ACard card = new GameObject("card" + cardTemplate.ID).AddComponent<T>();
        AddEffect(card, cardTemplate);
        return card;
    }
    public ACardObject GetCardObjectByCardData(CardData data) 
    {

        CardTemplate cardTemplate = data.cardTemplate;
        GameObject cardPrefab;
        if (data.cardTemplate.isGun)
        {
            cardPrefab = poolManager.GetObject("BulletCardObject");
            if (cardPrefab == null)
                cardPrefab = Resources.Load<GameObject>("Prefabs/Card/BulletCardObject");
        }
        else if (findHeal(data.cardTemplate))
        {
            cardPrefab = poolManager.GetObject("HealCardObject");
            if (cardPrefab == null)
            cardPrefab = Resources.Load<GameObject>("Prefabs/Card/HealCardObject");
        }
        else
        {
            cardPrefab = poolManager.GetObject("CardObject");
            if (cardPrefab == null)
             cardPrefab = Resources.Load<GameObject>("Prefabs/Card/CardObject");

        }
        ACardObject card = cardPrefab.GetComponent<ACardObject>();

        if (!data.isFusion)
            AddEffect(card, cardTemplate);
        else
            LoadFusionCard(card,data);
        card.gameObject.SetActive(true);
        return card;
    }
    public bool findHeal(CardTemplate card)
    {
        foreach(var effect in card.effects)
        {
            if (effect >= 200 && effect <= 500||(effect>=10000&&effect<20000))
                return true;
        }
        return false;
    }
    public CardInfoObject GetCardInfoObjectByCardData(CardData data)
    {
      
        CardTemplate cardTemplate = data.CloneCardData().cardTemplate;
        GameObject cardPrefab = poolManager.GetObject("CardImage");
        CardInfoObject card = cardPrefab.GetComponent<CardInfoObject>();
        if (!data.isFusion)
            AddEffect(card, cardTemplate);
        else
            LoadFusionCard(card, data);
        card.gameObject.SetActive(true);
        return card;
    }
    public CardSelectObject GetCardSelectInfoObjectByID(int id)
    {
        CardTemplate cardTemplate = GameManager.Instance.dataManager.GetTemplate<CardTemplate>(id);
        if (cardTemplate == null) cardTemplate = GameManager.Instance.dataManager.GetTemplate<CardTemplate>(100);
        GameObject cardPrefab = poolManager.GetObject("CardImageSelect");
        CardSelectObject card = cardPrefab.GetComponent<CardSelectObject>();

        AddEffect(card, cardTemplate);
        card.gameObject.SetActive(true);
        return card;
    }
    public CardInfoObject GetCardInfoObjectByID(int id)
    {
        CardTemplate cardTemplate = GameManager.Instance.dataManager.GetTemplate<CardTemplate>(id);
    
        GameObject cardPrefab = poolManager.GetObject("CardImage");
        CardInfoObject card = cardPrefab.GetComponent<CardInfoObject>();

        AddEffect(card, cardTemplate);
        card.gameObject.SetActive(true);
        return card;
    }
    public DragableCardInfoObject GetDragCardInfoObjectByCardData(CardData data)
    {

        CardTemplate cardTemplate = data.CloneCardData().cardTemplate;

        GameObject cardPrefab = poolManager.GetObject("CardImageDrag");
        DragableCardInfoObject card = Instantiate(cardPrefab).GetComponent<DragableCardInfoObject>();
        if (!data.isFusion)
            AddEffect(card, cardTemplate);
        else
            LoadFusionCard(card,data);
   
        card.gameObject.SetActive(true);
        return card;
    }
    private void AddEffect(ACard card, CardTemplate cardTemplate)
    {
        //GameObject effectContainer = new GameObject("EffectContainer");
       // effectContainer.transform.SetParent(card.transform);
     
        card.ResetEffects();
        if (cardTemplate.effects != null) { 
            //카드 매니저로 분리 필요. 
            foreach (int effectId in cardTemplate.effects)
            {
                AddCardEffect(effectId,card.gameObject, card);

                }
            card.Init(cardTemplate);
        }
    }
    //카드 추가
    public ACard GetCard(int id)
    {
        CardTemplate cardTemplate = GameManager.Instance.dataManager.GetTemplate<CardTemplate>(id);
        ACard card = new GameObject("card" + id).AddComponent<NormalCard>();

        GameObject effectContainer = new GameObject("EffectContainer");
        effectContainer.transform.SetParent(card.transform);
        if (cardTemplate.effects != null)
            //카드 매니저로 분리 필요. 
            foreach (int effectId in cardTemplate.effects)
            {
                AddCardEffect(effectId, effectContainer, card);

            }
        card.Init(cardTemplate);
        return card;
    }
    //카드 효과 추가
    public void AddCardEffect(int id, GameObject container, ACard card)
    {
        CardEffectTemplate effectTemplate = GameManager.Instance.dataManager.GetTemplate<CardEffectTemplate>(id);
        ACardEffect effect = null;
        switch ((CardEnum.EffectType)effectTemplate.type)
        {
            case CardEnum.EffectType.Attack:
                effect = container.AddComponent<AttackEffect>();

                break;
            case CardEnum.EffectType.Move:
                effect = container.AddComponent<MoveEffect>();
                break;
            case CardEnum.EffectType.Heal:
                effect = container.AddComponent<HealEffect>();
                break;
            case CardEnum.EffectType.Armor:
                effect = container.AddComponent<ArmorEffect>();
                break;
            case CardEnum.EffectType.Draw:
                effect = container.AddComponent<DrawEffect>();
            break;
                case CardEnum.EffectType.Stun:
                case CardEnum.EffectType.Poison:
                case CardEnum.EffectType.Burn:

                effect  = container.AddComponent<StatusEffectEffect>();
                break;
          
        }
        if (effect != null)
        {
            effect.Init(effectTemplate);
            card.AddEffect(effect);
        }

    }
    public void LoadFusionCard(ACard card ,CardData datas)
    {
        foreach (FusionCardEffectData data in datas.fusioneffects)
        {
            AddFusionCardEffect(data, card.gameObject, card);
        }


        card.SetFusionData(datas);
        card.Fusion();
        
    }
    public void LoadFusionCard(ACard card)
    {
        card.ResetEffects();
        foreach (FusionCardEffectData data in card.Data.fusioneffects)
        {
            AddFusionCardEffect(data, card.gameObject, card);
        }
        card.Fusion();
    }
    public void AddFusionCardEffect(FusionCardEffectData data , GameObject container,ACard card)
    {
        CardEffectTemplate effectTemplate= data.template;
        ACardEffect effect = null;
        switch ((CardEnum.EffectType)effectTemplate.type)
        {
            case CardEnum.EffectType.Attack:
                effect = container.AddComponent<AttackEffect>();

                break;
            case CardEnum.EffectType.Move:
                effect = container.AddComponent<MoveEffect>();
                break;
            case CardEnum.EffectType.Heal:
                effect = container.AddComponent<HealEffect>();
                break;
            case CardEnum.EffectType.Armor:
                effect = container.AddComponent<ArmorEffect>();
                break;
            case CardEnum.EffectType.Draw:
                effect = container.AddComponent<DrawEffect>();
                break;
            case CardEnum.EffectType.Stun:
            case CardEnum.EffectType.Poison:
            case CardEnum.EffectType.Burn:

                effect = container.AddComponent<StatusEffectEffect>();
                break;

        }
        if (effect != null)
        {
            effect.Init(effectTemplate);
            card.AddEffect(effect);
        }

    }

    public string MakeDescription(ACard card)
    {
        string text = null;
        return text;
    }
    #endregion
}
