using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitTemplate : ScriptableObject //이걸 상속 받아서
{
    public int ID;
    public string nameKey;
    public string flavorTextKey;
    public int Atk; //기타 등등 필요한 스텟...

}
