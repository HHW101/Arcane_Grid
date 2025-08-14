using UnityEngine;

public class StageTime : MonoSingleton<StageTime>
{
    private GameManager gameManager;
    public float deltaTime => DeltaTime();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    protected void Start()
    {
        gameManager = GameManager.Instance;
    }
    public float DeltaTime()
    {
        if (gameManager.IsPaused())
        {
            return 0f;
        }
        return Time.deltaTime;  
    }
}