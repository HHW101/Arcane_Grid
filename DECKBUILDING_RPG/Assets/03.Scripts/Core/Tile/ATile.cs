using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class ATile : MonoBehaviour, ITurnable, IShowInfoable
{
    [SerializeField]
    public TileData tileData = new TileData();
    [SerializeField]
    public SpriteRenderer objectTypeMarkerSpriteRenderer;
    [SerializeField]
    public SpriteRenderer isReachableMarkerSpriteRenderer;
    [SerializeField]
    protected StageManager stageManager;
    [SerializeField]
    private PathMarker pathMarker;
    private RuleManager ruleManager;
    private EnumAssociatedResourceManager enumAssociateResourceManager;
    public bool isDontNeedSave = false;
    public bool isCreatedInStageLoader = false;
    private float tileRadius;

    protected Unit unit;
    public Unit Unit {  get { return unit; } }  

    protected virtual void Awake()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
        ruleManager = GameManager.Instance.ruleManager;
        enumAssociateResourceManager = GameManager.Instance.enumAssociatedResourceManager;
        tileRadius = stageManager.GetTileRadius();
        if (isDontNeedSave)
        {
            return;
        }
    }
    protected virtual void Start()
    {
        if (isDontNeedSave)
        {
            return;
        }
        stageManager = GameManager.Instance.campaignManager.stageManager;
     
        if (tileData == null)
        {
            tileData = new TileData();
        }
        // 동기화 위해서 gameContext에 등록하기
        stageManager.RegisterTile(this, tileData);
    }
    #region highlight

    public void ShowPathMarker(ATile prevTile, ATile nextTile, bool isReachable)
    {
        isReachableMarkerSpriteRenderer.sprite = null;
        pathMarker.Show(prevTile.tileData.hexCoord, tileData.hexCoord, nextTile.tileData.hexCoord, isReachable);
    }

    public void HidePathMarker()
    {
        pathMarker.Hide();
    }

    public void SetIsReachableMarkerSprite(Sprite sprite)
    {
        isReachableMarkerSpriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        isReachableMarkerSpriteRenderer.sprite = sprite;
    }

    public void SetIsReachableDestMarkerSprite(Sprite sprite, ATile prevTile)
    {
        HexCoord prev = prevTile.tileData.hexCoord;
        HexCoord cur = tileData.hexCoord;
        HexCoord dir = HexCoord.GetDirection(prev, cur);
        float angle = HexCoord.GetAngleFromDirection(dir);
        isReachableMarkerSpriteRenderer.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, (540f - angle) % 360f);
        isReachableMarkerSpriteRenderer.sprite = sprite;
    }

    public void HighlightOn()
    {
    }

    public void HighlightTargetPossibleOn()
    {

    }

    public void HighlightTargetImpossibleOn()
    {

    }

    public void HighlightOff()
    {

    }

    #endregion

    public abstract bool isTarget(ACard card, Unit unit);
    #region UnitControl

    #endregion
    #region CardEffect
    public virtual  bool IsTargetable(ACard card)
    {
        if (unit == null)
        return false;
        return true;
    }
    public virtual void CanAttack()
    {

    }
    public virtual void TakeDamage(int dmg, Action onComplete)
    {
   

        
        unit.TakeDamage(dmg, onComplete);
        onComplete?.Invoke();
    }
    #endregion

    public float GetObjectOnTileWorldPositionOffset()
    {
        return tileRadius * tileData.objectOnTileWorldPositionOffsetMultiTileRadius;
    }

    public virtual void Synchronize()
    {
    }

    protected virtual void OnDestroy()
    {
        Synchronize();
    }

    [ContextMenu("CameraFollow")]
    public void CameraFollow()
    {
        GameManager.Instance.cameraManager.SetCameraFollow(gameObject.transform);
    }

    public void HighlightMovePossibleOn()
    {
    }

    public void HighlightMoveImpossibleOn()
    {
    }

    public virtual void ReactBeforeUnitExitThisTile(Unit unit, Action onComplete = null)
    {
        Logger.Log($"[{gameObject.name}] Tile : {unit.gameObject.name} exited this tile");
        this.unit = unit;
        UnitEnum.UnitCampType unitCampType = UnitEnum.UnitCampType.None;
        objectTypeMarkerSpriteRenderer.sprite = enumAssociateResourceManager.GetObjectTypeMarkerSprite(unitCampType);
        onComplete?.Invoke();
    }

    public virtual void ReactAfterUnitEnterThisTile(Unit unit, Action onComplete=null)
    {
        Logger.Log($"[{gameObject.name}] Tile : {unit.gameObject.name} entered this tile");
       this.unit = unit;
        UnitEnum.UnitCampType unitCampType = ruleManager.GetUnitCampType(unit.unitData);
        objectTypeMarkerSpriteRenderer.sprite = enumAssociateResourceManager.GetObjectTypeMarkerSprite(unitCampType);
        onComplete?.Invoke();
    }
    public virtual void ReactAfterUnitOutThisTile(Unit unit, Action onComplete=null)
    {
        Logger.Log($"[{gameObject.name}] Tile : {unit.gameObject.name} entered this tile");
        this.unit = null;
        onComplete?.Invoke();
    }
    [ContextMenu("RemoveFromSave")]
    public void RemoveFromSave()
    {
        if (stageManager == null)
        {
            return;
        }
        stageManager.RemoveTilePermanently(this);
    }

    public virtual void StartTurn()
    {
        if (tileData.turnStarted == false)
        {
            tileData.turnStarted = true;
        }
        Logger.Log($"[ATile] Tile {gameObject.name} Turn Started");
        EndTurn();
    }

    public virtual void EndTurn()
    {
        tileData.turnEnded = true;
        Logger.Log($"[ATile] Tile {gameObject.name} Turn Ended");
    }

    public virtual void RefillTurn()
    {
        Logger.Log($"[ATile] Tile {gameObject.name} Turn Refilled");
        tileData.turnEnded = false;
        tileData.turnStarted = false;
    }

    public bool IsTurnEnded()
    {
        return tileData.turnEnded;
    }
    
    public virtual void OnUnitPushedHere(Unit unit)
    {
    }
}
