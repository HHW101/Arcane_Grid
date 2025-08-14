using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameContext
{
    public string saveDataPath;
    public string defaultSaveDataPath;

    public SaveData saveData;
    public StageManagerParam currentStageManagerParam; 
    
    #region Save Option
    public bool dontSaveCurSceneBundle = false;
    #endregion  

    #region Object In Scene
    public APlayer player = null;
    public List<ANPC> npcList = new List<ANPC>();
    public List<ATile> tileList = new List<ATile>();
    public ACard selectedCard = null;
    #endregion

    #region Temp Scene Data
    public Dictionary<ANPC, NPCData> npcDatas = new Dictionary<ANPC, NPCData>();
    public Dictionary<ATile, TileData> tileDatas = new Dictionary<ATile, TileData>();
    #endregion

    public CampaignPreviewSO selectedCampaignPreviewSO;

    #region Temp Stage Data
    public PlayerStateInStage playerStateInStage = new PlayerStateInStage();
    public int enemyCount = 0;
    #endregion


    public bool isStarted = false;

    public bool isPaused = false; // stage Pause

    public bool isSelectingMoveTarget = false;
    public bool isSelectingAttackTarget = false;
    public bool isSelectingUseCard = false;
    public bool isSelectingInteractTarget = false;

    #region race Condition
    public bool isInputLocked = false;
    public bool isTurnLocked = false;
    #endregion

    public List<ATile> lastSearchShortestPath = new();
    public List<ATile> lastSearchReachableTile = new();
    public List<ANPC> lastSearchAttackableNPC = new();
    public List<ANPC> lastSearchInteractableNPC = new();

    public bool isNPCListUpdateRequested = false;
    

    public AnnounceUI announceUI = null;
    public DialogUI dialogUI = null;
    public ImageUI imageUI = null;
    public InfoUI infoUI = null;
    public DetailInfoUI detailInfoUI = null;
    public HpUI hpUI = null;
    public TurnStartUI turnStartUI = null;

    public GameContext()
    {
    }

    public void Init(string saveDataPath,
        string defaultSaveDataPath)
    {
        this.saveDataPath = saveDataPath;
        this.defaultSaveDataPath = defaultSaveDataPath;
    }

    public void ClearAllSelecting()
    {
        isSelectingMoveTarget = false;
        isSelectingAttackTarget = false;
        isSelectingUseCard = false;
        isSelectingInteractTarget = false;
        isInputLocked = false;
        isNPCListUpdateRequested = false;
        //isBriefingUnitEnded = false;
        //isAnnouncingEnded = false;
    }
}
