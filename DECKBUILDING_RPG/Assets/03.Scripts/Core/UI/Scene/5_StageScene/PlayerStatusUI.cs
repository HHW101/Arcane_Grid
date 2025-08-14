using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField]
    public Image playerActionPointImage;
    [SerializeField]
    public Image playerManaPointImage;
    [SerializeField]
    public TMP_Text actionPointText;
    [SerializeField]
    public TMP_Text manaPointText;

    private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        playerData = GameManager.Instance.gameContext.saveData.playerData;
    }

    // Update is called once per frame
    void Update()
    {
        playerActionPointImage.fillAmount = (float)playerData.actionPoint / playerData.maxActionPoint;
        actionPointText.SetText($"{playerData.actionPoint}/{playerData.maxActionPoint}");
        playerManaPointImage.fillAmount = (float)playerData.technicalPoint / playerData.maxTechnicalPoint;
        manaPointText.SetText($"{playerData.technicalPoint}/{playerData.maxTechnicalPoint}");
    }
}
