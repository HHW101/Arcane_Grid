using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pack : ACardZone
{
    //[SerializeField] Pack pk;

    public override void Save()
    {
        GameManager.Instance.cardManager.SavePack(cards);
    }
    // Start is called before the first frame update
    public void Init()
    {

        GameManager.Instance.cardManager.LoadPack(this);
    }
  
    public override void ShowZone()
    {
        base.ShowZone();
        show?.Invoke();
        
    }
}
