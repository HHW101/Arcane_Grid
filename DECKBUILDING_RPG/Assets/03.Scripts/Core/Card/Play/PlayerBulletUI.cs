using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletUI : MonoBehaviour
{

    [SerializeField]
    Transform startPos;
    [SerializeField]
    float offset=100;

    public StageManager stageManager;
    public Action action;
    protected AGun gun;
    protected List<ACardObject> bullets = new List<ACardObject>();
    public void Init(AGun gun)
    {
        this.gun = gun;
        stageManager = GameManager.Instance.campaignManager.stageManager;
        (gun as PlayerGun).SetUI(this);
    }


    public void ShowBullet()
    {
        int count = gun.Bullets.Count;
        int index = 0;
        foreach (ACardObject obj in bullets)
        {
            Destroy(obj.gameObject);
        }
        bullets.Clear();
        foreach (ABullet bullet in gun.Bullets) {
            
            Vector2 pos = SetPosition(index, count);
            index++;
           ACardObject obj=  GameManager.Instance.cardManager.GetCardObjectByCardData(bullet.Card.Data);
            obj.transform.localScale = new Vector2(50, 50);
            obj.transform.position = pos;
            (obj as PlayCardObject).NotDragable = true;
            obj.transform.SetParent(startPos, true);
            bullets.Add(obj);
        }
    }
 
    public Vector2 SetPosition(int index, int count)
    {

  
        Vector2 localPos =  -index * new Vector2(0, offset);
        Vector2 pos = startPos.TransformPoint(localPos);
        return pos;
    }
    private void OnCancelButtonClicked()
    {
        stageManager.CancelAction();

    }


}
