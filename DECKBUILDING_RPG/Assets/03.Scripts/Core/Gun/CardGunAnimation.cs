using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardGunAnimation
{

    [SerializeField] private string LoadPara = "isLoad";
    [SerializeField] private string PutPara = "isPut";
  
    public int LoadParaHash { get; private set; }
    public int PutParaHash { get; private set; }

    public void Init()
    {
       LoadParaHash = Animator.StringToHash(LoadPara);
       PutParaHash = Animator.StringToHash(PutPara);

    }
}


