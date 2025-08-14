using CardEnum;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ACardShowUI : MonoBehaviour
{
    [SerializeField]
    public RectTransform content;
    [SerializeField]
    protected RectTransform viewport;
   
    public Vector2 velocity;
    public bool dragging = false;
    [SerializeField]
    private float inertia = 0.9f;
    protected int columns;
    protected int rows;
    protected ACardZone zone;
    protected CardManager cardManager;
    protected List<CardInfoObject> cards = new List<CardInfoObject>();
    [Header("뷰 설정")]
    [SerializeField]
    private float scrollSpeed = 100f;
    [SerializeField]
    protected float spacingX = 10f;
    [SerializeField]
    protected float spacingY = 10f;
    protected float itemWidth = 200f;
    protected float itemHeight = 300f;
    [SerializeField]
    private Button SortButton;
    private TextMeshProUGUI sortText;
    protected CardEnum.SortMode sortMode = CardEnum.SortMode.Cost;
    public virtual void Init(ACardZone zone)
    {
        this.zone = zone;
        cardManager = GameManager.Instance.cardManager;
        SetCard();
        if (SortButton != null)
        {
            sortText = SortButton.GetComponentInChildren<TextMeshProUGUI>();
            SortButton.onClick.AddListener(ChangeMode);
            SortButton.onClick.AddListener(() => GameManager.Instance.audioManager.PlaySfx("Clicks-010"));
        }
    }
    #region Sort
    protected virtual void ChangeMode()
    {
        switch (sortMode)
        {
            case CardEnum.SortMode.Cost:
                sortMode = CardEnum.SortMode.Value;
                sortText.text = "값";
                break;
            case CardEnum.SortMode.Value:
                sortMode = CardEnum.SortMode.Range;
                sortText.text = "사거리";
                break;
            case CardEnum.SortMode.Range:
                sortMode = CardEnum.SortMode.Cost;
                sortText.text = "코스트";
                break;

        }
        Sort(sortMode);

    }
    public virtual void Sort(CardEnum.SortMode sortMode)
    {
        switch (sortMode)
        {
            case CardEnum.SortMode.Cost:
                CostSort();
                break;
            case CardEnum.SortMode.Value:
                ValueSort();
                break;
            case CardEnum.SortMode.Range:
                RangeSort();
                break;

        }
        SetCardPos();
     
    }
    protected void CostSort()
    {
    
        cards = cards.OrderBy(card =>card.Cost).ThenBy(card => card.ID).ToList();
    }
    protected void RangeSort()
    {

        cards = cards.OrderBy(card => card.Range).ThenBy(card => card.ID).ToList();
    }
    protected virtual void ValueSort()
    {

        cards = cards.OrderBy(card => card.Value).ThenBy(card => card.ID).ToList();
    }
    #endregion
    public virtual Vector2 FindPosition(int index)
    {

        int row = index / columns;
        int col = index % columns;

        float x = col * (itemWidth + spacingX);
        float y = -row * (itemHeight + spacingY); 

        return new Vector2(x, y);
    }
    public virtual void SetCardPos()
    {


        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform cardRect = cards[i].GetComponent<RectTransform>();
            if(content==null)
                return;
            RectTransform contentRect = content.GetComponent<RectTransform>();
            cardRect.SetParent(contentRect, false);
            (cards[i] as CardInfoObject).MoveCardRect(FindPosition(i));

        }
    }
    public virtual void SetCardPos2()
    {
  

        for (int i = 0; i < cards.Count; i++)
        {
            RectTransform cardRect = cards[i].GetComponent<RectTransform>();
            if (content == null)
                return;
            RectTransform contentRect = content.GetComponent<RectTransform>();
            cardRect.SetParent(contentRect, false);
            cardRect.anchoredPosition = FindPosition(i);


        }
    }
    public virtual void SetCard()
    {
        cards.Clear();
      columns = Mathf.FloorToInt((viewport.rect.width + spacingX) / (itemWidth + spacingX));
        CardInfoObject[] cardss = content.GetComponents<CardInfoObject>();
        foreach (CardInfoObject card in cardss)
        {
            card.EndCard();
        }
        for (int i=0;i<zone.GetCards().Count;i++) { 
            CardInfoObject cardInfo= cardManager.GetCardInfoObjectByCardData(zone.GetCards()[i]);
           
            cards.Add(cardInfo);
        }
        CostSort();
       
        for(int i=0;i<cards.Count;i++)
        {
            RectTransform cardRect = cards[i].GetComponent<RectTransform>();
            RectTransform contentRect = content.GetComponent<RectTransform>();
            cardRect.SetParent(contentRect, false);
            cardRect.anchoredPosition = FindPosition(i);
        }
    }


    
    protected virtual void Update()
    {
    
        if (!dragging && velocity.magnitude > 0.1f)
        {
            content.anchoredPosition += new Vector2(0,velocity.y);
            velocity *= inertia;
           ClampPosition();
        }
    }
    public void ClampPosition()
    {
        float contentHeight = ((cards.Count / columns)+1) * (itemHeight + spacingY);
        float viewportHeight = viewport.rect.height;

        float maxY = Mathf.Max(0, contentHeight - viewportHeight); 
        float minY = 0; 

        Vector2 pos = content.anchoredPosition;
        pos.y = Mathf.Clamp(pos.y, minY, maxY); 
        content.anchoredPosition = pos;
    }

}
