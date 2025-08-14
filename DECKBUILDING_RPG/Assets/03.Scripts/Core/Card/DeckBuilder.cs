using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeckBuilder : MonoBehaviour
{
 
    Deck deck;
    Pack pack;
    [SerializeField]
    CardShowDragAble deckUI;
    [SerializeField]
    CardShowDragAble packUI;
    [SerializeField]
    TextMeshProUGUI Num;
    void Start()
    {
        MakeDeck();
        MakePack();
        
        
    }
    void MakeDeck()
    {
        GameObject deckobj = new GameObject("Deck");
        deck = deckobj.AddComponent<Deck>();
        if (deck != null)
            deck.Init();
        deckUI.Init(deck);
        deckUI.isMakingDeck = true;
       // deck.show?.Invoke();
    }
    void MakePack()
    {
        GameObject packobj = new GameObject("Pack");
        pack = packobj.AddComponent<Pack>();
        pack.Init();
        packUI.Init(pack);
       // pack.ShowZone();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
