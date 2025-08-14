using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSaveSceneButton : MonoBehaviour
{
    [SerializeField] private string defaultNextScene;
    SaveManager saveManager;

    private void Start()
    {
        saveManager = GameManager.Instance.saveManager;
        if (gameObject.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }

    public void OnButtonClick()
    {
        GameManager.Instance.audioManager.PlaySfx("Wood_Type_02_07");
        // SceneManager.LoadScene(saveManager.LastSavedScene() ?? defaultNextScene);
        string sceneToLoad = saveManager.LastSavedScene() ?? defaultNextScene;

        SceneLoader.Instance.LoadSceneWithLoadingScreen(sceneToLoad);

    }
}

