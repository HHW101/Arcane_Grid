using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    [SerializeField] private GameObject optionPanel; 

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>(); 

        if (button == null)
        {
            return;
        }

        if (optionPanel == null)
        {
            return;
        }

        button.onClick.AddListener(OnClick);

        optionPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnClick);
        }
    }

    private void OnClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        optionPanel.SetActive(true);
    }
}