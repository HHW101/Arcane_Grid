using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashZone : ACardZone
{
    
    public override void Remove(CardData card)
    {
        base.Remove(card);
        Save();
    }
    public override void Add(CardData card)
    {
        base.Add(card);
        Save();
    }

    public override void Save()
    {
 
        GameManager.Instance.cardManager.SaveTrashZone(cards);
    }
}
