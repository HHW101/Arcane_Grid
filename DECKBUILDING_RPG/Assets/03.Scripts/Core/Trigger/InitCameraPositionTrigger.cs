using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCameraPositionTrigger : MonoBehaviour
{
    [SerializeField]
    private bool autoDestroy = true;
    // Start is called before the first frame update
    private void Start()
    {
        Logger.Log("[InitCameraPositionTrigger] called.");
        Camera.main.transform.position = new Vector3(0, 0, -10);
        if (autoDestroy)
        {
            Destroy(gameObject);
        }
    }
}
