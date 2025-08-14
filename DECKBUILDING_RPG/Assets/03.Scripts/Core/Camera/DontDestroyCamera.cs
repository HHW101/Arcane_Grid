public class DontDestroyCamera : MonoSingleton<DontDestroyCamera>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}