using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ABullet 
{
    private ACard card;
    public ACard Card { get { return card; } }
    private int duration;
    public void Init(ACard card)
    {
        this.card = card;
    }
    public void OverTurn()
    {
        duration--;
    }
    public void Shoot(Unit target)
    {

        target.TakeDamage(card.Value);
        

    }
}
