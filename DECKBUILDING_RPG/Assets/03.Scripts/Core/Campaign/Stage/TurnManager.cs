using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StatusEffectEnum;
using UnityEngine;

[Serializable]
public class TurnManagerParam
{
    public StatusEffectBookSO statusEffectBookSO;
    public float waitTimeBeforeStartNextProgress = 3f;
}

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PortraitCanvasUI portraitCanvasUI;
    private GameContext gameContext;    
    private TurnManagerParam turnManagerParam;

    public StatusEffectBook statusEffectBook;
    
    public ITurnable currentTurnable { get; private set; }
    private Coroutine turnCoroutine;
    public static event Action OnTurnStarted;
    public static event Action OnTurnEnded;
    public static event Action OnGameStarted;

    public void Init(GameContext context, TurnManagerParam turnManagerParam)
    {
        this.gameContext = context;
        this.turnManagerParam = turnManagerParam;
        
        if (turnManagerParam.statusEffectBookSO)
            statusEffectBook = new StatusEffectBook(turnManagerParam.statusEffectBookSO);
        
    }
    public void StartTurn(Func<bool> CanStageClear, Func<bool> CanStageFail)
    {
             
            
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
        turnCoroutine = StartCoroutine(ProcessTurn(CanStageClear, CanStageFail));
    }

    public void StopTurn()
    {
        if (turnCoroutine != null)
        {
            StopCoroutine(turnCoroutine);
        }
    }

    private IEnumerator ProcessTurn(Func<bool> IsClearFunc, Func<bool> IsFailFunc)
    {
        // Wait Player Register
        yield return null;
        yield return null;
        yield return null;

        bool isCurrentJobDone = false;
        gameContext.isInputLocked = true;
        gameContext.isTurnLocked = true;

        WaitUntil waitUntil = new WaitUntil(() => currentTurnable.IsTurnEnded());

        WaitUntil waitUntilCurrentJobDone = new WaitUntil(() => isCurrentJobDone == true);

        StageStoryManager stageStoryManager = GameManager.Instance.campaignManager.stageManager.stageStoryManager;
        stageStoryManager.SelectStrategyForCurStageType();
        UIManager uiManager = GameManager.Instance.uiManager;

        gameContext.isTurnLocked = true;

        if (gameContext.announceUI != null)
        {
            gameContext.player.CameraFollow();
            isCurrentJobDone = false;
            if (stageStoryManager.CanEnterNextProgress())
            {
                yield return new WaitForSeconds(turnManagerParam.waitTimeBeforeStartNextProgress);
                stageStoryManager.EnterNextProgress(() => isCurrentJobDone = true);
            }
            else
            {
                isCurrentJobDone = true;
            }
            yield return waitUntilCurrentJobDone;
        }

        isCurrentJobDone = false;
        stageStoryManager.AnnounceCurProgress(() => isCurrentJobDone = true);
        yield return waitUntilCurrentJobDone;

        APlayer player = gameContext.player;
        List<ANPC> npcList = new List<ANPC>(gameContext.npcList);
        List<ATile> tileList = new List<ATile>(gameContext.tileList);
        
        List<Unit> turnUnits = new List<Unit> { player };
        turnUnits.AddRange(npcList);

        var popupUI = GameManager.Instance.poolManager.GetPopupUI();
        if (popupUI)
            popupUI.SetPortraitCanvasActive(turnUnits.Count > 0);
        if (portraitCanvasUI)
        {
            gameContext.isNPCListUpdateRequested = true;
        }

        gameContext.isInputLocked = false;
        gameContext.isTurnLocked = false;


        int curStageIndex = gameContext.saveData.campaignData.playerStateInCampaign.curStageIndex;
        StageData stageData = gameContext.saveData.campaignData.stageDataList[curStageIndex];

        if(!gameContext.saveData.isStarted)
        {

            gameContext.saveData.isStarted = true;
            OnGameStarted?.Invoke();
  
        }
        while (true)
        {
             OnTurnStarted?.Invoke();
         
            
            #region StartTurn
            if (stageData.turnPhase == TurnEnum.TurnPhase.Player)
            {
               
                isCurrentJobDone = false;
                uiManager.ShowPlayerTurnStart(() => isCurrentJobDone = true);
                yield return waitUntilCurrentJobDone;

                if (portraitCanvasUI) portraitCanvasUI.HighlightOnly(0);
                
                ProcessStatusEffects(player);
                if (player.isStunned)
                {
                    player.isStunned = false;
                    stageData.turnPhase = TurnEnum.TurnPhase.NPC;
                    yield return waitUntil;
                }
           
                    currentTurnable = player;
                currentTurnable.StartTurn();
                yield return waitUntil;
                if (CheckClearFail(stageData, IsClearFunc, IsFailFunc))
                    yield break;
                stageData.turnPhase = TurnEnum.TurnPhase.NPC;
            }
            GameManager.Instance.campaignManager.stageManager.StartProcessingUnitAct();
            if (stageData.turnPhase == TurnEnum.TurnPhase.NPC)
            {
                isCurrentJobDone = false;
                uiManager.ShowNPCTurnStart(() => isCurrentJobDone = true);
                yield return waitUntilCurrentJobDone;
                for (int i = 0; i < npcList.Count; i++)
                {
                    if (npcList[i] == null) continue;
                    if (npcList[i].npcData.turnEnded == true) continue;
                    
                    if (portraitCanvasUI) portraitCanvasUI.HighlightOnly(i + 1);
                  

                    ProcessStatusEffects(npcList[i]);
                    if (npcList[i].isStunned)
                    {
                        npcList[i].isStunned = false;
                        continue;
                    }
                    currentTurnable = npcList[i];
                    currentTurnable.StartTurn();
                    yield return waitUntil;
                    if (CheckClearFail(stageData, IsClearFunc, IsFailFunc))
                        yield break;
                }
                stageData.turnPhase = TurnEnum.TurnPhase.Tile;
            }
            player.CameraFollow();
            if (stageData.turnPhase == TurnEnum.TurnPhase.Tile)
            {
                for (int i = 0; i < gameContext.tileList.Count; i++)
                {
                    if (tileList[i] == null) continue;
                    if (tileList[i].tileData.turnEnded == true) continue;
                    currentTurnable = tileList[i];
                    currentTurnable.StartTurn();
                    yield return waitUntil;
                    if (CheckClearFail(stageData, IsClearFunc, IsFailFunc))
                        yield break;
                }
            }
            OnTurnEnded?.Invoke();
            #endregion

            stageData.curTurn++;

            #region RefillTurn
            stageData.turnPhase = TurnEnum.TurnPhase.Player;
            player.RefillTurn();
            player.RefillActionPoint();
            player.RefillTechnicalPoint();
            foreach (var npc in gameContext.npcList.Where(npc => npc))
            {
                npc.RefillTurn();
            }
            for (int i = 0; i < gameContext.tileList.Count; i++)
            {
                if (tileList[i] == null) continue;
                tileList[i].RefillTurn();
            }
            #endregion

            GameManager.Instance.campaignManager.stageManager.EndProcessingUnitAct();
        }
    }
    public bool CheckClearFail(StageData stageData, Func<bool> isClearFunc, Func<bool> isFailFunc)
    {
        /*stageData.isClear = isClearFunc();
        if (stageData.isClear)
        {
            if (stageData.clearCampaignIfClearThisStage)
            {
                gameContext.saveData.campaignData.isClear = true;
            }
            return true;
        }*/

        stageData.isFail = isFailFunc();
        if (stageData.isFail)
        {
            if (stageData.failCampaignIfFailThisStage)
            {
                gameContext.saveData.isStarted = false;
                gameContext.saveData.campaignData.isFail = true;
            }
            Logger.Log("졌어요");
            return true;
        }

        return false;
    }
 
    public void AddStatusEffect(Unit unit, StatusEffectType type, int? duration = null)
    {
        var logic = statusEffectBook.GetLogic(type);

        logic?.Apply(unit, duration);
    }
    
    public void AddStatusEffect(Unit unit, StatusEffectSO so, int? durationOverride = null)
    {
        var type = so.effectType;
        int duration = durationOverride ?? so.duration;
        var logic = statusEffectBook.GetLogic(type);

        logic?.Apply(unit, duration);
    }
    
    private void ProcessStatusEffects(Unit unit)
    {
        bool alreadyProcessed = unit switch
        {
            APlayer player => player.playerStateInStage.statusEffectProcessedThisTurn,
            ANPC npc => npc.npcData.statusEffectProcessedThisTurn,
            _ => false
        };

        if (alreadyProcessed)
            return;
        
        for (var i = unit.StatusEffectList.Count - 1; i >= 0; i--)
        {
            var inst = unit.StatusEffectList[i];
            var logic = statusEffectBook.GetLogic(inst.effectType);
            logic?.OnTurnStart(unit, inst);

            inst.remainingTurn--;
            if (inst.remainingTurn > 0) continue;
            logic?.OnRemove(unit, inst);
            unit.StatusEffectList.RemoveAt(i);
        }

        switch (unit)
        {
            case ANPC npc:
                npc.npcData.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                npc.npcData.statusEffectProcessedThisTurn = true;
                break;
            case APlayer player:
                player.playerStateInStage.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                player.playerStateInStage.statusEffectProcessedThisTurn = true;
                break;
        }
    }
    
    public void ApplyStatusEffectToUnit(IStatusEffect effect, Unit unit, int? customTurn = null)
    {
        statusEffectBook.ApplyEffectToUnit(effect, unit, customTurn);
        
        switch (unit)
        {
            case ANPC npc:
                npc.npcData.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                break;
            case APlayer player:
                player.playerStateInStage.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                break;
        }
    }
    
    public void RemoveStatusEffect(Unit unit, StatusEffectType type)
    {
        var idx = unit.StatusEffectList.FindIndex(x => x.effectType == type);
        if (idx < 0) return;
        var inst = unit.StatusEffectList[idx];
        var logic = statusEffectBook.GetLogic(type);
        logic?.OnRemove(unit, inst);
        unit.StatusEffectList.RemoveAt(idx);
        
        switch (unit)
        {
            case ANPC npc:
                npc.npcData.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                break;
            case APlayer player:
                player.playerStateInStage.statusEffectList = new List<StatusEffectInstance>(unit.StatusEffectList);
                break;
        }
    }
    public void RegisterPortraitUI(PortraitCanvasUI ui)
    {
        this.portraitCanvasUI = ui;
    }
}
