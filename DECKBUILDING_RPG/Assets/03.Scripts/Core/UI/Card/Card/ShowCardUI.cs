using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowCardUI : ACardBaseUI
{
    protected override void Click(PointerEventData eventData)
    {
        zoneUI.CardDescriptionUI.show(card);
       
    }
}
