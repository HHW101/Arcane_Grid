using StageEnum;
using System.Collections.Generic;
using UnityEngine;

public class StageStoryBook
{
    private readonly Dictionary<StageType, StageStorySO> announceMap = new();

    public StageStoryBook(StageStoryBookSO bookSO)
    {
        foreach (var entry in bookSO.announceStringListList)
        {
            if (!announceMap.ContainsKey(entry.stageType))
            {
                announceMap.Add(entry.stageType, entry.stagStageStorySO);
            }
            else
            {
                Debug.LogWarning($"[StageStoryBook] Duplicate entry for StageType {entry.stageType} ignored.");
            }
        }
    }

    public bool TryGet(StageType stageType, out StageStorySO announceSO)
    {
        return announceMap.TryGetValue(stageType, out announceSO);
    }

    public StageStorySO Get(StageType stageType)
    {
        announceMap.TryGetValue(stageType, out var result);
        return result;
    }
}
