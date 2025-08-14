using System.Collections.Generic;
using NPCEnum;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnitEnum;

public class NpcController : MonoBehaviour
{   
    public ANPC npc;
    [SerializeField] private Animator animator;
    public Animator Animator => animator;
    
    public NPCStateResult currentStateType = NPCStateResult.None; 
    public INPCState currentState;

    private readonly Dictionary<NPCStateResult, INPCState> stateMap = new();
    private Coroutine npcTurnRoutine; 

    private StageManager Stage => npc?.StageManager;
    private RuleManager Rule => GameManager.Instance.ruleManager;
    
    public void Init(ANPC npc)
    {
        this.npc = npc;
        
        stateMap[NPCStateResult.ToIdle]    = new NpcIdleState();
        stateMap[NPCStateResult.ToPatrol]  = new NpcPatrolState();
        stateMap[NPCStateResult.ToChase]   = new NpcChaseState();
        stateMap[NPCStateResult.ToAttack]  = new NpcAttackState();
    }

    public void OnTurnStart()
    {
        if (npcTurnRoutine != null)
        {
            StopCoroutine(npcTurnRoutine);
        }
        ChangeStateInternal(NPCStateResult.ToIdle);
        npcTurnRoutine = StartCoroutine(ProcessNPCTurnRoutine());
    }
    
    private IEnumerator ProcessNPCTurnRoutine()
    {
        NPCStateResult nextStateSignal = NPCStateResult.None;

        while (!npc.npcData.turnEnded && npc.npcData.currentHealth > 0)
        {
            if (currentState == null) break;

            bool stateSignaled = false;
            IEnumerator actionRoutine = currentState.Execute(this, result =>
            {
                nextStateSignal = result;
                stateSignaled = true;
            });

            if (actionRoutine != null)
                yield return StartCoroutine(actionRoutine);

            if (stateSignaled)
            {
                switch (nextStateSignal)
                {
                    case NPCStateResult.ToIdle:    ChangeStateInternal(NPCStateResult.ToIdle); break;
                    case NPCStateResult.ToPatrol:  ChangeStateInternal(NPCStateResult.ToPatrol); break;
                    case NPCStateResult.ToChase:   ChangeStateInternal(NPCStateResult.ToChase); break;
                    case NPCStateResult.ToAttack:  ChangeStateInternal(NPCStateResult.ToAttack); break;
                    case NPCStateResult.EndTurn:   FinishNpcTurn(); yield break;
                    case NPCStateResult.None:      break;
                }
                nextStateSignal = NPCStateResult.None;
            }
            else
            {
                if (npc.npcData.currentActionPoint <= 0)
                {
                    FinishNpcTurn();
                    yield break;
                }
            }

            yield return null;
        }

        if (!npc.npcData.turnEnded)
            FinishNpcTurn();
    }
    
    public void ChangeState(NPCStateResult newStateType) => ChangeStateInternal(newStateType);
    
    private void ChangeStateInternal(NPCStateResult newStateType)
    {
        currentState?.Exit(this);

        currentStateType = newStateType;
        currentState = stateMap[newStateType];
        
        currentState?.Enter(this); 
        npc?.Synchronize(); 
    }

    private void FinishNpcTurn()
    {
        currentState?.Exit(this);
        if (npcTurnRoutine != null)
        {
            StopCoroutine(npcTurnRoutine); 
            npcTurnRoutine = null;
        }
        npc.EndTurn(); 
    }

    private int GetDistanceToTarget(HexCoord targetCoord) => npc.npcData.hexCoord.Distance(targetCoord);
    public bool IsTargetInRange(HexCoord targetCoord, int range) => GetDistanceToTarget(targetCoord) <= range;

    private HexCoord GetUnitCoord(Unit unit)
    {
        if (unit is APlayer player) return player.playerStateInStage.hexCoord;
        if (unit is ANPC anpc)      return anpc.npcData.hexCoord;
        return default;
    }

    public Unit PeekTargetInDetectRange()
    {
        var list = CollectPrimaryTargets();
        return (list.Count > 0) ? list[0] : null;
    }

    public Unit GetTarget()
    {
        var candidates = CollectPrimaryTargets();
        if (candidates.Count == 0) return null;

        var blockingNeutrals = new List<Unit>();

        foreach (var cand in candidates)
        {
            if (TryFindBestNormalApproach(cand, out _, out _))
                return cand;

            var blockers = CollectBlockingNeutralsIfAllNormalFail(cand);
            if (blockers != null && blockers.Count > 0)
                blockingNeutrals.AddRange(blockers);
        }

        if (blockingNeutrals.Count > 0)
        {
            var unique = blockingNeutrals.Distinct().ToList();
            unique.Sort((a, b) =>
            {
                int da = npc.npcData.hexCoord.Distance(GetUnitCoord(a));
                int db = npc.npcData.hexCoord.Distance(GetUnitCoord(b));
                return da.CompareTo(db);
            });
            return unique[0];
        }

        return null;
    }

