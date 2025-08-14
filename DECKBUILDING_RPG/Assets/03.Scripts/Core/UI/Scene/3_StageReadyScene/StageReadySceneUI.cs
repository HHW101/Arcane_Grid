using UnityEngine;
using UnityEngine.UI;

public class StageReadySceneUI : MonoBehaviour
{
    public void Start()
    {
        GameManager.Instance.gameContext.saveData.playerData.RecoverBeforeStartStage();
    }
}