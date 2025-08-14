using StageEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageStoryBookSO", menuName = "Custom/StageStoryBook")]
public class StageStoryBookSO : ScriptableObject
{
    public List<StageStoryEntry> announceStringListList;
}

[Serializable]
public class StageStoryEntry
{
    public StageType stageType;
    public StageStorySO stagStageStorySO;
}