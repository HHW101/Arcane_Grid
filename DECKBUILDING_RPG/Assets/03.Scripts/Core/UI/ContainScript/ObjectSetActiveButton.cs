
using UnityEngine;
using UnityEngine.UI;

public class ObjectSetActiveButton : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private bool setActive;

    public void OnClick()
    {
        targetObject.SetActive(setActive);
    }
}