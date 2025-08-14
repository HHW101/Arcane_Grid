using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitEnum;
public class StageManagerChild
{
    public TurnManager turnManager;
    public StageStoryManager stageStoryManager;
}

[Serializable]
public class StageManagerParam
{
    public float tileRadius = 1.5f;
    public TurnManagerParam turnManagerParam;
    public StageStoryManagerParam stageStoryManagerParam;
}

public class StageManager : MonoBehaviour
{
    #region Init Param

    StageManagerParam stageManagerParam;

    #endregion

    private readonly StageManagerChild stageManagerChild = new();
    public StageData currentLoadedStageData; //퍼블릭으로 바꿈

    public TurnManager turnManager => stageManagerChild.turnManager;
    public StageStoryManager stageStoryManager => stageManagerChild.stageStoryManager;

    private GameContext gameContext;

    private List<ATile> playerReachableTiles = new();
    private List<ATile> playerResultPath = new();
    private List<ATile> attackableNPCs = new();    
    private AStarPathFinder aStarPathFinder = new AStarPathFinder();
    private List<ATile> interactableNPCs = new();

    public void Init(GameContext gameContext, StageManagerParam stageManagerParam)
    {
        this.gameContext = gameContext;
        this.stageManagerParam = stageManagerParam;
        this.gameContext.currentStageManagerParam = this.stageManagerParam;

        GameObject turnManagerObject = new GameObject("TurnManager");
        turnManagerObject.transform.SetParent(this.transform);
        stageManagerChild.turnManager = turnManagerObject.AddComponent<TurnManager>();
        stageManagerChild.turnManager.Init(gameContext, stageManagerParam.turnManagerParam);

        GameObject stageStoryManagerObject = new GameObject("StageStoryManager");
        stageStoryManagerObject.transform.SetParent(this.transform);
        stageManagerChild.stageStoryManager = stageStoryManagerObject.AddComponent<StageStoryManager>();
        stageManagerChild.stageStoryManager.Init(gameContext, stageManagerParam.stageStoryManagerParam);
    }

    [ContextMenu("EnemyCountLog")]
    public void ShowEnemyCountLog()
    {
        Logger.Log(gameContext.enemyCount);
    }

    public bool IsNPCListUpdateRequested()
    {
        return gameContext.isNPCListUpdateRequested;
    }

    public void RequestNPCListUpdate()
    {
        gameContext.isNPCListUpdateRequested = true;
    }

    public void ProcessNPCListUpdateRequest()
    {
        gameContext.isNPCListUpdateRequested = false;
    }


    public void StartBriefingUnit()
    {
        //gameContext.isBriefingUnitEnded = false;
    }

    public void EndBriefingUnit()
    {
        //gameContext.isBriefingUnitEnded = true;
    }

    public void AddDealDamage(int damage)
    {
        GetCurStageData().dealDamage += damage;
        CampaignManager campaignManager = GameManager.Instance.campaignManager;
        campaignManager.GetCampaignData().dealDamage += damage;
    }
    public void AddEarnGold(int gold)
    {
        GetCurStageData().earnGold += gold;
        CampaignManager campaignManager = GameManager.Instance.campaignManager;
        campaignManager.GetCampaignData().earnGold += gold;
    }

    public float GetTileRadius()
    {
        return stageManagerParam.tileRadius;
    }

    public void RegisterNPC(ANPC anpc, NPCData data)
    {
        gameContext.npcDatas[anpc] = data;
        gameContext.npcList.Add(anpc);
        if (data.isEnemy)
        {
            gameContext.enemyCount++;
        }
    }
    public void RegisterCard(ACard card, CardData data)
    {
        
    }

    // 저장 되지 않게 영구 제거
    public void RemoveNPCPermanently(ANPC anpc, NPCData data)
    {
        gameContext.npcDatas.Remove(anpc);
        gameContext.npcList.Remove(anpc);
        if (data.isEnemy)
        {
            gameContext.enemyCount--;
        }
    }

    public int GetEnemyCount()
    {
        int count = 0;
        for(int i = 0; i < gameContext.npcList.Count; i++)
        {
            if (gameContext.npcList[i] != null && gameContext.npcList[i].npcData.isEnemy == true)
            {
                count++;
            }
        }
        return count;
    }

    public void RegisterTile(ATile aTile, TileData data)
    {
        gameContext.tileDatas[aTile] = data;
        gameContext.tileList.Add(aTile);
    }

