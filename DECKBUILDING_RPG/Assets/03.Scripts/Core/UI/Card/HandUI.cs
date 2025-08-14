using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class HandUI : AZoneBaseUI
{
    // Start is called before the first frame update
    Hand hand;
    
    CanvasGroup canvasGroup;
    public void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        GameManager.Instance.cardManager.AddHandUI += Init;
        GameManager.Instance.cardManager.onClick += Click;
        HideUI();
    }
    protected override void ChangeMode()
    {
        base.ChangeMode();
        
    }
    private void OnDestroy()
    {
        if(GameManager.Instance!=null&& GameManager.Instance.cardManager!=null )
            GameManager.Instance.cardManager.onClick -= Click;
    }
    public void Init()
    {
        hand = GameManager.Instance.cardManager.AddHand();
        hand.show += Show;
     

    }
    public void Click()
    {
        if(GameManager.Instance.gameContext.isSelectingUseCard)
            ShowUI();
        else
            HideUI();
    }
    public void ShowUI()
    {
        Logger.Log("HandUI Show");
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HideUI()
    {
        Logger.Log("HandUI HIde");
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnDisable()
    {
        hand.show -= Show;
        if(GameManager.Instance != null && GameManager.Instance.cardManager != null)
          GameManager.Instance.cardManager.AddHandUI -= Init;

    }
    public void Reset()
    {
        int count = cards.Count;
        for (int i = count-1; i >= 0; i--)
        {
            if (cards[i] != null && cards[i].gameObject != null)
            {
                Destroy(cards[i].gameObject);
            }
        }
      
        cards.Clear();
    }
    public override void Show()
    {
       Reset();
        foreach (CardData cardData in hand.GetCards())
        {
            ACard card = GameManager.Instance.cardManager.GetCardByCardData<ACard>(cardData.CloneCardData());
            //CardUIMaker ui = Instantiate(CardUIPrefab, this.transform, true);
            
            //ACardBaseUI u= ui.makeCardUI(card, hand,this,CardEnum.CardUIMode.Hand);
            //cards.Add(u);

        }
    }
}
