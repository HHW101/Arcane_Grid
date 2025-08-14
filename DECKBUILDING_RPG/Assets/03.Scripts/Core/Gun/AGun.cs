using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AGun :MonoBehaviour
{
    private Queue<ABullet> bullets= new Queue<ABullet>();
    public bool IsEmpty { get { if (bullets.Count == 0) return true; return false; } }
    protected virtual void Awake()
    {
        TurnManager.OnGameStarted += ResetGun;
    }
    protected virtual void OnDestroy() { 
        TurnManager.OnGameStarted -= ResetGun;
    }
    public Queue<ABullet> Bullets {  get { return bullets; } }
    private void ResetGun()
    {
        bullets.Clear();
    }
   
    public virtual void Shoot(Unit target)
    {
             if (IsEmpty)
            return;
       bullets.Dequeue().Shoot(target);
        Save();
    }
    public virtual void LoadData(List<CardData> list)
    {
        foreach (CardData data in list)
        {
            NormalBullet bullet = new NormalBullet();
            bullet.Init(GameManager.Instance.cardManager.GetCardByCardData<NormalCard>(data.CloneCardData()));
            bullets.Enqueue(bullet);
        }
        
    }
    public virtual void Save()
    {
        GameManager.Instance.gameContext.saveData.playerData.cards.gun.Clear();
        foreach (ABullet data in bullets)
        {
            GameManager.Instance.gameContext.saveData.playerData.cards.gun.Add(data.Card.Data);
        }
       
    }
    public virtual void Load(ACard card)
    {
        NormalBullet bullet = new NormalBullet();
        bullet.Init(card);  
        bullets.Enqueue(bullet);
        GameManager.Instance.audioManager.PlaySfx("reload");
        Save();
    }
    public virtual bool CanUse()
    {
        //여기에 조건 추가 필요. 
        return true;
    }

    public ABullet GetFrontBullet()
    {
        return bullets.Peek();
    }
}
