using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractUI : AInteractUI
{
    private Action onComplete;
    private CardManager cardManager;
    [SerializeField] private Button closeButton;


    public void Start()
    {
        closeButton?.onClick.AddListener(OnCloseButtonClicked);
    }

    public override void Disable()
    {
        this.gameObject.SetActive(false);
        this.onComplete?.Invoke();
    }

    public override void Enable(Action onComplete)
    {
        this.gameObject.SetActive(true);
        this.onComplete = onComplete;
    }


    public void OnCloseButtonClicked()
    {
        Disable();
    }
}