using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JobSelectSceneUI : MonoBehaviour
{
    [Header("Prefab & Parent")]
    [SerializeField] private GameObject jobSlotPrefab;
    [SerializeField] private Transform jobSlotParent;

    [Header("Text UI")]
    [SerializeField] private IntroducePanel introducePanel;

    private List<JobSlot> jobSlotList = new List<JobSlot>();

    private void Start()
    {
        List<JobSO> jobList = GameManager.Instance.jobManager.GetJobSOList();

        foreach (var job in jobList)
        {
            GameObject slotObj = Instantiate(jobSlotPrefab, jobSlotParent);
            if (slotObj.TryGetComponent<JobSlot>(out JobSlot jobSlot))
            {
                jobSlot.Init(job, OnJobSlotButtonClick);
                jobSlotList.Add(jobSlot);
            }
        }

        if (jobSlotList.Count > 0)
        {
            jobSlotList[0].Click();
        }
    }

    private void OnJobSlotButtonClick(JobSO job)
    {
        introducePanel.Init(job);
        GameManager.Instance.gameContext.saveData.playerData = job.jobDefaultPlayerData.ClonePlayerData();
    }
}
