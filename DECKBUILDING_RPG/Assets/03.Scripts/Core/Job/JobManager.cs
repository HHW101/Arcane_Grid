using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JobManagerParam
{
    public string jobSODir;
}

public class JobManager : MonoBehaviour
{
    private JobManagerParam jobManagerParam = new();
    private List<JobSO> jobSOs = new();
    private GameContext gameContext;

    public void Init(GameContext gameContext, JobManagerParam jobManagerParam)
    {
        this.gameContext = gameContext;
        this.jobManagerParam = jobManagerParam;
        LoadJobSOList();
    }

    public List<JobSO> GetJobSOList()
    {
        return jobSOs;
    }

    #region Private
    private void LoadJobSOList()
    {
        JobSO[] jobSOs = Resources.LoadAll<JobSO>(jobManagerParam.jobSODir);

        if (jobSOs == null || jobSOs.Length == 0)
        {
            Logger.LogWarning($"[JobManager] No JobSO found in {jobManagerParam.jobSODir}");
            return;
        }
        this.jobSOs = new List<JobSO>(jobSOs);
    }
    #endregion
}