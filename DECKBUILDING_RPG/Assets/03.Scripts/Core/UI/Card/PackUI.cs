using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR;

public class PackUI : AZoneBaseUI,IDropZoneUI
{
    // Start is called before the first frame update
    Pack pack;
  

    public void Init(Pack pack)
    {
        this.pack = pack;
        pack.show = Show;
    }
    public override void OnDrop(ACard card)
    {
        pack.Add(card.Data);
        Show();
    }


    private void OnDisable()
    {
        pack.show -= Show;

    }
    // Update is called once per frame

    public override void Show()
    {
        Reset();
        foreach (CardData cardData in pack.GetCards())
        {
            ACard card = GameManager.Instance.cardManager.GetCardByCardData<ACard>(cardData.CloneCardData());
            CardUIMaker ui = Instantiate(CardUIPrefab, this.transform,true);
            ACardBaseUI u = ui.makeCardUI(card,pack,this, CardEnum.CardUIMode.Pack);
            cards.Add(u);

        }
    }

}
