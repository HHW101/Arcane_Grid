using System;
using UnityEngine;

[System.Serializable]
public class AnimationCallbackConfig
{
    public bool callOnEnter = false;
    public bool callOnUpdate = false;
    public bool callOnExit = true;

    [NonSerialized] public Action onEnter;
    [NonSerialized] public Action onUpdate;
    [NonSerialized] public Action onExit;
}