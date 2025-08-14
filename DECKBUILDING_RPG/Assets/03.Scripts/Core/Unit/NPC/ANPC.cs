using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class ANPC : Unit, ITurnable, IShowInfoable
{
    private static readonly int DeadAnim = Animator.StringToHash("Die");
    private static readonly int Damage = Animator.StringToHash("Damage");
    [SerializeField] public NPCData npcData = new NPCData();
    [SerializeField] protected AInteractUI interactUI;

    public ACardZone zone;

    public Coroutine MoveCoroutine;
    public Coroutine AttackCoroutine;
    private WaitForSeconds ws = new WaitForSeconds(0.3f);
    public StageManager StageManager => stageManager;
    private CampaignManager campaignManager;
    private StageTime stageTime;
    
    public List<NPCAttackTypeSO> attackTypeList;
    Dictionary<NPCAttackTypeSO, float> attackCooldownDict = new();
    public float CurrentTurn => GameManager.Instance.campaignManager.stageManager.currentLoadedStageData?.curTurn ?? 0;
    public override string PortraitPath => npcData?.portraitPath;
    public override bool IsSpriteFlipped => sprite.flipX;
    public override void SetSpriteFlip(bool isFlipped)
    {
        sprite.flipX = isFlipped;
        npcData.isFlipped = isFlipped; 
    }

    protected readonly List<StatusEffectInstance> _statusEffects = new();
    public override List<StatusEffectInstance> StatusEffectList => _statusEffects;
    public override HexCoord CurrentHexCoord
    {
        get => npcData.hexCoord;
        protected set => npcData.hexCoord = value;
    }

    protected override void Start()
    {
        base.Start();
        if (npcData == null)
        {
            npcData = new NPCData();
        }

        // 동기화 위해서 gameContext에 등록하기
        stageManager.RegisterNPC(this, npcData);
        if (npcData.currentHealth == 0)
        {
            Dead(null);
        }

        interactUI?.gameObject?.SetActive(false);
        ATile tile = stageManager.FindTileByCoord(npcData.hexCoord);
        campaignManager = GameManager.Instance.campaignManager;
        Debug.Log(tile);
        tile.ReactAfterUnitEnterThisTile(this);
        SetSpriteFlip(npcData.isFlipped);
        stageTime = StageTime.Instance;
    }

    protected override IEnumerator StartCoroutineOneFrameAfterStart()
    {
        yield return null;
        Vector3 vector3 = gameObject.transform.position;
        vector3.y = HexCoord.HexToWorld(npcData.hexCoord, (int)stageManager.GetTileRadius()).y + stageManager.FindTileByCoord(npcData.hexCoord)?.GetObjectOnTileWorldPositionOffset() ?? 0f;
        gameObject.transform.position = vector3;
    }

    public override void TakeDamage(int dmg, Action onComplete)
    {
        base.TakeDamage(dmg, onComplete);    
        if (this.transform.GetChild(0).GetComponent<ParticleSystem>() != null)
        {
            ParticleSystem childparticle = this.transform.GetChild(0).GetComponent<ParticleSystem>();
            childparticle.Play();
            GameManager.Instance.audioManager.PlaySfx("Weapon_Hitting_Flesh-004");
        }
        // 이미 HP 계산은 base에서 처리했다고 가정 (0 이하 방지)
        if (npcData.currentHealth <= 0)
        {
            // Damage 트리거가 걸려 있었을 수 있으니 반드시 리셋
            var anim = (this as NPC)?.controller?.Animator;
            if (anim != null)
            {
                anim.ResetTrigger(Damage);
                anim.SetBool(DeadAnim, true);
                // 즉시 Die 재생(전이 대기 없이)
                anim.Play("bguaiDie", 0, 0f); // 상태명은 실제 컨트롤러에 맞게
            }
            Dead(onComplete);
            return;
        }
    // 살아있을 때만 데미지 리액션
    (this as NPC)?.controller?.Animator?.SetTrigger(Damage);
        Logger.Log($"[ANPC] [{gameObject.name}] {dmg}데미지를 입었다!");
    }

    public void TakeDamageAnimatorPlay()
    {
        (this as NPC)?.controller?.Animator?.SetTrigger(Damage);
    }

    protected override IEnumerator TakePushCoroutine(int pushPower, HexCoord direction, Action onComplete = null)
    {
        Logger.Log($"{gameObject.name} {pushPower} NPC TakePush Coroutine");
        HexCoord start = CurrentHexCoord;
        HexCoord dest = start;

        for (int i = 0; i < pushPower; i++)
        {
            HexCoord next = dest + direction;

            if (!stageManager.IsTileExist(next) || stageManager.IsUnitOnTile(next))
            {
                Logger.Log("NPC TakePush Coroutine break");
                break;
            }

            ATile fromTile = stageManager.FindTileByCoord(dest);
            fromTile?.ReactAfterUnitOutThisTile(this);

            fromTile.ReactBeforeUnitExitThisTile(this);

            yield return PushEffectRoutine(dest, next);

            dest = next;
            CurrentHexCoord = dest;

            ATile toTile = stageManager.FindTileByCoord(dest);
            if (!toTile)
            {
                Logger.LogError("도착 타일이 없습니다: " + dest);
            }
            else
            {
                toTile.ReactAfterUnitEnterThisTile(this); 
            }

            toTile?.OnUnitPushedHere(this);
        }

        onComplete?.Invoke();
        stageManager.RequestNPCListUpdate();
    }
    
    protected override IEnumerator PushEffectRoutine(HexCoord from, HexCoord to)
    {
        Logger.Log("NPC PushEffect Coroutine");
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
        stageManager.RequestNPCListUpdate();
    }
    
    public override void TakeInteract(Action onComplete)
    {
        Debug.Log($"[{gameObject.name}] 상호작용 발생");
        onComplete?.Invoke();
    }

    public NPCAttackTypeSO GetRandomAvailableAttack(Unit target)
    {
        if (attackTypeList == null || attackTypeList.Count == 0)
            return null;
    
        var validList = attackTypeList.Where(at => at).ToList();
        if (validList.Count == 0)
            return null;

        float dist = npcData.hexCoord.Distance(
            (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord);

        var candidates = validList.Where(at =>
        {
            if (dist > at.range) return false;
            if (npcData.currentActionPoint < at.apCost) return false;
            var lastUsed = attackCooldownDict.GetValueOrDefault(at, -999);
            return !(CurrentTurn < lastUsed + at.cooldown);
        }).ToList();

        if (candidates.Count == 0)
            return null;

        var idx = UnityEngine.Random.Range(0, candidates.Count);
        return candidates[idx];
    }

    public override void Attack()
    {
    }

    public void Attack(Unit target)
    {
        base.Attack();
        if (AttackCoroutine != null)
        {
            StopCoroutine(AttackCoroutine);
            AttackCoroutine = null;
        }

        AttackCoroutine = StartCoroutine(AttackRoutine(target));
    }

    private IEnumerator AttackRoutine(Unit target)
{
        try
        {
            if (!target || target.IsDead())
                yield break;

            if (attackTypeList is { Count: > 0 })
            {
                var attackType = GetRandomAvailableAttack(target);
                if (!attackType)
                    yield break;

                if (attackType.UseDefaultEffect)
                {
                    var hasVisualEffect = attackType.effects != null && (
                        attackType.effects.projectilePrefab ||
                        attackType.effects.effectPrefab
                    );

                    if (!hasVisualEffect)
                    {
                        const float shakeTime = 0.4f;
                        const float shakeAmount = 0.5f;
                        var elapsed = 0f;
                        var originalPos = transform.position;

                        while (elapsed < shakeTime)
                        {
                            elapsed += stageTime.deltaTime;
                            var t = elapsed / shakeTime;
                            var offsetX = Mathf.Sin(t * Mathf.PI * 4) * shakeAmount * (1 - t);
                            transform.position = originalPos + new Vector3(offsetX, 0, 0);
                            yield return null;
                        }
                        transform.position = originalPos;
                    }

                    yield return new WaitForSeconds(0.2f);
                }

                yield return attackType.ExecuteAttackCoroutine(this, target);

                npcData.currentActionPoint -= attackType.apCost;
                npcData.currentAttackCount--;
                attackCooldownDict[attackType] = CurrentTurn;
            }
            else
            {
                float dist = npcData.hexCoord.Distance(
                    (target is APlayer p) ? p.playerStateInStage.hexCoord : ((ANPC)target).npcData.hexCoord);

                if (dist > npcData.attackRange || npcData.currentActionPoint < npcData.actionPointPerAttack)
                    yield break;

                const float shakeTime = 0.4f;
                const float shakeAmount = 0.5f;
                var elapsed = 0f;
                var originalPos = transform.position;

                while (elapsed < shakeTime)
                {
                    elapsed += stageTime.deltaTime;
                    var t = elapsed / shakeTime;
                    var offsetX = Mathf.Sin(t * Mathf.PI * 4) * shakeAmount * (1 - t);
                    transform.position = originalPos + new Vector3(offsetX, 0, 0);
                    yield return null;
                }
                transform.position = originalPos;

                yield return new WaitForSeconds(0.2f);

                target.TakeDamage(npcData.attackDamage, null);
                npcData.currentActionPoint -= npcData.actionPointPerAttack;
                npcData.currentAttackCount--;
            }

            if (!target || target.IsDead())
                yield break;

            yield return new WaitForSeconds(npcData.attackDelay);
        }
        finally
        {
            AttackCoroutine = null;
        }
        stageManager.RequestNPCListUpdate();
    }

    [ContextMenu("Dead")]
    public void Dead(Action onComplete)
    {
        StartCoroutine(DeadRoutine(onComplete));
    }

    private IEnumerator DeadRoutine(Action onComplete)
    {
        RaiseDiedOnce();
        onComplete?.Invoke();
        Logger.Log($"[ANPC] [{gameObject.name}]는 죽었다!");
        var anim = (this as NPC)?.controller?.Animator;
        stageManager.FindTileByCoord(npcData.hexCoord).ReactBeforeUnitExitThisTile(this);
        RemoveFromSave();
        if (anim != null)
        {
            anim.ResetTrigger(Damage);          // 안전장치
            anim.SetBool(DeadAnim, true);
        }
        else
        {
            OnDieAnimEnd();
        }
        yield return null;
    }
    public override bool IsDead() => base.IsDead() ||npcData.currentHealth <= 0 || !this;
    
    private void SpawnCorpse()
    {
        if (string.IsNullOrEmpty(npcData.corpsePrefabPath))
            return;

        GameObject corpsePrefab = Resources.Load<GameObject>(npcData.corpsePrefabPath);
        if (corpsePrefab == null)
        {
            Logger.LogWarning($"[ANPC] 시체 프리팹 경로가 잘못됨: {npcData.corpsePrefabPath}");
            return;
        }

        Vector3 currentPos = transform.position;
        Quaternion currentRot = transform.rotation;

        GameObject corpse = Instantiate(corpsePrefab, currentPos, currentRot);

        if (!corpse.TryGetComponent<ANPC>(out var corpseNpc))
        {
            Logger.LogWarning($"[ANPC] 시체 프리팹에 ANPC 컴포넌트가 없음");
            return;
        }

        var corpseData = corpseNpc.npcData;
        corpseData.hexCoord = npcData.hexCoord;


        if (npcData.npcDropCardTable != null)
        {
            List<int> dropIDList = npcData.npcDropCardTable.MakeDropIDList();
            corpseData.droppedCardIDList = dropIDList;
        }
    }

    public void MoveAlongPath(List<HexCoord> path, float moveSpeed = 10f, bool drawLine = false)
    {
        if (MoveCoroutine != null)
        {
            StopCoroutine(MoveCoroutine);
            MoveCoroutine = null;
        }

        if (path is not { Count: > 1 })
        {
            return;
        }

        if (npcData.currentActionPoint < npcData.actionPointPerMove)
        {
            return;
        }

        MoveCoroutine = StartCoroutine(MoveRoutine(path, moveSpeed, drawLine));
    }

    private IEnumerator MoveRoutine(List<HexCoord> path, float moveSpeed, bool drawLine = false)
    {
        ATile tile = stageManager.FindTileByCoord(npcData.hexCoord);
        tile?.ReactAfterUnitOutThisTile(this);

        var stageData = stageManager.GetCurStageData();

        for (int i = 0; i < path.Count - 1; i++)
        {
            HexCoord currentHexCoord = path[i];
            HexCoord nextHexCoord = path[i + 1];

            if (!stageData.tiles.TryGetValue(nextHexCoord.ToString(), out TileData nextTileData))
                break;

            int modifier = GameManager.Instance.ruleManager.GetMoveSpeedModifier(npcData, nextTileData);
            int apCost = npcData.actionPointPerMove * modifier;

            if (npcData.currentActionPoint < apCost)
                break;

            npcData.currentActionPoint -= apCost;
            npcData.currentActionPoint = Mathf.Max(0, npcData.currentActionPoint);

            Vector3 startPos = stageManager.ConvertHexToWorld(currentHexCoord);
            Vector3 endPos = stageManager.ConvertHexToWorld(nextHexCoord);
            ATile nextTile = stageManager.FindTileByCoord(nextHexCoord);
            endPos.y += nextTile?.GetObjectOnTileWorldPositionOffset() ?? 0;

            if (sprite)
            {
                Vector3 diff = endPos - startPos;
                if (Mathf.Abs(diff.x) > 0.01f)
                    SetSpriteFlip(diff.x < 0);
            }

            float distance = Vector3.Distance(startPos, endPos);
            float duration = distance / moveSpeed;
            float elapsed = 0f;

            tile = stageManager.FindTileByCoord(npcData.hexCoord);
            tile?.ReactBeforeUnitExitThisTile(this);

            while (elapsed < duration)
            {
                elapsed += stageTime.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                transform.position = Vector3.Lerp(startPos, endPos, t);

                if (drawLine)
                    Debug.DrawLine(startPos, endPos, Color.red);

                yield return null;
            }

            transform.position = endPos;
            npcData.hexCoord = nextHexCoord;

            tile = stageManager.FindTileByCoord(npcData.hexCoord);
            tile?.ReactAfterUnitEnterThisTile(this);

            yield return new WaitForSeconds(0.3f);
        }

        MoveCoroutine = null;
        stageManager.RequestNPCListUpdate();
    }



    [ContextMenu("RemoveFromSave")]
    public void RemoveFromSave()
    {
        if (stageManager == null)
        {
            return;
        }

        stageManager.RemoveNPCPermanently(this, npcData);
    }


    public virtual void StartTurn()
    {
        npcData.turnStarted = true;
        if(npcData.canBehaviour == false)
        {
            return;
        }
        CameraFollow();

        //(this as NPC)?.controller.OnTurnStart(); 
    }

    public virtual HexCoord GetDestinationCoord(Unit target = null)
    {
        if (!target)
            target = GameManager.Instance.gameContext.player;

        var targetCoord = (target is APlayer player)
            ? player.playerStateInStage.hexCoord
            : ((ANPC)target).npcData.hexCoord;

        var nStageManager = GameManager.Instance.campaignManager.stageManager;
        var neighbors = targetCoord.GetNeighbors();

        var candidates = neighbors
            .Where(coord => !nStageManager.IsUnitOnTile(coord) && nStageManager.IsTileExist(coord)).ToList();

        if (candidates.Count == 0)
            return npcData.hexCoord;

        candidates.Sort((a, b) => npcData.hexCoord.Distance(a).CompareTo(npcData.hexCoord.Distance(b)));
        return candidates[0];
    }

    public virtual void EndTurn()
    {
        npcData.turnEnded = true;
    }

    public virtual void RefillTurn()
    {
        npcData.turnEnded = false;
        npcData.turnStarted = true;
        npcData.currentAttackCount = npcData.maxAttackPerTurn;
        npcData.currentActionPoint = npcData.actionPoint;
        npcData.statusEffectProcessedThisTurn = false;
    }

    public bool IsTurnEnded()
    {
        return npcData.turnEnded;
    }


    public override List<UnitEnum.UnitType> GetUnitTypeList()
    {
        return npcData.unitTypeList;
    }

    public override bool IsHaveThisType(UnitEnum.UnitType unitType)
    {
        return npcData.IsHaveThisType(unitType);
    }
    public void OnDieAnimEnd()
    {
        Destroy(gameObject); 
        SpawnCorpse();
        stageManager.RequestNPCListUpdate();
    }
}