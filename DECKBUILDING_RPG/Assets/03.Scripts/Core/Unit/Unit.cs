using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnitEnum;
using UnityEngine;
using UnityEngine.UI;

public abstract class Unit : MonoBehaviour
{
    [SerializeField]
    protected StageManager stageManager;
    protected UIManager uiManager;
    public bool isDontNeedSave = false;
    public bool isCreatedInStageLoader = false;

    [SerializeField] private SpriteRenderer SelectableTargetMark;
    [SerializeField] public BaseCardController cardController;
    public abstract List<StatusEffectInstance> StatusEffectList { get; }
    public abstract string PortraitPath { get; }
    public bool isStunned = false;

    [SerializeField] protected SpriteRenderer sprite;

    public abstract HexCoord CurrentHexCoord { get; protected set; }
    public abstract bool IsSpriteFlipped { get; }
    public abstract void SetSpriteFlip(bool isFlipped);
    
    public event Action OnHealthChanged;
    public event Action<Unit> OnDied;  
    private bool hasDied = false;
    
    protected int armor;
    protected AGun gun;
    public int Armor { get { return armor; } }
   

    public UnitData unitData
    {
        get
        {
            if (this is APlayer player)
            {
                return player.playerData;
            }
            else if (this is ANPC npc)
            {
                return npc.npcData;
            }
            else
            {
                return null;
            }
        }
    }
    
    public virtual float CurrentHealth
    {
        get => unitData?.currentHealth ?? 0f;
        protected set
        {
            if (unitData == null) return;
            float v = Mathf.Clamp(value, 0f, MaxHealth);
            if (Mathf.Approximately(unitData.currentHealth, v))
            {
                return;
            }

            unitData.currentHealth = v;
            OnHealthChanged?.Invoke();
        }
    }
    
    public virtual float MaxHealth
    {
        get => Mathf.Max(1f, unitData?.maxHealth ?? 1f);
        protected set
        {
            if (unitData == null) return;
            float clampedValue = Mathf.Max(1f, value);
            if (Mathf.Approximately(unitData.maxHealth, clampedValue))
            {
                return;
            }

            unitData.maxHealth = clampedValue;
            if (CurrentHealth > clampedValue) CurrentHealth = clampedValue;
            else OnHealthChanged?.Invoke();
        }
    }
    protected virtual void Awake()
    {
        stageManager = GameManager.Instance.campaignManager.stageManager;
    }

    protected virtual void Start()
    {
        if (isDontNeedSave)
        {
            return;
        }
        stageManager = GameManager.Instance.campaignManager.stageManager;
        uiManager = GameManager.Instance.uiManager;
        SelectableTargetMark?.gameObject?.SetActive(false);
        StartCoroutine(StartCoroutineOneFrameAfterStart());
       
    }
    #region highlight
    public void HighlightOn()
    {

        SelectableTargetMark?.gameObject?.SetActive(true);
    }

    public void HighlightTargetPossibleOn()
    {

        SelectableTargetMark?.gameObject?.SetActive(true);
    }

    public void HighlightTargetImpossibleOn()
    {

    }

    public void HighlightOff()
    {
        SelectableTargetMark?.gameObject?.SetActive(false);
    }

    #endregion

    // public abstract bool isTarget(ACard card, Unit unit);

    protected virtual IEnumerator StartCoroutineOneFrameAfterStart()
    {
        yield return null;
    }

    public abstract List<UnitEnum.UnitType> GetUnitTypeList();

    public abstract bool IsHaveThisType(UnitEnum.UnitType unitType);

    #region CardEffect
    public virtual void Stunned()
    {
        isStunned = true;
 
    }
    public virtual void TakeDamage(int dmg, Action onComplete = null)
    {
        int actual = Mathf.Max(0, dmg - armor);
        if (this is ANPC)
        {
            stageManager.AddDealDamage(Mathf.Min((int)CurrentHealth, actual));
        }
        if (actual > 0) CurrentHealth -= actual;
        
        GameObject obj = GameManager.Instance.poolManager.GetObject("DamageText");
        if (!obj)
        {
            onComplete?.Invoke();
            return;
        }

        var popupCanvas = GameManager.Instance.poolManager.GetPopupCanvas();
        if (!popupCanvas)
        {
            onComplete?.Invoke();
            return;
        }

        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();
        RectTransform rect = obj.GetComponent<RectTransform>();

        if (!canvasRect || !rect)
        {
            onComplete?.Invoke();
            return;
        }

        Vector3 worldPos = transform.position + Vector3.up * 1.5f;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out var localPoint
        );
        rect.anchoredPosition = localPoint;

