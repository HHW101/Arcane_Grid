using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCardObject :PlayCardObject
{
    public override void EndCard()
    {
        GameManager.Instance.cardManager.PoolBackCard(this, "BulletCardObject");
    }
    protected override void OnEnable()
    {
        base.OnEnable();
       // playerCardController = GameManager.Instance.gameContext.player.cardController as PlayerCardController;
    }
    public override void Drag(Vector2 mousePos, Camera cam)
    {
        Vector2 worldPos = cam.ScreenToWorldPoint(mousePos);
        gameObject.transform.position = worldPos;

    }
}
