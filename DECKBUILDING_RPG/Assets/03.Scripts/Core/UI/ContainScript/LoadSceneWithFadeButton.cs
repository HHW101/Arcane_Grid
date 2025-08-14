using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneWithFadeButton : MonoBehaviour
{
    [SerializeField]
    private string nextScene;
    [SerializeField]
    private GameObject triggerPrefab;

    private void Start()
    {
        if (gameObject.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(LoadScene);
        }
    }

    public void LoadScene()
    {
        if (triggerPrefab != null)
        {
            Instantiate(triggerPrefab);
        }
        GameManager.Instance.gameContext.saveData.isStarted = false;
        SceneLoader.Instance.LoadSceneWithFade(nextScene);
    }
}