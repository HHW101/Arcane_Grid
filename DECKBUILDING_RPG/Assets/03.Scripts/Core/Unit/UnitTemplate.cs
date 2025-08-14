using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitTemplate : ScriptableObject //�̰� ��� �޾Ƽ�
{
    public int ID;
    public string nameKey;
    public string flavorTextKey;
    public int Atk; //��Ÿ ��� �ʿ��� ����...

}
