using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    public void OnClickChangeScene(string sceneName)
    {
        SceneLoader.Instance.LoadSceneWithFade(sceneName);
    }

}
