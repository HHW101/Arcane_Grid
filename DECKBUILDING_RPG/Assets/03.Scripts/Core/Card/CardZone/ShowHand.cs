using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class ShowHand : Hand {
    public Transform deckRoot;
    public Transform TrashZoneRoot;
    public Transform Hand;
    [SerializeField] private ACardShowUI showZone;
    [SerializeField] private Button deckButton;
    [SerializeField] private Button trashZoneButton;
    [SerializeField] private Button backButton;
    private List<ACardObject> cardObjects;
    private PlayerCardController cardController;
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField]
    public PlayerGun gun;
    [SerializeField] private float handSize;
    private CanvasGroup canvasGroup;
    private GraphicRaycaster graphicRaycaster;
    PointerEventData pointerEventData;
    private Canvas canvas;
    [SerializeField] GameObject Blocker;
    
    public bool isPlaying;
   
    enum Root
    {
        deck,trashZone,hand
    }
    public void OnEnable()
    {
        deckButton.onClick.RemoveAllListeners();
        trashZoneButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        deckButton.onClick.AddListener(OpenDeck);
        trashZoneButton.onClick.AddListener(OpenTrashZone);
        backButton.onClick.AddListener(CloseZone);
        GameManager.Instance.cardManager.LoadGun(gun);
        CloseZone();
       // CheckLoaded();

    }
    public void OnDisable()
    {
        CloseZone();
        deckButton.onClick.RemoveAllListeners();
        trashZoneButton.onClick.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        if(GameManager.Instance.cardManager != null)
        GameManager.Instance.cardManager.onClick -= Click;
    }
    public override void Init(BaseCardController cardController)
    {
        base.Init(cardController);
        this.cardController = controller as PlayerCardController;
        if (!this.TryGetComponent<CanvasGroup>(out CanvasGroup can))
        {
            can = this.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup = can;
        GameManager.Instance.cardManager.onClick += Click;
        canvas = GetComponentInChildren<Canvas>();
        graphicRaycaster= canvas.GetComponent<GraphicRaycaster>();
        pointerEventData = new PointerEventData(null);
        HideUI();
    }
    private void Update()
    {
        if (!cardController.IsPlaying)
            return;
        if(!isPlaying) 
            return;

        Vector2 mousePos = HandleMouseInput(out bool isClicked,out bool isUnClicked);
        if (Clicked(mousePos, isClicked)) {

            return;
        }
        if (UnClicked(mousePos, isUnClicked)) {
  
            return;
        
        }
        if(cardController.SelectedCard != null)
        Drag(mousePos);
        
    }
    #region button
    public void OpenZone()
    {
        backButton.gameObject.SetActive(true);
        showZone.gameObject.SetActive(true);
    }
    public void OpenDeck()
    {
        OpenZone();
        showZone.Init(cardController.myDeck);

    }
    public void OpenTrashZone()
    {
        OpenZone();
        showZone.Init(cardController.myTrashZone);
    }
    public void CloseZone()
    {
        backButton.gameObject.SetActive(false);
        showZone.gameObject.SetActive(false);
    }
    #endregion
    #region ShowUI
    public void Click()
    {
        if (GameManager.Instance.gameContext.isSelectingUseCard)
            ShowUI();
        else
            HideUI();
    }


    [ContextMenu("ShowUI")]
    public void ShowUI()
    {
        Logger.Log("ShowHand Show ");
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isPlaying = true;
     
        Blocker.SetActive(false);
    }
    
    [ContextMenu("HideUI")]
    public void HideUI()
    {
        Logger.Log("ShowHand Hide");
        isPlaying = false;
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        CloseZone();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
  
        Blocker.SetActive(true);
        Blocker.transform.SetAsLastSibling();
    }
    #endregion
    #region Input
    protected Vector2 HandleMouseInput(out bool isClicked,out bool isUnClicked)
    {
        Vector2 mousePos = Input.mousePosition;
        isClicked = Input.GetMouseButtonDown(0);
        isUnClicked = Input.GetMouseButtonUp(0);
        return mousePos;
    }
   protected bool Clicked(Vector2 mousePos, bool isClicked)
    {
  
        if (isClicked && cardController.SelectedCard == null)
        {
            if (FindCard(mousePos, out ACardObject card))
            {

                if(!card.CanUse(GameManager.Instance.gameContext.player))
                    return false;

                cardController.SetSelectedCard(card);
                card.Selected();
                if(card.Data.cardTemplate.isGun)
                    gun.CanUseEffect();
                return true;
            }
        }
        return false;
    }
    public bool UnClicked(Vector2 mousePos, bool isUnClicked)
    {
        if (isUnClicked )
        {
            if (cardController.SelectedCard != null) { 
                ACardObject nowCard = cardController.SelectedCard as ACardObject;
                //cardController.selectedCard = null;
                if(nowCard.Data.cardTemplate.isGun)
                {
                    if (FindTarget(mousePos))
                    {

                        cardController.Use(nowCard);
                        RemoveCard(nowCard);
                        nowCard.End();
                    }
                    gun.DisablceCanUseEffect();
                    nowCard.UnSelected();
                }
                else if (GameManager.Instance.cardManager.Target != null)
                {
                    cardController.Use(nowCard, GameManager.Instance.cardManager.Target);
                    RemoveCard(nowCard);
                    nowCard.End();
                }
                else if (FindTarget(mousePos, out ATile target))
                {

                    cardController.Use(nowCard, target);
                    RemoveCard(nowCard);
                    nowCard.End();
                }
                else
                    nowCard.UnSelected();
                 nowNum = cardObjects.Count;
                cardController.UnSelectedCard();
                ResetPosition();
          

            }

            return true;
        }
        return false;
    }
    protected virtual void RemoveCard(ACardObject card)
    {
        cards.Remove(card.Data);
        cardObjects.Remove(card);
        if(cards.Count !=cardObjects.Count)
        {
            ForseReset(cardObjects);
        }
        ResetPosition();
        Save();
    }
    protected bool FindTarget(Vector2 mousePos, out ATile target)
    {
        target = null;
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos, tileLayer);

        if (hit != null)
        {
 
            if (hit.TryGetComponent<ATile>(out var findtile))
            {
          
                if (cardController.SelectedCard.CanUse(findtile, GameManager.Instance.gameContext.player))
                {
                    target= findtile;
                    return true;
                }
                target = null;
            }

        }
        return false;
    }
    protected bool FindTarget(Vector2 mousePos)
    {

        
        
        List<RaycastResult> results = new List<RaycastResult>();
        pointerEventData.position = mousePos;
        graphicRaycaster.Raycast(pointerEventData, results);
        foreach (RaycastResult result in results) { 
            if(result.gameObject.TryGetComponent<AGun>(out var gun))
            {
                return true;
            }
        }
        
        return false;
    }
    protected void Drag(Vector2 mousePos)
    {
        (cardController.SelectedCard as ACardObject).Drag(mousePos,cam);
    }
    protected bool FindCard(Vector2 mousePos, out ACardObject card)
    {
        card = null;
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);
       
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null) {
        
            if (hit.TryGetComponent<ACardObject>(out var cardObj))
            {
                card = cardObj;
                return true;
            }

        }
        return false;
    }

    #endregion
    #region Control Card
    public override void LoadCards(List<CardData> cards)
    {

        base.LoadCards(cards);
        cardObjects = new List<ACardObject>();
        foreach (var card in cards)
        {
            ACardObject cardObj = GameManager.Instance.cardManager.GetCardObjectByCardData(card.CloneCardData());
            cardObjects.Add(cardObj);
            cardObj.gameObject.transform.SetParent(Hand);
            cardObj.transform.position = deckRoot.position;
            cardObj.InitAfterSetParent();
        }
        ResetPosition();
        Save();
    }
   
    public override void Save()
    {
       // cards.Clear();
        //foreach (var card in cardObjects)
        //{
        //    CardData data = card.Data;
       //     cards.Add(data);
      //  }
        base.Save();
    }
    public void Draw(CardData card) {
        Add(card);
        ACardObject cardObject = GameManager.Instance.cardManager.GetCardObjectByCardData(card.CloneCardData());
        cardObjects.Add(cardObject);

        if (cardObjects.Count > nowNum) { 
            nowNum = cardObjects.Count;
        }
        Vector2 cardPos = FindCardPosition(cardObjects.Count-1, cardObject);
        cardObject.transform.SetParent(Hand);
        cardObject.transform.position = deckRoot.position;
        cardObject.MoveCard(cardPos);
        cardObject.InitAfterSetParent();

    }
    
    public void Trash(CardData card)
    {
        ACardObject cardObject = GameManager.Instance.cardManager.GetCardObjectByCardData(card.CloneCardData());
        cardObject.transform.position = deckRoot.position;
        cardObject.MoveCard(TrashZoneRoot.position,true);
    }
    public void TrashToDeck(CardData card)
    {

    }
    public CardData Trash()
    {
        CardData data = GiveCard();
        ACardObject card = cardObjects[0];
        card.MoveCard(TrashZoneRoot.position,true);
        cardObjects.Remove(card);
       
        return data;
    }
    public override CardData GiveCard()
    {

        CardData data= base.GiveCard();
     
        return data;
    }
    public override CardData GiveCard(CardData card)
    {
        int index = cards.IndexOf(card);  

        if (index >= 0)
        {
            cards.RemoveAt(index);    
            cardObjects.RemoveAt(index);
        }

        return card;
    }
    public void Draw()
    {

    }
    public void Insert(int index, ACardObject card)
    {
        cards.Insert(index, card.Data);
        cardObjects.Insert(index, card);
        card.gameObject.transform.SetParent(Hand);
    }

    #endregion
    //Rect로 수정 하지 말자...
    private void ResetPosition()
    {
        nowNum = cardObjects.Count;
        for (int i = 0; i < cards.Count; i++)
        {
            cardObjects[i].MoveCard(FindCardPosition(i, cardObjects[i]));
        }
    }
    private Vector2 FindCardPosition(int index,ACard card)
    {
        float spacing = handSize / (nowNum > 1 ? (nowNum - 1f) : 1f);
        float startOffset = -spacing * (nowNum - 1) / 2f;
        float offset = startOffset + index * spacing;
        Vector2 localPos = new Vector2(0, offset);
        Vector2 pos = Hand.transform.TransformPoint(localPos);
        //Debug.Log($"{localPos}:{pos}");
        return pos;
    }
    

}
