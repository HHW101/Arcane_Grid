using System;
using UnityEngine;

public abstract class AInteractUI : MonoBehaviour
{
    [SerializeField]
    public NPCData npcData;

    public void Init(NPCData npcData)
    {
        this.npcData = npcData;
    }

    public abstract void Enable(Action onComplete);

    public abstract void Disable();
}