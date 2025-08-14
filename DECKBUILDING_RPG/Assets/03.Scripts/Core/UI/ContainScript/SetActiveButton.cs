using Unity.VisualScripting;
using UnityEngine;

public class SetActiveButton : MonoBehaviour
{
    [SerializeField]
    private GameObject targetObject;
    [SerializeField]
    private bool setActive = true;

    public void OnClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        targetObject?.SetActive(setActive);
    }
}