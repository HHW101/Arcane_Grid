using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCardController : BaseCardController 
{
    GameContext gameContext;
    APlayer player;
    public APlayer Player;
    private int drawCardNum =5;
    public Action onClick;

    public override Hand MyHand { get => base.MyHand as ShowHand; set => base.MyHand = value; }

    public override void Init(GameObject unit)
    {
        base.Init(unit);
        ShowHand prefab = Resources.Load<ShowHand>("Prefabs/Card/Hand");
        MyHand = Instantiate(prefab);
        
        MyHand.gameObject.transform.position=new Vector2(100,100);
        //GameManager.Instance.cardManager.AddHandUI?.Invoke();

        GameManager.Instance.cardManager.LoadDeck(myDeck);
        GameManager.Instance.cardManager.LoadHand(MyHand);
        GameManager.Instance.cardManager.LoadTrashZone(myTrashZone);

        MyHand.Init(this);
        onClick = GameManager.Instance.cardManager.onClick;
        isPlaying = true;
       // handUI = GameManager.Instance.cardManager.FindHand();
    }
    

    public void SuffleDeck()
    {
        myDeck.Shuffle();
    }
    private void OnEnable()
    {
        TurnManager.OnGameStarted += SuffleDeck;
        TurnManager.OnGameStarted += setStart;
        TurnManager.OnTurnEnded += ThrowCard;
        TurnManager.OnTurnStarted += DrawCards;

    }
    private void OnDisable()
    {
        TurnManager.OnGameStarted -= SuffleDeck;
        TurnManager.OnGameStarted-= setStart;
        TurnManager.OnTurnEnded -= ThrowCard;
        TurnManager.OnTurnStarted -= DrawCards;
      

    }
    private void setStart()=> isStarted = false;
    public override void DrawCard()
    {
        if (myDeck.GetCards().Count == 0)
            ResetTrashZone();
        CardData card = myDeck.GiveCard();
        if (MyHand.GetCards().Count > 7)
        {
            myTrashZone.Add(card);
            (MyHand as ShowHand).Trash(card);
        }
        else
            (MyHand as ShowHand).Draw(card);
    }
    public override void ThrowCard()
    {
        Debug.Log("카드 제거");
        int count = MyHand.GetCards().Count;
        for (int i = count; i > 0; i--)
        {
          
           Debug.Log("카드 제거 횟수"+MyHand.GetCards().Count);
            myTrashZone.Add((MyHand as ShowHand).Trash());

        }

    }
    public void DrawCards()
    {
        Debug.Log("처음 카드 드로우");
        if (isStarted)
        {
            Debug.Log("드로우 안함");
            isStarted = false;
            return;
        }
        if (MyHand.GetCards().Count == 0)
        {
            MyHand.nowNum = drawCardNum;
            for (int i = 0; i < drawCardNum; i++)
            {
                DrawCard();
            }
        }
        Invoke("HideTemp", 1.0f);
    }
    public void HideTemp()
    {
        (MyHand as ShowHand).HideUI();
    }
    public void Start()
    {
        gameContext = GameManager.Instance.gameContext;
        Debug.Log(gameContext.player);
        player = gameContext.player;
    }
    public void makePlayerDeck()
    {
        int count = MyHand.GetCards().Count;
        for (int i = count; i >= 0; i--)
        {
            myDeck.Add(MyHand.GiveCard());

        }
        count = myTrashZone.GetCards().Count;
        for (int i = count; i >= 0; i--)
        {
            myDeck.Add(myTrashZone.GiveCard());

        }
    }
    public override bool CanUse(ACard card)
    {
        return card.CanUse(player);
    }
   
    public override void Use(ACard card, ATile target=null)
    {
        if (player == null)
            player = gameContext.player;

        if (!card.CanUse(target, gameContext.player))
            return;
        card.Use(target, player);
        myTrashZone.Add(MyHand.GiveCard(card.Data));
        
    }
    public override AGun AddGun()
    {
        return (MyHand as ShowHand).gun;
    }
}
