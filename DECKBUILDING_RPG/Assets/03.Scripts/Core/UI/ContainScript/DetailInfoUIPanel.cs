using System;
using TMPro;
using UnityEngine;

public class DetailInfoUIPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameText;
    [SerializeField]
    private TMP_Text descriptionText;

    public void Init(DetailInfoData detailInfoData)
    {
        nameText.SetText(detailInfoData.name);
        descriptionText.SetText(detailInfoData.description);
    }
}