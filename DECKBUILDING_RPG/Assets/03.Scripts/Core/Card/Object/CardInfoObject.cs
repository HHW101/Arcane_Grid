using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CardInfoObject : ACardObject
{
    protected RectTransform rect;
   public RectTransform Rect;
    [SerializeField] protected Image cardImage;
   // CanvasGroup canvasGroup;
    public void InfoInit(Sprite image,string name,string text,int cost, int value)
    {
       cardImage.sprite = image;
        if (isFusion)
            cardImage.color = Color.red;
        nameText.text = name;
        descriptionText.text = text;
        costText.text = cost.ToString();
        valueText.text = value.ToString();
    }
    public override void Init(CardTemplate cardTemplate)
    {
        base.Init(cardTemplate);
        cardImage.sprite = img;
        //if (isFusion)
            //cardImage.color = Color.red;

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        rect = GetComponent<RectTransform>();
    
    }
    public bool forseStop = false;
    public void ForseStop()
    {
        StopAllCoroutines();
    }
    public void MoveCardRect(Vector2 destination, bool isTrash = false)
    {

        StartCoroutine(MoveCoroutine(destination, isTrash));

    }

    IEnumerator MoveCoroutine(Vector2 destination, bool isTrash)
    {
        if(forseStop)
        {
            forseStop = false;
            yield break;
        }
        Vector2 originalPos = rect.anchoredPosition;
        Vector2 destinationPos = destination;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            if (forseStop)
            {
                forseStop = false;
                yield break;
            }
            rect.anchoredPosition = Vector2.Lerp(originalPos, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = destinationPos;

        if (isTrash)
            EndCard();
    }
    public override void EndCard()
    {
        GameManager.Instance.cardManager.PoolBackCard(this, "CardImage");
    }
}
