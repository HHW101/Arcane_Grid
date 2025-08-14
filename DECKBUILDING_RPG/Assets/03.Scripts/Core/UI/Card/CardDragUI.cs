using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public class CardDragUI : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 originalPos;
    private RectTransform rect;
    private ACardBaseUI cardUI;
    public void Init(ACardBaseUI ui)
    {
        cardUI = ui;

    }
    public void Awake()
    {
        rect = GetComponent<RectTransform>();

        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {

        originalPos = rect.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {

        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
}
