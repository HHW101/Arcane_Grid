using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class APlayer : Unit, ITurnable, IShowInfoable
{
    private static readonly int Move1 = Animator.StringToHash("Move");
    private static readonly int MeleeAttackTr = Animator.StringToHash("MeleeAttack");
    private static readonly int AttackTr      = Animator.StringToHash("Attack");
    [SerializeField] private string meleeStateName = "PlayerMeleeAttack";
    [SerializeField] private string shootStateName = "Attack Shoot";
    [SerializeField, Range(0f,1f)] private float meleeHitPoint = 0.35f; 
    [SerializeField, Range(0f,1f)] private float shootFirePoint = 0.20f; 
    [SerializeField, Range(0f,1f)] private float exitPoint = 0.95f;      
    [SerializeField] private string idleStateName = "Idle";   
    [SerializeField] private float moveSpeed = 30f;

    private AnimatorStateInfo CurInfo() => Anim.GetCurrentAnimatorStateInfo(0);
    
    [SerializeField] private float attackSpeed = 3f;

    [SerializeField] private float interactSpeed = 3f;
    [SerializeField] private float moveArcHeight = 2.4f;
    [SerializeField] private float attackArcHeight = 1.6f;
    [SerializeField] private float interactArcHeight = 1.6f;
    [SerializeField] public PlayerActionUI playerActionUI;
    [SerializeField] public PlayerActionCancelUI playerActionCancelUI;
    [SerializeField] public PlayerBulletUI playerBulletUI;
    [SerializeField] private Animator animator;

    [Header("Auto Load After Select Job")]
    public PlayerData playerData = new PlayerData();
    public PlayerStateInStage playerStateInStage = new PlayerStateInStage();
    public HexCoord targetCoord;
    public StageManager StageManager => stageManager;
    private CampaignManager campaignManager;

    private StageTime stageTime;
    public int dmg = 10;

    private Animator Anim
    {
        get
        {
            if (animator) return animator;
            animator = GetComponentInChildren<Animator>(true);
            if (!animator) Logger.LogWarning($"[APlayer] Animator not found on {name}");
            return animator;
        }
    }

    public AttackInfo GetNextAttackInfo()
    {
        AttackInfo attackInfo;
        if (!gun.IsEmpty)
        {
            attackInfo = new AttackInfo(false,
                gun.GetFrontBullet().Card.Name,
                playerData.gunRange,
                playerData.defaultAttack,
                gun.GetFrontBullet().Card.Value,
                playerData.radius,
                playerData.pushForce + 1);
        }
        else
        {
            attackInfo = new AttackInfo(true,
                null,
                playerData.meleeAttackRange,
                playerData.defaultAttack,
                0,
                playerData.radius,
                playerData.pushForce);
        }
        return attackInfo;
    }

    public void RecoverWhenEscapeStage()
    {
        RefillActionPoint();
        RefillTechnicalPoint();
    }

    private IEnumerator WaitEnterState(string stateName)
    {
        while (!CurInfo().IsName(stateName)) yield return null;
    }

    private IEnumerator WaitNormalized(string stateName, float t)
    {
        yield return WaitEnterState(stateName);
        while (CurInfo().IsName(stateName) && CurInfo().normalizedTime < t)
            yield return null;
    }
    
    private bool wasProcessing = false;
    private bool moveInterrupted = false;
    public override string PortraitPath => playerData?.portraitPath;
    public override bool IsSpriteFlipped => sprite.flipX;
   
    public override void SetSpriteFlip(bool isFlipped)
    {
        sprite.flipX = isFlipped;
        playerStateInStage.isFlipped = isFlipped;
    }
    public override HexCoord CurrentHexCoord
    {
        get => playerStateInStage.hexCoord;
        protected set => playerStateInStage.hexCoord = value;
    }
    public override List<StatusEffectInstance> StatusEffectList
    {
        get
        {
            if (playerStateInStage == null)
                throw new Exception("npcData is null!");
            return playerStateInStage.statusEffectList ?? (playerStateInStage.statusEffectList = new List<StatusEffectInstance>());
        }
    }
    protected override void Start()
    {
        base.Start();
        stageTime = StageTime.Instance;
        stageManager.RegisterPlayer(this);
        playerActionUI.gameObject.SetActive(false);
       playerActionUI.Init(OpenActionCancelUI);
        playerActionCancelUI.gameObject.SetActive(false);
        playerBulletUI?.gameObject.SetActive(false);
        playerActionCancelUI.Init(OpenActionUI);

        if(cardController == null)
           cardController= this.gameObject.AddComponent<PlayerCardController>();
        cardController.Init(this.gameObject);
        PlayerCardController pc = cardController as PlayerCardController;
        campaignManager = GameManager.Instance.campaignManager;
        ATile tile = StageManager.FindTileByCoord(playerStateInStage.hexCoord);
        tile.ReactAfterUnitEnterThisTile(this);
        if (playerData.currentHealth == 0)
        {
            Dead();
        }
        gun = cardController.AddGun();
        playerBulletUI.Init(gun);
        //gun.transform.SetParent(this.gameObject.transform, false);
        wasProcessing = stageManager.IsProcessingUnitAct();
        Invoke("tempFindTile", 0.1f);
        SetSpriteFlip(playerStateInStage.isFlipped);
    }
   
    protected override IEnumerator StartCoroutineOneFrameAfterStart()
    {
        yield return null;
        Vector3 vector3 = gameObject.transform.position;
        vector3.y += (stageManager.FindTileByCoord(playerStateInStage.hexCoord)?.GetObjectOnTileWorldPositionOffset()) ?? 0f;
        gameObject.transform.position = vector3;
    }
    
    public void UpdateUIActive()
    {
        bool isProcessing = stageManager.IsProcessingUnitAct();
        if (!wasProcessing && isProcessing)
        {
            playerActionCancelUI.gameObject.SetActive(false);
            playerBulletUI.gameObject.SetActive(false);
        }
        if (wasProcessing && !isProcessing)
        {
            playerActionCancelUI.gameObject.SetActive(true);
            playerBulletUI.gameObject.SetActive(true);
        }

        wasProcessing = isProcessing;
    }

    public override void Heal(int value)
    {
        GameManager.Instance.particleManager.PlayAnimation("HealEffect", gameObject.transform.position,gameObject.transform.rotation);
        playerData.currentHealth += value;
        playerData.currentHealth = Mathf.Min(playerData.currentHealth, playerData.maxHealth);
        Logger.Log($"[Player] {value} 치료.");
    }
    public override void TakeDamage(int dmg, Action onComplete)
    {
        GameManager.Instance.audioManager.PlaySfx("Weapon_Hitting_Flesh-004");
        base.TakeDamage(dmg, onComplete);
        if (this.transform.GetChild(0).GetComponent<ParticleSystem>() != null)
        {
            ParticleSystem childparticle = this.transform.GetChild(0).GetComponent<ParticleSystem>();
            childparticle.Play();

        }


        if (playerData.currentHealth == 0)
        {
            Dead();
        }
        //campaignManager.AddDealDamage(dmg);

        //onComplete?.Invoke();
    }

    protected override IEnumerator PushEffectRoutine(HexCoord from, HexCoord to)
    {
        Vector3 startPos = stageManager.ConvertHexToWorld(from);
        Vector3 endPos = stageManager.ConvertHexToWorld(to);

        float duration = 0.18f; 
        float elapsed = 0f;
        float arcHeight = 1.0f; 

        while (elapsed < duration)
        {
            elapsed += stageTime.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 flatPos = Vector3.Lerp(startPos, endPos, t);
            float height = -4f * arcHeight * (t - 0.5f) * (t - 0.5f) + arcHeight;
            transform.position = new Vector3(flatPos.x, flatPos.y + height, flatPos.z);
            yield return null;
        }

        transform.position = endPos;
    }

    public override bool IsDead() => base.IsDead() || playerData.isDead;

    [ContextMenu("Dead")]
    public void Dead()
    {
        if (playerData.isDead) return;

        EndTurn();
        Logger.Log($"[Player]는 죽었다!");
        playerData.isDead = true;

        RaiseDiedOnce();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        stageManager.UnRegisterPlayer(this);
    }

    public virtual void StartTurn()
    {
        Logger.Log($"[APlayer] Player {gameObject.name} Turn Started");

        if (!playerStateInStage.turnStarted)
        {
            playerStateInStage.turnStarted = true;
        }

        playerActionUI.gameObject.SetActive(true);
        playerActionCancelUI.gameObject.SetActive(false);
        playerBulletUI.gameObject.SetActive(true);
        playerBulletUI.ShowBullet();
        CameraFollow();
    }

    public virtual void EndTurn()
    {
        playerActionUI.gameObject.SetActive(false);
        playerBulletUI.gameObject.SetActive(false);
        playerStateInStage.turnEnded = true;
        uiManager.HideGetDetailInfo();
        Logger.Log($"[APlayer] Player {gameObject.name} Turn Ended");
    }

    public virtual void RefillTurn()
    {
        Logger.Log($"[APlayer] Player {gameObject.name} Turn Refilled");
        playerStateInStage.turnEnded = false;
        playerStateInStage.turnStarted = false;
        playerStateInStage.statusEffectProcessedThisTurn = false;
    }

    public void RefillActionPoint()
    {
        playerData.actionPoint = playerData.maxActionPoint;
    }

    public void RefillTechnicalPoint()
    {
        playerData.technicalPoint = playerData.maxTechnicalPoint;
    }

    public bool IsTurnEnded()
    {
        return playerStateInStage.turnEnded;
    }

    public void OpenActionUI()
    {
        playerActionCancelUI.gameObject.SetActive(false);
        playerBulletUI.gameObject.SetActive(true);
        playerActionUI.gameObject.SetActive(true);
        playerBulletUI.ShowBullet();
        GameManager.Instance.cardManager.onClick?.Invoke();
    }

    public void OpenActionCancelUI()
    {
        playerActionCancelUI.gameObject.SetActive(true);
        playerActionUI.gameObject.SetActive(false);
        playerBulletUI.gameObject.SetActive(false);
        GameManager.Instance.cardManager.onClick?.Invoke();
    }

    public void Move(HexCoord hexCoord, Action onComplete)
    {
        moveInterrupted = false;
        StartCoroutine(MoveRoutine(hexCoord, onComplete));
    }

    private IEnumerator MoveRoutine(HexCoord hexCoord, Action onComplete)
    {
        if (animator)
            animator.SetBool(Move1, true);

        var childParticleObj = transform.childCount > 1 ? transform.GetChild(1) : null;
        if (childParticleObj != null)
        {
            var childParticle = childParticleObj.GetComponent<ParticleSystem>();
            if (childParticle != null)
                childParticle.Play();
        }

        stageManager.OnPlayerExitCoord(playerStateInStage.hexCoord, this, null);

        float tileSize = stageManager.GetTileRadius();
        Vector3 startPos = transform.position;
        Vector3 endPos = HexCoord.HexToWorld(hexCoord, tileSize);
        ATile tile = stageManager.FindTileByCoord(hexCoord);
        endPos.y += tile?.GetObjectOnTileWorldPositionOffset() ?? 0;

        if (sprite)
        {
            Vector3 diff = endPos - startPos;
            if (Mathf.Abs(diff.x) > 0.01f)
                SetSpriteFlip(diff.x < 0);
        }

        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / moveSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += stageTime.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.position = Vector3.Lerp(startPos, endPos, t);

            yield return null;
        }

        transform.position = endPos;
        playerStateInStage.hexCoord = hexCoord;

        if (animator)
            animator.SetBool(Move1, false);

        stageManager.OnPlayerEnterCoord(hexCoord, this, onComplete);
        stageManager.RequestNPCListUpdate();
    }


    public void SetMoveInterrupt(bool moveInterrupt)
    {
        moveInterrupted = moveInterrupt;
    }

    public bool IsMoveInterrupted()
    {
        return moveInterrupted;
    }


    public void Attack(Unit target, Action onComplete = null)
    {
        StartCoroutine(AttackRoutine(target, onComplete));
    }

    private IEnumerator AttackRoutine(Unit unit, Action onComplete)
    {
        Logger.Log($"[Player] {unit.name}에게 공격 시도");

        AttackInfo nextAttackInfo = GetNextAttackInfo();

        int damage = nextAttackInfo.damage + nextAttackInfo.offsetBulletDamage;
        int radius = nextAttackInfo.radius;
        int pushForce = nextAttackInfo.pushForce;
        bool isMelee = nextAttackInfo.isMelee;

        string stateName = isMelee ? meleeStateName : shootStateName;
        float hitPoint = isMelee ? meleeHitPoint : shootFirePoint;
        

        var anim = Anim;
        FaceTarget(unit);
        if (anim)
            anim.CrossFadeInFixedTime(stateName, 0.05f, 0, 0f);

        if (anim)
            while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
                yield return null;

        if (anim)
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < hitPoint)
                yield return null;

        if (!gun.IsEmpty)
        {
            gun.Bullets.Dequeue();
        }
        HexCoord center = unit.CurrentHexCoord;
        List<ANPC> npcList = stageManager.GetNPCsInRange(center, radius);
        for (int i = 0; i < npcList.Count; i++)
        {
            if (isMelee)
            {
                npcList[i].TakeDamage(damage, null);
                GameManager.Instance.audioManager.PlaySfx("Weapon_Whoosh 02");
            }
            else
            {
                if (i == 0)
                {
                    GameObject projectileObj = GameManager.Instance.poolManager.GetObject("PlayerProjectile");
                    projectileObj.transform.position = transform.position;
                    projectileObj.SetActive(true);

                    yield return StartCoroutine(MoveProjectileRoutine(projectileObj, unit.transform.position));
                    GameManager.Instance.poolManager.ReturnObject("PlayerProjectile", projectileObj);

                    GameManager.Instance.audioManager.PlaySfx("Pistol-005");
                }

                npcList[i].TakeDamage(damage, null);
                if (i == 0)
                {
                    GameManager.Instance.particleManager.Play("ExplotionEffect", unit.transform.position, Quaternion.identity, 3f);
                }
            }

            if (npcList[i] != null && npcList[i].npcData.currentHealth > 0)
            {
                if (pushForce >= 1)
                {
                    HexCoord attacker = playerStateInStage.hexCoord;
                    HexCoord target = npcList[i].CurrentHexCoord;
                    HexCoord pushDir = HexCoord.GetDirection(attacker, target);
                    yield return npcList[i].TakePush(pushForce, pushDir);
                }
            }
        }

        if (anim)
            while (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                   anim.GetCurrentAnimatorStateInfo(0).normalizedTime < exitPoint)
                yield return null;

        if (anim && !anim.GetCurrentAnimatorStateInfo(0).IsName(idleStateName))
            anim.CrossFadeInFixedTime(idleStateName, 0.05f, 0, 0f);

        onComplete?.Invoke();
        stageManager.RequestNPCListUpdate();
    }

