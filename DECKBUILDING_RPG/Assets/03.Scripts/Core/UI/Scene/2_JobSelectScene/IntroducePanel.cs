using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroducePanel : MonoBehaviour
{
    [SerializeField]
    private Image background;
    [SerializeField]
    private TMP_Text jobName;
    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private TMP_Text hp;
    [SerializeField]
    private TMP_Text gold;
    [SerializeField]
    private TMP_Text attack;
    [SerializeField]
    private TMP_Text actionPoint;
    [SerializeField]
    private TMP_Text technicalPoint;
    // Start is called before the first frame update
    public void Init(JobSO jobSO)
    {
        background.sprite = jobSO.jobSprite;
        jobName?.SetText(jobSO.jobName);
        description?.SetText(jobSO.jobDescription);
        hp?.SetText($"체력 : {jobSO.jobDefaultPlayerData.currentHealth} / {jobSO.jobDefaultPlayerData.maxHealth}");
        gold?.SetText($"골드 : {jobSO.jobDefaultPlayerData.coin}");
        attack?.SetText($"공격력 : {jobSO.jobDefaultPlayerData.defaultAttack}");
        actionPoint?.SetText($"AP : {jobSO.jobDefaultPlayerData.actionPoint}");
        technicalPoint?.SetText($"TP : {jobSO.jobDefaultPlayerData.technicalPoint}");
    }
}
