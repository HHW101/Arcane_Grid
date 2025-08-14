using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StageStorySO", menuName = "Custom/StageStory")]
public class StageStorySO : ScriptableObject
{
    public List<StageMonoPhaseStory> phaseStoryList;
    public List<StageMonoPhaseStory> clearStoryList;
    public List<StageMonoPhaseStory> failStoryList;
    // phase는 스테이지 내의 메인 퀘스트 흐름에 따라 분류되거나 서브 퀘스트의 단계 구별
}

[Serializable]
public class StageMonoPhaseStory
{
    public List<NarrativeResourceSO> narrativeResourceSO;
    public AnnounceResourceSO announceResourceSO;
    // 인스펙터로 작성하니까 꽤 불편해서 나중에 json으로 바꿔야 함
}