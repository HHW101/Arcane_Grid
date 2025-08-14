using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimationController : MonoBehaviour // npc 와 플레이어의 공통기능을 구현한다
{
    protected Animator animator;
    protected virtual void Awake() 
    {
        animator = this.GetComponent<Animator>();
    } 


    public virtual void PlayAttack() // 공격 할때
    {
        animator.SetTrigger("Attack");
    }


    public virtual void PlayHit() //공격받을때
    {
        animator.SetTrigger("Hit");
    }


    public virtual void PlayDie() //죽었을때
    {
        animator.SetTrigger("Die");
    }


    public virtual void PlayMove()//이동할때
    {
        animator.SetTrigger("Move");
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
