using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropZoneToScroll : MonoBehaviour,IDropZoneUI
{
    [SerializeField]
    AZoneBaseUI zone;
    public void OnDrop(ACard card)
    {
        zone.OnDrop(card);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
