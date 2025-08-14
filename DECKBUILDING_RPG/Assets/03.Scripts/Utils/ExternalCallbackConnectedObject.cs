using System;
using UnityEngine;

public class ExternalCallbackConnectedObject : MonoBehaviour
{
    public Action callback;
    public void Init(Action callback)
    {
        this.callback += callback;
    }

    public void CallBack()
    {
        callback?.Invoke();
    }
}