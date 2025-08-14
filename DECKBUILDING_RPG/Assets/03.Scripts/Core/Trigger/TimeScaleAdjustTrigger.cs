using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleAdjustTrigger : MonoBehaviour
{
    [SerializeField]
    private bool autoDestroy = false;
    [SerializeField]
    private float timeScale = 1f;
    // Start is called before the first frame update
    private void Start()
    {
        Logger.Log($"[TimeScaleAdjustTrigger] called. [timeScale = {timeScale}]");
        Time.timeScale = timeScale;
        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }
}
