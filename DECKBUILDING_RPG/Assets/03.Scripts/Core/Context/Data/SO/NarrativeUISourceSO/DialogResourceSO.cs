using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="DialogSource", menuName ="Custom/DialogSource")]
public class DialogResourceSO : NarrativeResourceSO
{
    public List<DialogEntry> dialogEntryList;
    public float waitBetweenDialogSecond;
}

[Serializable]

public class DialogEntry
{
    public UnitEnum.UnitIdentifierType leftDialogSpriteType;
    public UnitEnum.UnitIdentifierType rightDialogSpriteType;
    public bool leftShade;
    public bool rightShade;
    [TextArea]public string dialogText;
    public bool leftFlip;
    public bool rightFlip;
}