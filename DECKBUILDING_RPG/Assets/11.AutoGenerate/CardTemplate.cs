using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CardTemplate
{
public int ID;
public string nameKey;
public string flavorTextKey;
public List<int> effects;
public int cost;
public string img;
public bool isGun;
    public int price;
    public CardTemplate CloneCardTemplate()
    {
        return new CardTemplate
        {
            ID = this.ID,
            nameKey = this.nameKey,
            flavorTextKey = this.flavorTextKey,
            cost = this.cost,
            img = this.img,
            isGun = this.isGun,
            price = this.price
            ,
            effects = new List<int>(effects)

        };
    }
}
