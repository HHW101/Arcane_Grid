using TMPro;
using UnityEngine;

public class ResultPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text dealDamageText;
    [SerializeField]
    private TMP_Text earnGoldText;

    public void Init(int dealDamage, int earnGold)
    {
        dealDamageText.SetText($"dealed Damage : {dealDamage}");
        earnGoldText.SetText($"Earned Gold : {earnGold}");
    }
}