using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneButton : MonoBehaviour
{
    [SerializeField]
    private string nextScene;
    [SerializeField]
    private GameObject triggerPrefab;

    private void Start()
    {
        if (gameObject.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener( LoadScene);
        }
    }

    public void LoadScene()
    {
        GameManager.Instance.audioManager.PlaySfx("Clicks-008");
        if (nextScene == "5_StageScene")
        {
            if (!GameManager.Instance.cardManager.isDeckCanUse())
                return;
        }
        //SceneManager.LoadScene(nextScene);
        if (triggerPrefab != null)
        {
            Instantiate(triggerPrefab);
        }
        GameManager.Instance.gameContext.saveData.isStarted = false;
      
        SceneLoader.Instance.LoadSceneWithLoadingScreen(nextScene);
    }
}
