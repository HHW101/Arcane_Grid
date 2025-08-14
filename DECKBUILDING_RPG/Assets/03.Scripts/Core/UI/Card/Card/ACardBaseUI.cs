using CardEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;

public abstract class ACardBaseUI : MonoBehaviour, IPointerDownHandler,IPointerUpHandler, IDragHandler
{


    protected TextMeshProUGUI cardName;

    protected TextMeshProUGUI cardDescription;

    private TextMeshProUGUI cardCost;

    protected TextMeshProUGUI cardValue;
    protected CardDragUI dragUI;
    protected CardDragUI DragPre;
    protected Canvas canvas;
    protected ACardZone zone;
    public BaseCardController cardController;
    protected Image image;
    protected CanvasGroup canvasGroup;
    protected RectTransform rect;
    protected CardEnum.CardUIMode mode;
    protected bool isDrag=false;
    protected Transform originalParent;
    protected LayerMask layermask = 1 << 6;
    protected ACard card;
    public ACard ACard { get { return card; } }
    protected AZoneBaseUI zoneUI;
  
    protected virtual void OnEnable()
    {
        image = GetComponent<Image>();
        canvas = GetComponentInParent<Canvas>();
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
        if (GameManager.Instance.gameContext.player != null)
            cardController = GameManager.Instance.gameContext.player.cardController;
    }
    protected virtual void OnDisable()
    {
        ACardEffect[] lastEffects = GetComponents<ACardEffect>();
        foreach (ACardEffect effect in lastEffects)
            Destroy(effect);
    }
    protected virtual void Selected()
    {
        Color c = image.color;
        c.a = 0.5f;
        image.color = c;
    }
    protected virtual void UnSelected()
    {
        Color c = image.color;
        c.a = 1.0f;
        image.color = c;
    }
    protected virtual void Click(PointerEventData eventData)
    {

    }
    protected virtual void ClickEnd(PointerEventData eventData)
    {

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Click(eventData);

    }
    public void OnDrag(PointerEventData eventData)
    {
        if (dragUI != null||isDrag)
            rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    


    public virtual void Init(ACard card)
    {
        cardName.text = card.Name;
        cardDescription.text = card.FlavorText;
        cardCost.text = card.Cost.ToString();
        cardValue.text = card.Value.ToString();
        this.card = card;

        this.mode = CardUIMode.Drop;
    }

    public virtual void Init(ACard card, ACardZone zone,AZoneBaseUI zoneUI,CardDragUI dragUI,TextMeshProUGUI cardN,TextMeshProUGUI cardDesc, TextMeshProUGUI cardC, TextMeshProUGUI cardV)
    {
        cardName = cardN;
        cardDescription = cardDesc;
        cardCost = cardC;
        cardValue = cardV;
        this.zoneUI = zoneUI;
        cardName.text = card.Name;
        cardDescription.text = card.makeShortDescription();
        cardCost.text = card.Cost.ToString();
        cardValue.text = card.Value.ToString();
        this.card = card;
        this.zone = zone;
        DragPre = dragUI;


    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ClickEnd(eventData);
    }
}
