using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class HoverHighlighterUseCard : MonoBehaviour
{
    private Camera mainCamera;

    private GameObject currentHovered = null;
    private StageManager stageManager;
    private GameContext gameContext;
    private PlayerStateInStage playerStateInStage;
    private PlayerData playerData;
    private CardManager cardManager;
    private List<ANPC> highlightedNPCs = new();
    protected List<ATile> highlightedTiles = new List<ATile>();
    protected ATile nearestTile;
    private bool isSelected= false;
    [SerializeField] private Sprite UseAbleSprite;
    [SerializeField] private Sprite willUseSprite;
    [SerializeField] private Sprite noneSprite;
    private void Awake()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        gameContext = GameManager.Instance.gameContext;
        cardManager = GameManager.Instance.cardManager;
        var campaignData = gameContext.saveData.campaignData;
        var playerStateInCampaign = campaignData.playerStateInCampaign;
        var stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];

        playerStateInStage = stageData.playerStateInStage;
        playerData = gameContext.saveData.playerData;
        cardManager.OnSelected.AddListener(CardSelected);
        cardManager.UnSelected.AddListener(CardUnSelected);
        mainCamera = Camera.main;
    }
    
    private void OnEnable()
    {
        //RefreshAttackableTargets();
    }

    private void OnDisable()
    {
        ClearCurrentHighlight();
        ClearCurrentHighlight();
       
    }
    private void OnDestroy()
    {
        cardManager.OnSelected.RemoveListener(CardSelected);
        cardManager.UnSelected.RemoveListener(CardUnSelected);
    }
    public void SetState(HoverHighlightEnum.HoverHighlight hoverHighlight)
    {
        gameObject.SetActive(hoverHighlight == HoverHighlightEnum.HoverHighlight.UseCard);
    }

    void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            if (HpUI.Instance != null && !HpUI.IsPointerOverSelf())
            {
                ClearCurrentHighlight();
                return;
            }
        }
        

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
        if (cardManager.SelectedCard != null)
        {
            ATile tile = FindNearestTarget(mousePos2D);
        
            if (tile == null)
            {
               
                cardManager.ResetSelectTarget();
                if (nearestTile != null)
                {
                    nearestTile.SetIsReachableMarkerSprite(UseAbleSprite);
                    nearestTile = null;
                }
            }
            else
            {

           
                //cardManager.ResetSelectTarget();
                cardManager.SetSelectTarget(tile);
                if(nearestTile != null&&tile!= nearestTile)
                nearestTile.SetIsReachableMarkerSprite(UseAbleSprite);
                tile.SetIsReachableMarkerSprite(willUseSprite);

                nearestTile = tile;
            }
        }
    
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        
        GameObject highestObj = null;
        
        int highestOrder = int.MinValue;

        foreach (var hit in hits)
        {
            if (hit.collider == null)
            {
                continue;
            }
            SpriteRenderer renderer = hit.collider.GetComponent<SpriteRenderer>();

            if (renderer != null && renderer.sortingOrder >= highestOrder)
            {
                highestOrder = renderer.sortingOrder;
                highestObj = hit.collider.gameObject;
            }
        }

        if (highestObj != currentHovered)
        {
            ClearCurrentHighlight();
            currentHovered = highestObj;
            ApplyHighlight(currentHovered);
        }
    }
    public void CardSelected()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks");
        isSelected = true;
        TurnOnTargetEffect(gameContext.player);
    }
    public void CardUnSelected()
    {
        isSelected = false;
        TurnOffTargetEffect(gameContext.player);
    }
    void ClearCurrentHighlight()
    {
        if (currentHovered == null) return;
        if (currentHovered.TryGetComponent<ATile>(out var tile))
        {
            tile.SetIsReachableMarkerSprite(noneSprite);
        }
        if (currentHovered.TryGetComponent<Unit>(out var unit))
        {
            unit.HighlightOff();
        }

        currentHovered = null;
    }

    void ApplyHighlight(GameObject obj)
    {
        if (obj == null) return;

        if (currentHovered.TryGetComponent<ATile>(out var tile))
        {
            tile.HighlightOn();
        }
        if (currentHovered.TryGetComponent<Unit>(out var unit))
        {
            unit.HighlightOn();
        }
    }
    public virtual void TurnOnTargetEffect(Unit unit)
    {
       // Debug.Log("타일 카드 사용 탐색 시작");
        FindTargets(unit);
        if (highlightedTiles.Count == 0)
            return;
        foreach (ATile tile in highlightedTiles)
        {
            tile.SetIsReachableMarkerSprite(UseAbleSprite);
        }
    }
    public virtual void TurnOffTargetEffect(Unit unit)
    {
       // Debug.Log("타일 카드 사용 탐색 종료");
        cardManager.ResetSelectTarget();
        if (highlightedTiles.Count == 0)
            return;
        foreach (ATile tile in highlightedTiles)
        {
            tile.SetIsReachableMarkerSprite(noneSprite);
            //특정 효과
        }
        highlightedTiles.Clear();
    }
    public virtual ATile FindNearestTarget(Vector2 mousePos)
    {
        float MinDistance = 100f;
        ATile NearestTile = null;
        foreach (ATile tile in highlightedTiles)
        {
            float distance = Vector2.Distance(tile.transform.position, mousePos);
           
            if (distance < 5f)
            {
        
                if (distance < MinDistance)
                {
 
                    NearestTile = tile;
                    MinDistance = distance;
                }
            }
        }

        return NearestTile;
    }
    public virtual void FindTargets(Unit User)
    {
       highlightedTiles.Clear();
        List<ATile> RangeTile = GameManager.Instance.campaignManager.stageManager.GetCardUsableTileList(cardManager.SelectedCard);
     
        if (cardManager.SelectedCard.Template.isGun)
            return;
        foreach (ATile rangeTile in RangeTile)
        {
            if (rangeTile.Unit != null)
            {
                
               if (cardManager.SelectedCard.CanUse(rangeTile,User))
                    highlightedTiles.Add(rangeTile);
            }

        }
    }
}