    private List<Unit> CollectPrimaryTargets()
    {
        var list = new List<Unit>();

        var player = GameManager.Instance.gameContext.player;
        if (player != null && (npc.npcData.isEnemy || npc.npcData.isAggroToPlayer))
            list.Add(player);

        foreach (var u in GameManager.Instance.gameContext.npcList)
        {
            if (u == npc) continue;

            if (u.unitData is NPCData nd && nd.isAttackableByEnemies)
            {
                var camp = Rule.GetUnitCampType(u.unitData);
                if (camp == UnitCampType.Ally) list.Add(u);
            }
        }

        list = list
            .Where(u => u != null && npc.npcData.hexCoord.Distance(GetUnitCoord(u)) <= npc.npcData.detectRange)
            .ToList();

        list.Sort((a, b) =>
        {
            int da = npc.npcData.hexCoord.Distance(GetUnitCoord(a));
            int db = npc.npcData.hexCoord.Distance(GetUnitCoord(b));
            if (da != db) return da.CompareTo(db);
            if (a is APlayer) return -1;
            if (b is APlayer) return 1;
            return 0;
        });

        return list;
    }

    private IEnumerable<HexCoord> GetApproachTiles(Unit target)
    {
        var targetCoord = GetUnitCoord(target);
        var stageData = Stage.GetCurStageData();

        foreach (var adj in targetCoord.GetNeighbors())
        {
            if (!stageData.tiles.TryGetValue(adj.ToString(), out var tileData)) continue;
            if (!Rule.CanUnitEnterTile(npc.unitData, tileData)) continue;
            if (Stage.IsUnitOnTile(adj)) continue;
            yield return adj;
        }
    }

    public bool TryFindBestNormalApproach(Unit target, out HexCoord dest, out List<HexCoord> bestPath)
    {
        dest = default;
        bestPath = null;

        var start = npc.npcData.hexCoord;
        int bestCost = int.MaxValue;

        foreach (var ap in GetApproachTiles(target))
        {
            var path = Stage.FindNpcPath(start, ap, npc.unitData);
            if (HasUsablePath(path, start, ap))
            {
                int cost = path.Count; 
                if (cost < bestCost)
                {
                    bestCost = cost;
                    dest = ap;
                    bestPath = path;
                }
            }
        }
        return bestPath != null;
    }

    private List<Unit> CollectBlockingNeutralsIfAllNormalFail(Unit target)
    {
        var start = npc.npcData.hexCoord;

        foreach (var ap in GetApproachTiles(target))
        {
            var normal = Stage.FindNpcPath(start, ap, npc.unitData);
            if (HasUsablePath(normal, start, ap))
                return null;
        }

        bool anyRelaxed = false;
        var blockers = new HashSet<Unit>();
        foreach (var ap in GetApproachTiles(target))
        {
            var relaxed = Stage.FindNpcPathIgnoringNeutral(start, ap, npc.unitData);
            if (HasUsablePath(relaxed, start, ap))
            {
                anyRelaxed = true;
                var b = FindBlockingNeutralPreferAdjacent(relaxed, start, ap);
                if (b != null) blockers.Add(b);
            }
        }
        return anyRelaxed ? blockers.ToList() : null;
    }

    private Unit FindBlockingNeutralPreferAdjacent(List<HexCoord> path, HexCoord start, HexCoord dest)
    {
        foreach (var n in start.GetNeighbors())
        {
            var u = Stage.GetUnitAt(n);
            if (u == null) continue;
            if (Rule.GetUnitCampType(u.unitData) != UnitCampType.Neutral) continue;
            if (u.unitData is NPCData nd && nd.isAttackableByEnemies)
            {
                if (n.Distance(dest) < start.Distance(dest))
                    return u;
            }
        }

        return path
            .Skip(1)
            .Select(c => Stage.GetUnitAt(c))
            .Where(u => u != null && Rule.GetUnitCampType(u.unitData) == UnitCampType.Neutral)
            .FirstOrDefault(u => u.unitData is NPCData nd && nd.isAttackableByEnemies);
    }

    private bool HasUsablePath(List<HexCoord> path, HexCoord start, HexCoord goal)
    {
        return path != null && path.Count > 1;
    }
}
