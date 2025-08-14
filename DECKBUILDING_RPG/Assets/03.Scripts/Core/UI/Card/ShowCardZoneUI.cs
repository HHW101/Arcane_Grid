using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowCardZoneUI : AZoneBaseUI
{
    [SerializeField]
    private Button CancelBtn;
    [SerializeField]
    private Button DeckBtn;
    [SerializeField]
    private Button TrashZoneBtn;
    private bool isDeck;
    public GameObject ShowPanel;
    public GameObject ShowPart;
    private Deck deck;
    private TrashZone trashZone;
    private ACardZone zone;
    // Start is called before the first frame update
    void Start()
    {
        CancelBtn.onClick.AddListener(Cancel);
        DeckBtn.onClick.AddListener(ShowDeck);
        TrashZoneBtn.onClick.AddListener (ShowTrashZone);
        ShowPart.SetActive(false);
     
        
       
    }
    private void OnDisable()
    {
        CancelBtn?.onClick.RemoveListener(Cancel);
        DeckBtn?.onClick.RemoveListener(ShowDeck);
        TrashZoneBtn?.onClick.RemoveListener(ShowTrashZone);
    }

    private void Cancel()
    {

        ShowPart.SetActive(false);
        GetComponentInParent<Canvas>().sortingOrder = 0;
    }
    public void findPlayer()
    {
        deck = GameManager.Instance.gameContext.player.cardController.myDeck;
        trashZone = GameManager.Instance.gameContext.player.cardController.myTrashZone;
    }
    private void ShowDeck()
    {
        if (deck == null || trashZone == null)
            findPlayer();
        zone = deck;
        Show();
    }
    private void ShowTrashZone()
    {
        if (deck == null || trashZone == null)
            findPlayer();
        zone = trashZone;
        Show();
    }
    public override void Show()
    {
     

        ShowPart.SetActive(true );

        GetComponentInParent<Canvas>().sortingOrder = 100;

        Reset();
        foreach (CardData cardData in zone.GetCards())
        {
           ACard card =  GameManager.Instance.cardManager.GetCardByCardData<ACard>(cardData.CloneCardData());
            CardUIMaker ui = Instantiate(CardUIPrefab, ShowPanel.gameObject.transform, true);
          
            ACardBaseUI uibase = ui.makeCardUI(card, zone, this, CardEnum.CardUIMode.Show);
            cards.Add(uibase);

        }
    }
    
}
