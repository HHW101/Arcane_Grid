using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardUIMaker : MonoBehaviour
{
    [SerializeField]
    protected CardDragUI DragPre;
    [SerializeField]
    protected TextMeshProUGUI cardName;
    [SerializeField]
    protected TextMeshProUGUI cardDescription;
    [SerializeField]
    private TextMeshProUGUI cardCost;
    [SerializeField]
    protected TextMeshProUGUI cardValue;
    public ACardBaseUI makeCardUI(ACard card,ACardZone zone,AZoneBaseUI zoneUI,CardEnum.CardUIMode mode)
    {
        ACardBaseUI cardUI =null;
        switch (mode)
        {
            case CardEnum.CardUIMode.Hand:
                cardUI = gameObject.AddComponent<HandCardUI>();
                break;
            case CardEnum.CardUIMode.Deck:
            case CardEnum.CardUIMode.Pack:
                cardUI = gameObject.AddComponent<PackCardUI>();
                break;
            case CardEnum.CardUIMode.Drop:
                cardUI = gameObject.AddComponent<DropCardUI>();
                break;
            case CardEnum.CardUIMode.Show:
                cardUI = gameObject.AddComponent<ShowCardUI>();
                break;

        }
        cardUI.Init(card, zone,zoneUI,DragPre,cardName,cardDescription,cardCost,cardValue);
        return cardUI;
    }

}