private void FaceTarget(Unit unit)
{
    const float faceFlipMinDeltaX = 0.01f;
    if (!unit) return;
    float dx = unit.transform.position.x - transform.position.x;
    if (Mathf.Abs(dx) > faceFlipMinDeltaX)
        SetSpriteFlip(dx < 0f); 
}

    private void PlayAttackTriggerByPlayerRange()
    {
        var anim = Anim;
        if (!anim) return;

        string stateName = (playerData.meleeAttackRange <= 1) ? meleeStateName : shootStateName;
        anim.CrossFadeInFixedTime(stateName, 0.05f, 0, 0f);
    }
    
    public void Interact(Unit unit, Action onComplete)
    {
        StartCoroutine(InteractRoutine(unit, onComplete));
    }

    private IEnumerator InteractRoutine(Unit unit, Action onComplete)
    {
        Logger.Log($"[Player] {unit.name}에게 상호작용 시도");

        float baseDuration = 0.3f;
        float duration = baseDuration / interactSpeed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += stageTime.deltaTime;
            yield return null;
        }

        unit.TakeInteract(onComplete);
        stageManager.RequestNPCListUpdate();
    }

    public bool CanUseActionPoint(int use)
    {
    
        return playerData.actionPoint >= use;
    }
    public bool CanUseGun()
    {
        return gun.CanUse();
    }

    public void UseActionPoint(int use)
    {
        playerData.actionPoint -= use;
    }

    public bool CanUseTechnicalPoint(int use)
    {
        return playerData.technicalPoint >= use;
    }

    public void UseTechnicalPoint(int use)
    {
        playerData.technicalPoint -= use;
    }
    public void GainExp(float amount)
    {
        playerData.experience += amount;
        Logger.Log($"획득 경험치:{amount}  | 현재 경험치: {playerData.experience}");

        if (playerData.experience >= playerData.maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        playerData.level++;
        playerData.experience = 0; // 또는 초과분 이월

        Logger.Log($"레벨업 현재 레벨: {playerData.level}");
    }

    public override List<UnitEnum.UnitType> GetUnitTypeList()
    {
        return playerData.unitTypeList;
    }

    public override bool IsHaveThisType(UnitEnum.UnitType unitType)
    {
        return playerData.IsHaveThisType(unitType);
    }
    
    private IEnumerator MoveProjectileRoutine(GameObject projectileObj, Vector3 targetPos)
    {
        const float speed = 30f;
        Vector3 dir = (targetPos - projectileObj.transform.position).normalized;
        projectileObj.transform.up = dir;
        while (Vector3.Distance(projectileObj.transform.position, targetPos) > 0.05f)
        {
            projectileObj.transform.position = Vector3.MoveTowards(
                projectileObj.transform.position, targetPos, speed * stageTime.deltaTime
            );
            yield return null;
        }
    }
}