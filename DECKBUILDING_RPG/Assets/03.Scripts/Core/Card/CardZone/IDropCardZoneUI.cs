using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDropCardZoneUI 
{

    public void RemoveCard(DragableCardInfoObject card);
    public void ClickCard(DragableCardInfoObject card);
}