        obj.SetActive(true);

        DamageText dmgText = obj.GetComponent<DamageText>();
        if (dmgText)
            dmgText.Set(dmg, this.transform, canvasRect, new Vector3(0, 1.5f, 0), GameManager.Instance.poolManager.transform);

        onComplete?.Invoke();
    }
    
    public virtual Coroutine TakePush(int pushPower, HexCoord direction, Action onComplete = null)
    {
        Logger.Log("Unit TakePush");
        return StartCoroutine(TakePushCoroutine(pushPower, direction, onComplete));
    }

    protected virtual IEnumerator TakePushCoroutine(int pushPower, HexCoord direction, Action onComplete = null)
    {
        Logger.Log("Unit TakePush Coroutine");
        HexCoord start = CurrentHexCoord;
        HexCoord dest = start;

        for (int i = 0; i < pushPower; i++)
        {
            HexCoord next = dest + direction;

            if (!stageManager.IsTileExist(next) || stageManager.IsUnitOnTile(next))
                break;

            ATile fromTile = stageManager.FindTileByCoord(dest);
            fromTile?.ReactBeforeUnitExitThisTile(this);  

            yield return PushEffectRoutine(dest, next);

            dest = next;
            CurrentHexCoord = dest;

            ATile toTile = stageManager.FindTileByCoord(dest);
            toTile?.ReactAfterUnitEnterThisTile(this); 

            toTile?.OnUnitPushedHere(this);
        }

        onComplete?.Invoke();
    }

    protected virtual IEnumerator PushEffectRoutine(HexCoord from, HexCoord to)
    {
        Vector3 fromPos = stageManager.ConvertHexToWorld(from);
        Vector3 toPos = stageManager.ConvertHexToWorld(to);
        transform.position = toPos;
        yield return null;
    }
    
    public virtual void Heal(int value)
    {
    
    }
    public virtual void ResetArmor()
    {
        armor = 0;
    }
    public virtual void HalfArmor()
    {
        armor /= 2;
    }
    public virtual void AddArmor(int value)
    {
        armor += value;

    }
    #endregion
    public virtual void Synchronize()
    {
    }

    protected virtual void OnDestroy()
    {
        Synchronize();
    }
 
    #region Movement
    public virtual void move(int x, int y)
    {
        Debug.Log("이동");
    }
    #endregion
    #region Card
    //이런 식으로 사용
    public virtual void LoadGun(ACard card)
    {
        gun.Load(card);
    }
    public virtual void UseCard(ACard card, ATile target)
    {
        card.Use(target,this);
    }
    #endregion
    #region Battle

    protected void RaiseDiedOnce()
    {
        if (hasDied) return;
        hasDied = true;
        try { OnDied?.Invoke(this); } catch {}
    }
    
    public virtual bool IsDead() { return hasDied; }
    
    public virtual void Attack() 
    {
        Debug.Log("공격");
    }
    
    public virtual void CopyStatusEffectsFromData(List<StatusEffectInstance> dataList)
    {
        StatusEffectList.Clear();

        if (dataList == null)
        {
            return;
        }

        foreach (var inst in dataList)
            StatusEffectList.Add(new StatusEffectInstance(inst.effectType, inst.remainingTurn));
    }
    #endregion
    #region CameraFollow

    [ContextMenu("CameraFollow")]
    public void CameraFollow()
    {
        GameManager.Instance.cameraManager.SetCameraFollow(gameObject.transform);
    }
    #endregion
    public virtual void TakeInteract(Action onComplete)
    {
        Debug.Log($"[{gameObject.name}] 상호작용 발생");
        onComplete?.Invoke();
    }

    //범위 계산 버추얼 메서드
}
