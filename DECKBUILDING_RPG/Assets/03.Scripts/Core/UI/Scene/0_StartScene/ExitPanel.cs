using UnityEngine;
using UnityEngine.UI;

public class ExitPanel : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button cancelButton;

    public void Start()
    {
        exitButton.onClick.AddListener(OnExitButtonClick);
        cancelButton.onClick.AddListener(OnCancelButtonClick);
    }

    public void OnCancelButtonClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        this.gameObject.SetActive(false);
    }

    public void OnExitButtonClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        Application.Quit();
    }
}