using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ReinforceCard : MonoBehaviour
{
    [SerializeField]
    OneCardDropZone baseCardZone;
    [SerializeField]
    OneCardDropZone fusionCardZone;
    [SerializeField]
    GameObject root;
    ACard baseCard;
    ACard fusionCard;
    CardManager cardManager;
    [SerializeField]
    CardShowDragAble show;
    [SerializeField]
    TextMeshProUGUI num;
    [SerializeField]
    Button backBtn;
    [SerializeField]
    Button fusionBtn;
    [SerializeField]
    Button OpenBtn;
    [SerializeField]
    public Canvas can;
    [SerializeField]
    int maxAddNum=3;
    int nowNum;
    private void Awake()
    {
        cardManager = GameManager.Instance.cardManager;     nowNum = GameManager.Instance.gameContext.saveData.nowReinforceNum;
        num.text = nowNum.ToString();
        
    }
    private void OnEnable()
    {
        baseCardZone.Init(this);
        fusionCardZone.Init(this);
        OpenBtn.onClick.AddListener(Open);
        fusionBtn.onClick.AddListener(Fusion);
        backBtn.onClick.AddListener(Close);

        root.SetActive(false);
    }
    private void OnDisable()
    {
        Reset();
        backBtn?.onClick.RemoveAllListeners();
        fusionBtn?.onClick.RemoveAllListeners();
        OpenBtn?.onClick.RemoveAllListeners();
        
    }
    public void Close()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        Reset();
        root.SetActive(false);
        OpenBtn.gameObject.SetActive(true);
    }
    public void Open()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        root.SetActive(true);
        OpenBtn.gameObject.SetActive(false);
    }
    public void ResetCard(ACard card)
    {
        if (can == null) {
            show.AddCard3((
                card as DragableCardInfoObject));
            return;
        }
        card.transform.SetParent(can.transform);
        show.AddCard2((card as DragableCardInfoObject));
    }
    public void ResetCard(OneCardDropZone dropZone)
    {
        
        if (dropZone == baseCardZone)
        {
            if (baseCard != null)
            {
                ResetCard(baseCard);
            }
            baseCard = null;
        }
        else if (dropZone == fusionCardZone)
        {
  
            if (fusionCard != null)
                ResetCard(fusionCard);
            fusionCard = null;
        }
    }
    public void ResetCard2(OneCardDropZone dropZone)
    {
        if (dropZone == baseCardZone)
        {
     
            baseCard = null;
        }
        else if (dropZone == fusionCardZone)
        {

            fusionCard = null;
        }
    }
    public void Reset()
    {
     
        if (baseCard != null)
        {
            ResetCard(baseCard);
        }
        if (fusionCard != null)
        {
            ResetCard(fusionCard);
        }
        baseCard = null; 
        fusionCard = null;
    }
    public void SetCard(ACard card, OneCardDropZone dropZone)
    {
        if (dropZone == baseCardZone)
        {
            SetBaseCard(card);
        }
        else if (dropZone == fusionCardZone)
        {
            SetfusionCard(card);
        }
    }
    public ACard GetCard(OneCardDropZone dropZone)
    {
        if (dropZone == baseCardZone)
        {
            return baseCard;
        }
        else if (dropZone == fusionCardZone)
        {
            return fusionCard;
        }
        return null;
    }
    public void SetBaseCard(ACard card)
    {
       baseCard = card;
        
  
    }
    public void SetfusionCard(ACard card)
    {
        fusionCard = card;
       
    }
    public void Fusion()
    {
        if(!cardManager.CanMakeDeck())
            return;
        if(baseCard == null)
            return;
        if(fusionCard == null)
            return ;
        if(nowNum<=0)
            return ;
        nowNum--;
        GameManager.Instance.gameContext.saveData.nowReinforceNum--;
        num.text = nowNum.ToString();
        DragableCardInfoObject card = FusionCard(baseCard,fusionCard) as DragableCardInfoObject;


    }
    public ACard FusionCard(ACard _baseCard, ACard _fusionCard)
    {
        GameManager.Instance.audioManager.PlaySfx("Metal_03");
        var effects = _fusionCard.GetComponents<ACardEffect>();
        var baseEffects = _baseCard.GetComponents<ACardEffect>();

        var fusionDatas = new List<FusionCardEffectData>();

        foreach (var baseEffect in baseEffects)
        {
            var newTemplate = baseEffect.template.Clone(); 
            var fusionData = new FusionCardEffectData(newTemplate);

      
            var match = effects.FirstOrDefault(e => e.type == baseEffect.type);
            if (match != null)
            {
                fusionData.template.value += match.value;
            }

            fusionDatas.Add(fusionData);
        }

        foreach (var effect in effects)
        {
            bool exists = baseEffects.Any(be => be.type == effect.type);
            if (!exists)
            {
                var newTemplate = effect.template.Clone();
                fusionDatas.Add(new FusionCardEffectData(newTemplate));
            }
        }

       
        _baseCard.ChangeFData(fusionDatas);


        _baseCard.ResetEffects();
        GameManager.Instance.cardManager.LoadFusionCard(_baseCard);
        
        _fusionCard.End();
        fusionCard= null;
       

        return _baseCard;
    }

}
