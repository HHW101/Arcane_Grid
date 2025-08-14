using UnityEngine;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    [SerializeField]
    private Button cancelButton;

    public void Start()
    {
        cancelButton.onClick.AddListener(OnCancleButtonClick);
    }

    public void OnCancleButtonClick()
    {
        gameObject.SetActive(false);
    }
}