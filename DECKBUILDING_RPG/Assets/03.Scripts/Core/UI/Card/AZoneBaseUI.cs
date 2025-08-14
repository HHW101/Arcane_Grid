using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class AZoneBaseUI : MonoBehaviour
{
    [SerializeField] CardDescriptionUI cardDescriptionUI;
    public CardDescriptionUI CardDescriptionUI { get { return cardDescriptionUI; } }
    protected List<ACardBaseUI> cards = new List<ACardBaseUI>();
    protected CardEnum.SortMode sortMode= CardEnum.SortMode.Cost;
    [SerializeField]
    private Button sortButton;
    private TextMeshProUGUI sortText;
    [SerializeField]
    public CardUIMaker CardUIPrefab;
    private void Start()
    {
        if (sortButton != null)
        {
            sortButton.onClick.AddListener(ChangeMode);
            sortText=sortButton.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            sortText.text = "코스트";
        }
    }
    public virtual void OnDrop(ACard card)
    {
      
    }
    public virtual void Reset()
    {
        int count = cards.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            Destroy(cards[i].gameObject);
        }

        cards.Clear();
    }
    protected virtual void ChangeMode()
    {
        switch (sortMode)
        {
            case CardEnum.SortMode.Cost:
                sortMode = CardEnum.SortMode.Value;
                sortText.text = "값";
                break;
            case CardEnum.SortMode.Value:
                sortMode= CardEnum.SortMode.Range;
                sortText.text = "사거리";
                break;
            case CardEnum.SortMode.Range:
                sortMode = CardEnum.SortMode.Cost;
                sortText.text = "코스트";
                break;

        }


    }
    public virtual void Show()
    {

    }
 
    public virtual void SaveZone(ACard card)
    {

    }
}
