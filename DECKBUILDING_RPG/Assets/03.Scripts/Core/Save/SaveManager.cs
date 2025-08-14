using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[Serializable]
public class SaveManagerParam
{
    public string saveDataPath;
    public string defaultSaveDataPath;
    public List<string> abortSceneNameList = new();
    public List<string> stageSceneNameList = new();
}

public class SaveManager : MonoBehaviour
{
    private SaveManagerParam saveManagerParam = new();

    private HashSet<string> abortSceneNames = new();
    private HashSet<string> stageSceneNames = new();
    private GameContext gameContext;

    #region Init
    public void Init(GameContext gameContext, SaveManagerParam saveManagerParam)
    {
        //임시
        saveManagerParam.saveDataPath = Path.Combine(Application.persistentDataPath, saveManagerParam.saveDataPath);
        saveManagerParam.defaultSaveDataPath = Path.Combine(Application.persistentDataPath, saveManagerParam.defaultSaveDataPath);

       
        string dir = Path.GetDirectoryName(saveManagerParam.saveDataPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        gameContext.Init(saveManagerParam.saveDataPath,
        saveManagerParam.defaultSaveDataPath);
        this.gameContext = gameContext;
        this.saveManagerParam = saveManagerParam;
        if (File.Exists(saveManagerParam.saveDataPath))
        {
            Load();
        }
        else if (File.Exists(saveManagerParam.defaultSaveDataPath))
        {
            LoadDefault();
            gameContext.saveData.nowReinforceNum = 3;
            Save();
        }
        else
        {
            gameContext.saveData = new SaveData();
            gameContext.saveData.nowReinforceNum = 3;

            Save();
        }
       
        foreach (string name in saveManagerParam.abortSceneNameList)
        {
            abortSceneNames.Add(name);
        }
        foreach (string name in saveManagerParam.stageSceneNameList)
        {
            stageSceneNames.Add(name);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }
    #endregion

    #region Load
    public void Load()
    {
      
        string json = File.ReadAllText(saveManagerParam.saveDataPath);
        gameContext.saveData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    public void LoadDefault()
    {
        string json = File.ReadAllText(saveManagerParam.defaultSaveDataPath);
        gameContext.saveData = JsonConvert.DeserializeObject<SaveData>(json);
    }

    public void LoadCurrentStageData()
    {
        int curStageIndex = gameContext.saveData.campaignData.playerStateInCampaign.curStageIndex;
        if (gameContext.saveData.campaignData.stageDataList.Count > curStageIndex)
        {
            gameContext.playerStateInStage = gameContext.saveData.campaignData.stageDataList[curStageIndex].playerStateInStage ?? new PlayerStateInStage();
            gameContext.npcDatas.Clear();
            gameContext.tileDatas.Clear();
        }
        else
        {
            Logger.Log($"[GameContext] No saved PlayerStateInStage found for: Stage {curStageIndex}, initialized empty runtime state.");
        }
    }
    #endregion

    #region Save
    public void Save()
    {
        string json = JsonConvert.SerializeObject(gameContext.saveData, Formatting.Indented);
        File.WriteAllText(saveManagerParam.saveDataPath, json);
    }

    public void SaveCurrentStage()
    {
        if (gameContext.player != null)
        {
            gameContext.player.Synchronize();
        }
        foreach (ANPC aNPC in gameContext.npcDatas.Keys)
        {
            if (aNPC != null)
            {
                aNPC.Synchronize();
            }
        }
        foreach (ATile aTile in gameContext.tileDatas.Keys)
        {
            if (aTile != null)
            {
                aTile.Synchronize();
            }
        }
        int curStageIndex = gameContext.saveData.campaignData.playerStateInCampaign.curStageIndex;

        while (gameContext.saveData.campaignData.stageDataList.Count <= curStageIndex)
        {
            gameContext.saveData.campaignData.stageDataList.Add(new StageData());
        }

        Queue<NPCData> npcDataQueue = new();
        foreach(ANPC anpc in gameContext.npcList)
        {
            string prefabPath = gameContext.npcDatas[anpc].prefabPath;
            npcDataQueue.Enqueue(gameContext.npcDatas[anpc]);
        }
        Queue<TileData> tileDataQueue = new Queue<TileData>();
        foreach (ATile aTile in gameContext.tileList)
        {
            string prefabPath = gameContext.tileDatas[aTile].prefabPath;
            tileDataQueue.Enqueue(gameContext.tileDatas[aTile]);
        }

        gameContext.saveData.campaignData.stageDataList[curStageIndex].npcDataQueue = npcDataQueue;
        gameContext.saveData.campaignData.stageDataList[curStageIndex].tileDataQueue = tileDataQueue;
        gameContext.saveData.campaignData.stageDataList[curStageIndex].playerStateInStage = gameContext.playerStateInStage;
    }
    #endregion

    #region Save Option
    [ContextMenu("ClearCurSceneBundle")]
    public void ClearCurSceneBundle()
    {
        gameContext.saveData.sceneDatas.Remove(gameContext.saveData.curSceneName);
    }

    [ContextMenu("DontSaveCurSceneBundle")]
    public void DontSaveCurSceneBundle()
    {
        gameContext.dontSaveCurSceneBundle = true;
    }

    [ContextMenu("ResetSave")]
    public void ResetSave()
    {
        LoadDefault();
        Save();
    }
   
    [ContextMenu("ClearBeforeLoad")]
    public void ClearBeforeLoad()
    {
        if (gameContext.player != null)
        {
            GameObject.Destroy(gameContext.player.gameObject);
        }
        foreach (ANPC aNPC in gameContext.npcDatas.Keys)
        {
            if (aNPC != null)
            {
                GameObject.Destroy(aNPC.gameObject);
            }
        }
        foreach (ATile aTile in gameContext.tileDatas.Keys)
        {
            if (aTile != null)
            {
                GameObject.Destroy(aTile.gameObject);
            }
        }
        gameContext.npcDatas.Clear();
        gameContext.tileDatas.Clear();
        gameContext.npcList.Clear();
        gameContext.tileList.Clear();
        gameContext.enemyCount = 0;
    }

    #endregion

    #region Unity Cycle

    protected void OnDestroy()
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        if (stageSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            SaveCurrentStage();
        }
        Save();
    }

    protected void OnApplicationQuit()
    {
        if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            return;
        }
        if (stageSceneNames.Contains(SceneManager.GetActiveScene().name))
        {
            SaveCurrentStage();
        }
        Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (abortSceneNames.Contains(scene.name))
        {
            return;
        }
        gameContext.saveData.curSceneName = scene.name;
    }

    private void OnSceneUnloaded(Scene scene)
    {
       if (abortSceneNames.Contains(SceneManager.GetActiveScene().name))
       {
           return;
       }
       if (stageSceneNames.Contains(SceneManager.GetActiveScene().name))
       {
           SaveCurrentStage();
       }
       ClearBeforeLoad();
       gameContext.dontSaveCurSceneBundle = false;
    }
    #endregion

    public bool IsSceneSaved(string sceneName)
    {
        if (gameContext.saveData.sceneDatas.ContainsKey(sceneName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public string LastSavedScene()
    {
        return gameContext.saveData.curSceneName;
    }

    public bool IsHaveLastCampaignData()
    {
        return gameContext.saveData.isStartedNewCampaign;
    }

    public void SetHaveLastCampaignData(bool set = false)
    {
        gameContext.saveData.isStartedNewCampaign = set;
    }
}
