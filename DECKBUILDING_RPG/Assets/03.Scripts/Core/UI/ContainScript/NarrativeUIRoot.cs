using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeUIRoot : MonoSingleton<NarrativeUIRoot>
{

    // Start is called before the first frame update
    void Start()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
}
