using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
public abstract class ACardObject : ACard
{
    private bool isClicked =false;
    public bool IsClicked {  get { return isClicked; } }
    protected PlayerCardController playerCardController;
    private float lerpDistance = 0.01f;
    [SerializeField] protected TextMeshProUGUI nameText;
    [SerializeField] protected TextMeshProUGUI descriptionText;
    [SerializeField] protected TextMeshProUGUI costText;
    [SerializeField] protected TextMeshProUGUI valueText;
    
    protected CanvasGroup canvasGroup;
    protected Sprite img;

    public virtual void EndCard()
    {
        Destroy(gameObject);
    }
    public override void End()
    {
        EndCard();
    }
    public override void Init(CardTemplate cardTemplate)
    {
        base.Init(cardTemplate);
        nameText.text = Name;
        if(isFusion)
            nameText.color = Color.red;
        descriptionText.text = makeShortDescription();
        costText.text = cost.ToString();
        valueText.text = value.ToString();
      
        img = GameManager.Instance.cardManager.GetSprite(id);
    
            

    }

    protected override void OnEnable()
    {
        base.OnEnable();
        canvasGroup = GetComponent<CanvasGroup>();
        if(canvasGroup ==null)
            canvasGroup= gameObject.AddComponent<CanvasGroup>();
    }
    #region Pointer
    protected override void OnDisable()
    {
        base.OnDisable();
        canvasGroup.alpha = 1.0f;
    }

    public virtual void InitAfterSetParent()
    {

    }
    public override void Selected()
    {

    }
    public override void UnSelected()
    {

    }
    public virtual void Drag(Vector2 mousePos,Camera cam)
    {


    }
    #endregion
  
   
    #region move
    public void MoveCard( Vector2 destination, bool isTrash=false)
    {

        StartCoroutine(MoveCoroutine(destination,isTrash));

    }

    IEnumerator MoveCoroutine( Vector2 destination, bool isTrash)
    {
        Vector2 originalPos = transform.position;
        Vector2 destinationPos = destination;
        float elapsedTime = 0f;
        float duration = 0.5f; 

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(originalPos, destination, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
       transform.position = destinationPos;
   
        if(isTrash)
            EndCard();
    }
#endregion
    public override void Delete()
    {
        throw new System.NotImplementedException();
    }

    public override void Show()
    {
        throw new System.NotImplementedException();
    }
}
