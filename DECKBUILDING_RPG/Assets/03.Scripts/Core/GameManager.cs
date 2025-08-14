using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerChild
{
    public SaveManager saveManager;
    public CampaignManager campaignManager;
    public JobManager jobManager;
    public CameraManager cameraManager;
    public PoolManager poolManager;             
    public ParticleManager particleManager;     
    public CardManager cardManager;
    public DataManager dataManager;
    public RuleManager ruleManager;
    public EnumAssociatedResourceManager enumAssociatedResourceManager;
    public UIManager uiManager;
    public AudioManager audioManager;
}

[Serializable]
public class GameManagerParam
{
    public SaveManagerParam saveManagerParam;
    public CampaignManagerParam campaignManagerParam;
    public JobManagerParam jobManagerParam;
    public CameraManagerParam cameraManagerParam;
    public PoolManagerParam poolManagerParam;      
    public CardManagerParam cardManagerParam;
    public ParticleManagerParam particleManagerParam;
    public RuleManagerParam ruleManagerParam;
    public EnumAssociatedResourceManagerParam enumAssociatedResourceManagerParam;
    public UIManagerParam uiManagerParam;
    public AudioManagerParam audioManagerParam;
}

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private GameManagerParam gameManagerParam = new GameManagerParam();

    private readonly GameManagerChild gameManagerChild = new GameManagerChild();
    public SaveManager saveManager => gameManagerChild.saveManager;
    public CampaignManager campaignManager => gameManagerChild.campaignManager;
    public JobManager jobManager => gameManagerChild.jobManager;

    public CameraManager cameraManager => gameManagerChild.cameraManager;

    public PoolManager poolManager => gameManagerChild.poolManager; 
    public ParticleManager particleManager => gameManagerChild.particleManager;
    public CardManager cardManager => gameManagerChild.cardManager;
    public DataManager dataManager => gameManagerChild.dataManager;

    public RuleManager ruleManager => gameManagerChild.ruleManager;

    public UIManager uiManager => gameManagerChild.uiManager;

    public AudioManager audioManager => gameManagerChild.audioManager;

    public EnumAssociatedResourceManager enumAssociatedResourceManager => gameManagerChild.enumAssociatedResourceManager;
    
    public readonly GameContext gameContext = new GameContext();

    private void Init()
    {
        GameObject saveManagerObject = new GameObject("SaveManager");
        saveManagerObject.transform.SetParent(this.transform);
        gameManagerChild.saveManager = saveManagerObject.AddComponent<SaveManager>();
        gameManagerChild.saveManager.Init(gameContext, gameManagerParam.saveManagerParam);

        GameObject ruleManager = new GameObject("RuleManager");
        ruleManager.transform.SetParent(this.transform);
        gameManagerChild.ruleManager = ruleManager.AddComponent<RuleManager>();
        gameManagerChild.ruleManager.Init(gameContext, gameManagerParam.ruleManagerParam);


        GameObject uiManagerObject = new GameObject("UIManager");
        uiManagerObject.transform.SetParent(this.transform);
        gameManagerChild.uiManager = uiManagerObject.AddComponent<UIManager>();
        gameManagerChild.uiManager.Init(gameContext, gameManagerParam.uiManagerParam);

        GameObject campaignManagerObject = new GameObject("CampaignManager");
        campaignManagerObject.transform.SetParent(this.transform);
        gameManagerChild.campaignManager = campaignManagerObject.AddComponent<CampaignManager>();
        gameManagerChild.campaignManager.Init(gameContext, gameManagerParam.campaignManagerParam);

        GameObject jobManagerObject = new GameObject("JobManager");
        jobManagerObject.transform.SetParent(this.transform);
        gameManagerChild.jobManager = jobManagerObject.AddComponent<JobManager>();
        gameManagerChild.jobManager.Init(gameContext, gameManagerParam.jobManagerParam);

        GameObject cameraManagerObject = new GameObject("CameraManager");
        cameraManagerObject.transform.SetParent(this.transform);
        gameManagerChild.cameraManager = cameraManagerObject.AddComponent<CameraManager>();
        gameManagerChild.cameraManager.Init(gameContext, gameManagerParam.cameraManagerParam);

        GameObject enumAssociatedResourceManagerObject = new GameObject("EnumAssociatedResourceManager");
        enumAssociatedResourceManagerObject.transform.SetParent(this.transform);
        gameManagerChild.enumAssociatedResourceManager = enumAssociatedResourceManagerObject.AddComponent<EnumAssociatedResourceManager>();
        gameManagerChild.enumAssociatedResourceManager.Init(gameContext, gameManagerParam.enumAssociatedResourceManagerParam);

        GameObject poolManagerObject = new GameObject("PoolManager");
        poolManagerObject.transform.SetParent(this.transform);
        gameManagerChild.poolManager = poolManagerObject.AddComponent<PoolManager>();
        gameManagerChild.poolManager.Init(gameManagerParam.poolManagerParam);
        
        GameObject particleManagerObject = new GameObject("ParticleManager");
        particleManagerObject.transform.SetParent(this.transform);
        gameManagerChild.particleManager = particleManagerObject.AddComponent<ParticleManager>();
        gameManagerChild.particleManager.Init(gameManagerChild.poolManager, gameManagerParam.particleManagerParam);

        GameObject cardManagerObject = new GameObject("CardManager");
        cardManagerObject.transform.SetParent(this.transform);
        gameManagerChild.cardManager = cardManagerObject.AddComponent<CardManager>();
        gameManagerChild.cardManager.Init(gameManagerChild.poolManager, gameManagerParam.cardManagerParam);

        GameObject dataManager = new GameObject("DataManager");
        dataManager.transform.SetParent(this.transform);
        gameManagerChild.dataManager = dataManager.AddComponent<DataManager>();

        GameObject audioManagerObject = new GameObject("AudioManager");
        audioManagerObject.transform.SetParent(this.transform);
        gameManagerChild.audioManager = audioManagerObject.AddComponent<AudioManager>();
        gameManagerChild.audioManager.Init(gameManagerParam.audioManagerParam);
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        Init();
    }

    public void Pause()
    {
        gameContext.isPaused = true;
    }
    public void Continue()
    {
        gameContext.isPaused = false;
    }

    public bool IsPaused()
    {
        return gameContext.isPaused;
    }
}