using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDescriptionUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cost;
    [SerializeField] TextMeshProUGUI flavor;

  
    public void OnEnable()
    {
        GameManager.Instance.cardManager.showCardInfo += show;
        //GameManager.Instance.cardManager.hideCardInfo += hide;
    }
    public void OnDisable()
    {
        GameManager.Instance.cardManager.showCardInfo -= show;
        //GameManager.Instance.cardManager.hideCardInfo -= hide;
    }
    public void show(ACard card)
    {
        cardName.text = card.Name;
        flavor.text = card.FlavorText;
        cost.text= card.Cost.ToString();
        description.text=card.makeDescription();
    }
    public void hide()
    {
        cardName.text = "";
        flavor.text = "";
        cost.text = "";
        description.text = "";
    }
}
