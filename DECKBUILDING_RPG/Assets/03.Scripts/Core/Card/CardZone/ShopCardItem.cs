using System;
using UnityEngine; 

[Serializable] 
public struct ShopCardItem
{
    public ACard Card;
    public int Quantity;

    public ShopCardItem(ACard card, int quantity)
    {
        Card = card;
        Quantity = quantity;
    }
}
