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
public int rarity;
}
