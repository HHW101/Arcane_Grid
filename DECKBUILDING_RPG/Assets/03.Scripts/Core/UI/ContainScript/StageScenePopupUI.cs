using System.Collections.Generic;
using UnityEngine;

public class StageScenePopupUI : MonoBehaviour
{
    [SerializeField] private GameObject portraitCanvasRoot;
    
    private void Start()
    {
        var poolManager = GameManager.Instance.poolManager;
        var canvas = GetComponent<Canvas>();
        if (poolManager && canvas)
            poolManager.RegisterPopupCanvas(canvas, this);
    }
    
    public void SetPortraitCanvasActive(bool active)
    {
        if (portraitCanvasRoot)
            portraitCanvasRoot.SetActive(active);
    }
}