using UnityEngine;

public class PauseUI : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseUIRoot;
    GameManager gameManager;

    public void Start()
    {
        pauseUIRoot.gameObject.SetActive(false);
        gameManager = GameManager.Instance;
    }

    public void Pause()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-001");
        pauseUIRoot.gameObject.SetActive(true);
        gameManager.Pause();
    }

    public void Continue()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-001");
        gameManager.Continue();
        pauseUIRoot.gameObject.SetActive(false);
    }
}