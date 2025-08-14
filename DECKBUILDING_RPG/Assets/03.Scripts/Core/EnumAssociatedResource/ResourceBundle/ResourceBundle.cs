using UnityEngine;
using System;
using UnityEngine.UI;

[Serializable]
public class UnitIdentifierTypeResourceBundle
{
    public UnitEnum.UnitIdentifierType unitType;
    public Sprite dialogSprite;
    public Sprite infoSprite;
    [TextArea] public string name;
    [TextArea] public string description;
}

[Serializable]
public class TileTypeResourceBundle
{
    public TileEnum.TileType tileType;
    public Sprite infoSprite;
    [TextArea] public string name;
    [TextArea] public string description;
}

[Serializable]
public class JobTypeResourceBundle
{
    public JobEnum.Job job;
    public Sprite dialogSprite;
    public Sprite infoSprite;
    [TextArea] public string name;
    [TextArea] public string description;
}

[Serializable]
public class UnitTypeResourceBundle
{
    public UnitEnum.UnitType unitType;
    [TextArea] public string name;
    [TextArea] public string description;
}

[Serializable]
public class UnitCampTypeResourceBundle
{
    public UnitEnum.UnitCampType unitCampType;
    public Sprite ObjectTypeMarkerSprite;
}