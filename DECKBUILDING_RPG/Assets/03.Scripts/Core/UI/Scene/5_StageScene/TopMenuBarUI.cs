using TMPro;
using UnityEngine;

public class TopMenuBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text goldText;
    private PlayerData playerData;
    float lastHp;
    float lastGold;
    public void Start()
    {
        playerData = GameManager.Instance.gameContext.saveData.playerData;
        hpText?.SetText($"{playerData.currentHealth} / {playerData.maxHealth}");
        lastHp = playerData.currentHealth;
        goldText?.SetText($"{playerData.coin}");
        lastGold = playerData.coin;
    }

    public void Update()
    {
        if(lastHp != playerData.currentHealth)
        {
            hpText?.SetText($"{playerData.currentHealth} / {playerData.maxHealth}");
        }
        lastHp = playerData.currentHealth;

        if(lastGold != playerData.coin)
        {
            goldText?.SetText($"{playerData.coin}");
        }
        lastGold = playerData.coin;
    }
}