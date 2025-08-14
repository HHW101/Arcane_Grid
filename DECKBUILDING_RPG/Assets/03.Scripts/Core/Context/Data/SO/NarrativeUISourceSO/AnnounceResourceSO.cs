using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Announce", menuName = "Custom/AnnounceSource")]
public class AnnounceResourceSO : NarrativeResourceSO
{ 
    public List<string> announceStringList;
    public float waitBeforeHideSecond;
}