    // 저장 되지 않게 영구 제거
    public void RemoveTilePermanently(ATile aTile)
    {
        gameContext.tileDatas.Remove(aTile);
        gameContext.tileList.Remove(aTile);
    }

    public void RegisterPlayer(APlayer aPlayer)
    {
        gameContext.player = aPlayer;

        if (gameContext.saveData.playerData == null)
        {
            gameContext.saveData.playerData = aPlayer.playerData;
        }
        else
        {
            aPlayer.playerData = gameContext.saveData.playerData;
        }

        if (!aPlayer.isCreatedInStageLoader)
        {
            aPlayer.playerStateInStage = new();
        }

        gameContext.playerStateInStage = aPlayer.playerStateInStage;
    }

    public void UnRegisterPlayer(APlayer aPlayer)
    {
        if (gameContext.player == aPlayer)
        {
            aPlayer.Synchronize();
        }

        if (gameContext.player != null && gameContext.player == aPlayer)
        {
            gameContext.player = null;
        }
    }

    public void LoadStage()
    {
        GameManager.Instance.saveManager.DontSaveCurSceneBundle();
        SaveManager saveManager = GameManager.Instance.saveManager;
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        currentLoadedStageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        saveManager.LoadCurrentStageData();
        LoadTile(campaignData);
        LoadNPC(campaignData);
    
        LoadPlayer(campaignData);

        stageStoryManager.SelectStrategyForCurStageType();

        turnManager.StartTurn(CanStageClear, CanStageFail);
    }

