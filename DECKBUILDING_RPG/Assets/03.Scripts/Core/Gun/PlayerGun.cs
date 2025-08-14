using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerGun : AGun
{
    [SerializeField]
    CardGunAnimation anim;
    Animator animator;
    [SerializeField]
    Image gunIMG;
    [SerializeField]
    Image clip;
    [SerializeField]
    private float blinkTime = 1.0f;
    private WaitForSeconds blinkSecond;
    private Coroutine blinkCoroutine;
    private PlayerBulletUI ui;
    protected override void Awake()
    {
        base.Awake();
        blinkSecond = new WaitForSeconds(blinkTime);
        
        anim.Init();
        animator = GetComponent<Animator>();
    }
    public void SetUI(PlayerBulletUI ui)
    {
        this.ui = ui;
    }
    public virtual void CanUseEffect()
    {
        if(blinkCoroutine == null)
            blinkCoroutine = StartCoroutine(Blink());
    }
    public virtual void DisablceCanUseEffect()
    {
        if(blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine= null;
            gunIMG.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b, 1.0f);
            clip.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b, 1.0f);
        }
    }
    private IEnumerator Blink()
    {
        while (true)
        {
            gunIMG.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b,0.7f);
            clip.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b, 0.7f);
            yield return blinkSecond;
            gunIMG.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b, 1.0f);
            clip.color = new Color(gunIMG.color.r, gunIMG.color.g, gunIMG.color.b, 1.0f);
            yield return blinkSecond;
        }
    }
    public void EndLoad()
    {

    }
    public override void Shoot(Unit target)
    {
        base.Shoot(target);
        ui.ShowBullet();
    }
    public override void Load(ACard card)
    {
        StartLoadMotion();
        base.Load(card);
        
    }
    void StartLoadMotion()
    {
        animator.SetBool(anim.LoadParaHash, true);
      
    }
    public void EndLoadMotion()
    {
        Debug.Log("로드 " +
            "종료");
        animator.SetBool(anim.LoadParaHash, false);
     
    }
  
    protected void OnEnable()
    {
        
        animator.enabled = true;
    }
    protected void OnDisable()
    {
        animator.SetBool(anim.PutParaHash, true);
        animator.enabled = false;
    }

}
