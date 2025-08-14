using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcAnimationController : BaseAnimationController
{

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PlayAttack()
    {
        base.PlayAttack();
        Debug.Log("PlayerAttack");
    }

    public override void PlayDie()
    {
        base.PlayDie();
        Debug.Log("PlayerDie");
    }


    public override void PlayHit()
    {
        base.PlayHit();
        Debug.Log("PlayerHit");
    }


    public override void PlayMove()
    {
        base.PlayMove();
        Debug.Log("PlayerMove");
    }






    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayAttack();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            PlayDie();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            PlayHit();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            PlayMove();
        }
    }
}