    public void DescendStage()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        ChangeStage(campaignData, 1);
    }

    public void AscendStage()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        ChangeStage(campaignData, -1);
    }

    public int GetCurStageIndex()
    {
        return gameContext.saveData.campaignData.playerStateInCampaign.curStageIndex;
    }

    public bool ForceClearCurStage()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        stageData.isClear = true;
        GameManager.Instance.gameContext.player.cardController.ResetDeck();
        gameContext.saveData.isStarted = false;
        gameContext.saveData.nowReinforceNum = 3;
        if (stageData.clearCampaignIfClearThisStage)
        {
            campaignData.isClear = true;
        }
        return true;
    }

    public StageData GetCurStageData()
    {
        return gameContext.saveData.campaignData.stageDataList[GetCurStageIndex()];
    }

    public void ReadyToMove()
    {
        gameContext.isSelectingMoveTarget = true;
    }

    public bool IsReadyToMove()
    {
        return gameContext.isSelectingMoveTarget;
    }

    public void ReadyToAttack()
    {
        gameContext.isSelectingAttackTarget = true;
    }

    public bool IsReadyToAttack()
    {
        return gameContext.isSelectingAttackTarget;
    }

    public void ReadyToUseCard()
    {
        gameContext.isSelectingUseCard = true;
    }

    public bool IsReadyToUseCard()
    {
        return gameContext.isSelectingUseCard;
    }

    public void ReadyToInteract()
    {
        gameContext.isSelectingInteractTarget = true;
    }

    public bool IsReadyToInteract()
    {
        return gameContext.isSelectingInteractTarget;
    }
    [ContextMenu("StartProcessingUnitAct")]
    public void StartProcessingUnitAct()
    {
        gameContext.isInputLocked = true;
    }

    public bool IsProcessingUnitAct()
    {
        return gameContext.isInputLocked;
    }
    [ContextMenu("EndProcessingUnitAct")]
    public void EndProcessingUnitAct()
    {
        Logger.Log("Process End");
        gameContext.isInputLocked = false;
    }


    public Vector3 ConvertHexToWorld(HexCoord coord)//Byunggil
    {
        return HexToWorld(coord);
    }

    public void CancelAction()
    {
        gameContext.isSelectingMoveTarget = false;
        gameContext.isSelectingAttackTarget = false;
        gameContext.isSelectingUseCard = false;
        gameContext.isSelectingInteractTarget = false;
    }

    public bool IsStageClear()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        return stageData.isClear;
    }

    public bool IsStageFail()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        return stageData.isFail;
    }

    public bool CanStageClear()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        if (stageData.isClear){
            return true;
        }
        if(stageData.clearCondition == ClearEnum.ClearCondition.Escape)
        {
            return false;
        }
        if (stageData.clearCondition == ClearEnum.ClearCondition.KillAllEnemy
            && gameContext.enemyCount == 0)
        {
            return true;
        }

        if (stageData.clearCondition == ClearEnum.ClearCondition.PlayerSurvive
            && stageData.curTurn >= stageData.surviveTurn)
        {
            return true;
        }

        return false;
    }

    public bool CanStageFail()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        if (stageData.isFail)
        {
            return true;
        }
        if (gameContext.saveData.playerData.isDead)
        {
            return true;
        }

        if (stageData.failCondition == FailEnum.FailCondition.OverTurn
            && stageData.curTurn > stageData.overTurn)
        {
            return true;
        }

        return false;
    }

    public bool IsTileExist(HexCoord coord) //Byunggil
    {
        foreach (var tile in gameContext.tileDatas.Keys)
        {
            if (tile.tileData.hexCoord.Equals(coord))
                return true;
        }
        return false;
    }

    public List<HexCoord> GetNpcAroundTile(HexCoord centerCoord, int minDistance = 1, int maxDistance = 2) //Byunggil
    {
        List<HexCoord> existTiles = new List<HexCoord>();

        foreach (var tile in gameContext.tileDatas.Keys)
        {
            HexCoord coord = tile.tileData.hexCoord;
            int distance = centerCoord.Distance(coord);

            if (distance >= minDistance && distance <= maxDistance)
            {
                existTiles.Add(coord);
            }
        }

        return existTiles;
    }

    public List<HexCoord> FindNpcPath(HexCoord startCoord, HexCoord targetCoord, UnitData unitData)
    {
        var stageData = GetCurStageData();
        return aStarPathFinder.FindPathWithWeightedCost(
            stageData,
            startCoord,
            targetCoord,
            (from, to) => CalculateMovementCostForAStar(from, to, stageData, unitData)
        );
    }

    [ContextMenu("CheackStageClearFail")]
    public void CheckStageClearFail()
    {
        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        turnManager.CheckClearFail(stageData, CanStageClear, CanStageFail);
    }

    public List<ATile> GetReachableTileList()
    {
        RuleManager ruleManager = GameManager.Instance.ruleManager;
        playerReachableTiles.Clear();

        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        PlayerData playerData = gameContext.saveData.playerData;

        int maxMoveCost = playerData.actionPoint / playerData.actionPointPerMove;

        HexCoord start = stageData.playerStateInStage.hexCoord;

        Dictionary<HexCoord, int> bestCost = new Dictionary<HexCoord, int>();
        Queue<HexCoord> q = new Queue<HexCoord>();

        bestCost[start] = 0;
        q.Enqueue(start);

        while (q.Count > 0)
        {
            HexCoord cur = q.Dequeue();
            int curCost = bestCost[cur];

            if (curCost >= maxMoveCost)
                continue;

            if (stageData.tileConnections.TryGetValue(cur.ToString(), out var connections))
            {
                foreach (var kvp in connections)
                {
                    HexCoord next = HexCoord.FromString(kvp.Value);

                    if (IsUnitOnTile(next))
                        continue;

                    if (!stageData.tiles.TryGetValue(next.ToString(), out TileData nextTile))
                        continue;

                    if (!ruleManager.CanUnitEnterTile(playerData, nextTile))
                        continue;

                    int modifier = ruleManager.GetMoveSpeedModifier(playerData, nextTile);
                    if (modifier < 1) modifier = 1;

                    int newCost = curCost + modifier;
                    if (newCost > maxMoveCost)
                        continue;

                    if (!bestCost.TryGetValue(next, out int prev) || newCost < prev)
                    {
                        bestCost[next] = newCost;
                        q.Enqueue(next);
                    }
                }
            }
        }

        foreach (var kvp in bestCost)
        {
            HexCoord coord = kvp.Key;
            if (coord.Equals(start))
                continue;

            if (stageData.tiles.TryGetValue(coord.ToString(), out TileData tileData))
            {
                foreach (var pair in gameContext.tileDatas)
                {
                    if (pair.Value.hexCoord.Equals(tileData.hexCoord))
                    {
                        playerReachableTiles.Add(pair.Key);
                        break;
                    }
                }
            }
        }

        gameContext.lastSearchReachableTile = playerReachableTiles;
        return playerReachableTiles;
    }

    public List<ATile> FindShortestPathAndRecordInGameContext(HexCoord targetCoord, UnitData unitData)
    {
        playerResultPath.Clear();

        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];

        HexCoord start = stageData.playerStateInStage.hexCoord;
        
        List<HexCoord> path = aStarPathFinder.FindPathWithWeightedCost(
            stageData,
            start,
            targetCoord,
            (from, to) => CalculateMovementCostForAStar(from, to, stageData, unitData) 
        );

        if (path == null || path.Count == 0) 
        {
            gameContext.lastSearchShortestPath.Clear();
            return playerResultPath; 
        }
        
        if (path.Count == 1 && path[0].Equals(start) && !start.Equals(targetCoord))
        {
            gameContext.lastSearchShortestPath.Clear();
            return playerResultPath;
        }
        
        foreach (HexCoord coord in path)
        {
            if (stageData.tiles.TryGetValue(coord.ToString(), out TileData tileData))
            {
                foreach (var pair in gameContext.tileDatas)
                {
                    if (pair.Value.hexCoord.Equals(tileData.hexCoord))
                    {
                        playerResultPath.Add(pair.Key);
                        break;
                    }
                }
            }
        }
        if (playerResultPath.Count > 0)
        {
            playerResultPath.RemoveAt(0);
        }

        gameContext.lastSearchShortestPath = playerResultPath;
        return playerResultPath;
    }

    private float CalculateMovementCostForAStar(HexCoord from, HexCoord to, StageData stageData, UnitData unitData)
    {
        if (IsUnitOnTile(to))
            return float.PositiveInfinity;

        if (!stageData.tiles.TryGetValue(to.ToString(), out TileData tileData))
            return float.PositiveInfinity;

        if (!GameManager.Instance.ruleManager.CanUnitEnterTile(unitData, tileData))
            return float.PositiveInfinity;

        int modifier = GameManager.Instance.ruleManager.GetMoveSpeedModifier(unitData, tileData);
        return 1f * modifier; 
    }

    public List<ANPC> GetAttackableNPCList()
    {
        GameContext gameContext = GameManager.Instance.gameContext;

        // 사전 리스트 초기화
        gameContext.lastSearchAttackableNPC.Clear();

        var campaignData = gameContext.saveData.campaignData;
        var playerState = campaignData.stageDataList[campaignData.playerStateInCampaign.curStageIndex].playerStateInStage;
        var playerCoord = playerState.hexCoord;


        if(gameContext.player == null)
        {
            return null;
        }

        int attackRange = gameContext.player.GetNextAttackInfo().range;

        foreach (var npc in gameContext.npcList)
        {
            HexCoord npcCoord = npc.npcData.hexCoord;
            int distance = HexCoord.Distance(playerCoord, npcCoord);

            if (distance <= attackRange)
            {
                gameContext.lastSearchAttackableNPC.Add(npc);
            }
        }

        return gameContext.lastSearchAttackableNPC;
    }
   
    public List<ATile> GetCardUsableTileList(ACard card)
    {
        List<ATile> tileList = new List<ATile>();

        CampaignData campaignData = gameContext.saveData.campaignData;
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        StageData stageData = campaignData.stageDataList[playerStateInCampaign.curStageIndex];
        PlayerData playerData = gameContext.saveData.playerData;
        ACard selectedCard = gameContext.selectedCard;


        Queue<(HexCoord coord, int moveCost)> queue = new Queue<(HexCoord, int)>();
        HashSet<HexCoord> visited = new HashSet<HexCoord>();
        int range= card.Range;

        HexCoord start = stageData.playerStateInStage.hexCoord;
        queue.Enqueue((start, 0));
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (currentCoord, currentCost) = queue.Dequeue();

            if (stageData.tiles.TryGetValue(currentCoord.ToString(), out TileData tileData))
            {
                foreach (var pair in gameContext.tileDatas)
                {
                    if (pair.Value.hexCoord.Equals(tileData.hexCoord))
                    {
                       tileList.Add(pair.Key);
                        break;
                    }
                }
            }

            if (currentCost >= range)
            {
                continue;
            }

            if (stageData.tileConnections.TryGetValue(currentCoord.ToString(), out var connections))
            {
                foreach (var kvp in connections)
                {
                    HexCoord nextCoord = HexCoord.FromString(kvp.Value);  
                    if (!visited.Contains(nextCoord))
                    {
                        visited.Add(nextCoord);
                        queue.Enqueue((nextCoord, currentCost + 1));
                    }
                }
            }
        }
        
        

        return tileList;
    }

    public List<ANPC> GetInteractableNPCList()
    {
        GameContext gameContext = GameManager.Instance.gameContext;

        // 사전 리스트 초기화
        gameContext.lastSearchInteractableNPC.Clear();

        var campaignData = gameContext.saveData.campaignData;
        var playerState = campaignData.stageDataList[campaignData.playerStateInCampaign.curStageIndex].playerStateInStage;
        var playerCoord = playerState.hexCoord;

        int interactRange = gameContext.saveData.playerData.interactRange;

        foreach (var npc in gameContext.npcList)
        {
            HexCoord npcCoord = npc.npcData.hexCoord;
            int distance = HexCoord.Distance(playerCoord, npcCoord);

            if (distance <= interactRange)
            {
                gameContext.lastSearchInteractableNPC.Add(npc);
            }
        }

        return gameContext.lastSearchInteractableNPC;
    }

    #region private

    private void ChangeStage(CampaignData campaignData, int index)
    {
        //turnManager.StopTurn();
        //SaveManager saveManager = GameManager.Instance.saveManager;
        int targetStage = campaignData.playerStateInCampaign.curStageIndex + index;
        if (targetStage < campaignData.stageDataList.Count && targetStage > -1)
        {
            //saveManager.SaveCurrentStage();
            //saveManager.Save();
            //saveManager.ClearBeforeLoad();
            campaignData.playerStateInCampaign.curStageIndex = targetStage;
            //LoadStage();
        }
        else
        {
            targetStage -= index;
            Logger.Log($"[StageLoader] targetStage index out! [targetStage : {targetStage}]");
        }
    }

    private void LoadNPC(CampaignData campaignData)
    {
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        Queue<NPCData> queue = campaignData.stageDataList[playerStateInCampaign.curStageIndex].npcDataQueue;
        while (queue.Count > 0)
        {
            NPCData npcData = queue.Dequeue();

            GameObject npcPrefab = Resources.Load<GameObject>(npcData.prefabPath);
            if (npcPrefab != null)
            {
                GameObject npc = GameObject.Instantiate(npcPrefab);
                npc.transform.position = HexToWorld(npcData.hexCoord);
                if (npc.TryGetComponent<ANPC>(out ANPC _npc))
                {
                    _npc.npcData = npcData;
                }
            }
            else
            {
                Logger.LogError($"Prefab not found at path: {npcData.prefabPath}");
            }
        }
    }

    private void LoadTile(CampaignData campaignData)
    {
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        Queue<TileData> queue = campaignData.stageDataList[playerStateInCampaign.curStageIndex].tileDataQueue;
        while (queue.Count > 0)
        {
            TileData tileData = queue.Dequeue();

            GameObject npcPrefab = Resources.Load<GameObject>(tileData.prefabPath);
            if (npcPrefab != null)
            {
                GameObject tile = GameObject.Instantiate(npcPrefab);
                tile.transform.position = HexToWorld(tileData.hexCoord);
                if (tile.TryGetComponent<ATile>(out ATile _tile))
                {
                    _tile.tileData = tileData;
                }
            }
            else
            {
                Logger.LogError($"Prefab not found at path: {tileData.prefabPath}");
            }
        }
      

    }

    private void LoadPlayer(CampaignData campaignData)
    {
        PlayerStateInCampaign playerStateInCampaign = campaignData.playerStateInCampaign;
        PlayerData playerData = gameContext.saveData.playerData;
        PlayerStateInStage playerStateInStage =
            campaignData.stageDataList[playerStateInCampaign.curStageIndex].playerStateInStage;
        GameObject playerPrefab = Resources.Load<GameObject>(playerData.prefabPath);
        if (playerPrefab != null)
        {
            GameObject player = GameObject.Instantiate(playerPrefab);
            player.transform.position = HexToWorld(playerStateInStage.hexCoord);    
            if (player.TryGetComponent<APlayer>(out APlayer _player))
            {
                _player.playerData = playerData;
                _player.playerStateInStage = playerStateInStage;
                _player.isCreatedInStageLoader = true;
            }
        }
    }


    private Vector3 HexToWorld(HexCoord coord)
    {
        float x = stageManagerParam.tileRadius * 1.5f * coord.q;
        float y = stageManagerParam.tileRadius * Mathf.Sqrt(3f) * (coord.r + coord.q * 0.5f);
        return new Vector3(x, y, 0f);
    }

    #endregion
    
    public bool IsUnitOnTile(HexCoord coord)
    {
        if (gameContext.player != null && gameContext.player.playerStateInStage.hexCoord.Equals(coord))
            return true;
        
        foreach (var npc in gameContext.npcList)
        {
            if (npc.npcData.hexCoord.Equals(coord))
                return true;
        }
        return false;
    }

    public bool IsUnitOnTileInStageData(HexCoord coord, StageData stageData)
    {
        foreach (var npcData in stageData.npcDataQueue)
        {
            if (npcData.hexCoord.Equals(coord))
                return true;
        }
        return false;
    }

    public List<TileData> GetNoUnitTileList()
    {
        List<TileData> result = new List<TileData>();
        var stageData = GetCurStageData();

        foreach (var kvp in stageData.tiles)
        {
            var coord = kvp.Value.hexCoord;

            if (!IsUnitOnTile(coord))
            {
                result.Add(kvp.Value);
            }
        }

        return result;
    }

    public void OnPlayerExitCoord(HexCoord coord, APlayer player, Action onComplete)
    {
        ATile tile = FindTileByCoord(coord);
        if (tile != null)
        {
            tile.ReactBeforeUnitExitThisTile(player, onComplete);
        }
        else
        {
            Logger.LogWarning($"[StageManager] No tile found at coord {coord}");
            onComplete?.Invoke();
        }
    }


    public void OnPlayerEnterCoord(HexCoord coord, APlayer player, Action onComplete)
    {
        ATile tile = FindTileByCoord(coord);
        if (tile != null)
        {
            tile.ReactAfterUnitEnterThisTile(player, onComplete);
        }
        else
        {
            Logger.LogWarning($"[StageManager] No tile found at coord {coord}");
            onComplete?.Invoke();
        }
    }

    public ATile FindTileByCoord(HexCoord coord)
    {
        foreach (var pair in gameContext.tileDatas)
        {
            if (pair.Value.hexCoord.Equals(coord))
            {
                return pair.Key;
            }
        }
        return null;
    }
    
    public List<ATile> GetTilesInRange(HexCoord center, int radius)
    {
        List<ATile> tilesInRange = new List<ATile>();
        foreach (var pair in gameContext.tileDatas)
        {
            if (center.Distance(pair.Value.hexCoord) <= radius)
            {
                tilesInRange.Add(pair.Key);
            }
        }
        return tilesInRange;
    }

    public List<ANPC> GetNPCsInRange(HexCoord center, int radius)
    {
        List<ANPC> npcsInRange = new List<ANPC>();
        foreach (var npc in gameContext.npcList)
        {
            if (npc != null && center.Distance(npc.npcData.hexCoord) <= radius)
            {
                npcsInRange.Add(npc);
            }
        }
        return npcsInRange;
    }
    public Unit GetUnitAt(HexCoord coord)
    {
        if (gameContext.player && gameContext.player.playerStateInStage.hexCoord.Equals(coord))
            return gameContext.player;

        return gameContext.npcList.FirstOrDefault(n => n && n.npcData.hexCoord.Equals(coord));
    }

    public List<HexCoord> FindNpcPathIgnoringNeutral(HexCoord startCoord, HexCoord targetCoord, UnitData unitData)
    {
        var stageData = GetCurStageData();
        return aStarPathFinder.FindPathWithWeightedCost(
            stageData,
            startCoord,
            targetCoord,
            (from, to) => CalculateMovementCostForAStar(from, to, stageData, unitData, ignoreNeutralBlocks: true)
        );
    }

    private float CalculateMovementCostForAStar(HexCoord from, HexCoord to, StageData stageData, UnitData unitData, bool ignoreNeutralBlocks)
    {
        var occ = GetUnitAt(to);
        if (occ)
        {
            var camp = GameManager.Instance.ruleManager.GetUnitCampType(occ.unitData);
            if (!(ignoreNeutralBlocks && camp == UnitCampType.Neutral))
                return float.PositiveInfinity; 
        }

        if (!stageData.tiles.TryGetValue(to.ToString(), out TileData tileData) || !GameManager.Instance.ruleManager.CanUnitEnterTile(unitData, tileData))
            return float.PositiveInfinity;

        int modifier = GameManager.Instance.ruleManager.GetMoveSpeedModifier(unitData, tileData);
        if (modifier < 1) modifier = 1;

        return 1f * modifier;
    }

}