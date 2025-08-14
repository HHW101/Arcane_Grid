// StageGraphViewUI.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Image = UnityEngine.UI.Image;

[Serializable]
public class StageIconEntry
{
    public StageEnum.StageType stageType;
    public Sprite stageIcon;
}

public class StageGraphViewUI : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private RectTransform nodeRoot;
    [SerializeField] private GameObject stageNodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private List<StageIconEntry> stageIconEntryList;

    private Dictionary<StageEnum.StageType, Sprite> stageIconMapCache;
    private readonly List<Button> buttonList = new();

    public Dictionary<StageEnum.StageType, Sprite> stageIconMap
    {
        get
        {
            if (stageIconMapCache == null)
            {
                stageIconMapCache = new Dictionary<StageEnum.StageType, Sprite>();
                foreach (var entry in stageIconEntryList)
                {
                    if (!stageIconMapCache.ContainsKey(entry.stageType))
                    {
                        stageIconMapCache.Add(entry.stageType, entry.stageIcon);
                    }
                }
            }
            return stageIconMapCache;
        }
    }

    [Header("Layout")]
    [SerializeField] private float xSpacing = 3f;
    [SerializeField] private float ySpacing = 3f;
    [SerializeField] private float nodeWidth = 1f;
    [SerializeField] private float padding = 4f;

    private CampaignData campaignData;
    private readonly Dictionary<int, RectTransform> nodeDict = new();
    private readonly Dictionary<int, StageNodeUI> nodeUIMap = new();
    private Dictionary<int, List<int>> floorIndexMap = new();
    private int maxFloor = 0;

    public void Start()
    {
        campaignData = GameManager.Instance.gameContext.saveData.campaignData;
        if (campaignData == null || campaignData.stageDataList.Count == 0)
        {
            Debug.LogWarning("[StageGraphViewUI] No campaign data assigned.");
            return;
        }

        BuildGraph();
        ResizeBackground();
        UpdateNodeHighlights();
    }

    private void BuildGraph()
    {
        nodeDict.Clear();
        nodeUIMap.Clear();
        floorIndexMap.Clear();
        buttonList.Clear();
        maxFloor = 0;

        for (int i = 0; i < campaignData.stageDataList.Count; i++)
        {
            int floor = campaignData.stageDataList[i].floor;
            if (!floorIndexMap.ContainsKey(floor))
                floorIndexMap[floor] = new List<int>();
            floorIndexMap[floor].Add(i);
            maxFloor = Mathf.Max(maxFloor, floor);
        }

        float centerFloor = maxFloor / 2f;

        foreach (var kvp in floorIndexMap)
        {
            int floor = kvp.Key;
            List<int> stageIndices = kvp.Value;

            float centerStageIndex = (stageIndices.Count - 1) / 2f;
            float y = -(centerFloor - floor) * ySpacing;

            for (int i = 0; i < stageIndices.Count; i++)
            {
                int stageIndex = stageIndices[i];
                float x = (i - centerStageIndex) * xSpacing;

                GameObject nodeGO = Instantiate(stageNodePrefab, nodeRoot);
                RectTransform rect = nodeGO.GetComponent<RectTransform>();
                rect.localPosition = new Vector3(x, y, 0f);
                rect.sizeDelta = new Vector2(nodeWidth, nodeWidth);

                if (nodeGO.TryGetComponent(out Image image))
                {
                    image.sprite = stageIconMap[campaignData.stageDataList[stageIndex].stageType];
                }

                if (rect.TryGetComponent(out Button btn))
                {
                    buttonList.Add(btn);
                    int capturedIndex = stageIndex;
                    btn.onClick.AddListener(() => OnStageSelected(capturedIndex));
                }

                nodeDict[stageIndex] = rect;

                if (nodeGO.TryGetComponent(out StageNodeUI nodeUI))
                {
                    nodeUIMap[stageIndex] = nodeUI;
                }
            }
        }

        foreach (var kvp in nodeDict)
        {
            int fromIndex = kvp.Key;
            RectTransform fromRect = kvp.Value;
            List<int> nextList = campaignData.stageDataList[fromIndex].nextStageIndexList;

            foreach (int toIndex in nextList)
            {
                if (!nodeDict.TryGetValue(toIndex, out RectTransform toRect)) continue;
                DrawUILine(fromRect, toRect);
            }
        }
    }

    private void DrawUILine(RectTransform from, RectTransform to)
    {
        GameObject lineGO = Instantiate(linePrefab, nodeRoot);
        RectTransform rect = lineGO.GetComponent<RectTransform>();

        Vector2 start = from.localPosition;
        Vector2 end = to.localPosition;
        Vector2 direction = end - start;

        float length = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rect.localPosition = start;
        rect.sizeDelta = new Vector2(length, 0.1f);
        rect.localRotation = Quaternion.Euler(0, 0, angle);
        rect.SetAsFirstSibling();
    }

    private void ResizeBackground()
    {
        if (nodeDict.Count == 0) return;

        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (var rect in nodeDict.Values)
        {
            Vector2 pos = rect.localPosition;
            minX = Mathf.Min(minX, pos.x);
            maxX = Mathf.Max(maxX, pos.x);
            minY = Mathf.Min(minY, pos.y);
            maxY = Mathf.Max(maxY, pos.y);
        }

        float width = (maxX - minX) + padding * 2f;
        float height = (maxY - minY) + padding * 2f;

        nodeRoot.sizeDelta = new Vector2(width, height);
        nodeRoot.anchoredPosition = Vector2.zero;

        if (backgroundImage != null)
        {
            backgroundImage.rectTransform.sizeDelta = nodeRoot.sizeDelta;
            backgroundImage.rectTransform.anchoredPosition = Vector2.zero;
        }
    }

    private void UpdateNodeHighlights()
    {
        int curStageIndex = campaignData.playerStateInCampaign.curStageIndex;

        foreach (var kvp in nodeUIMap)
        {
            kvp.Value.Init(kvp.Key, false, false, null);
        }

        if (curStageIndex == -1)
        {
            if (floorIndexMap.TryGetValue(0, out var zeroFloor))
            {
                foreach (int idx in zeroFloor)
                {
                    nodeUIMap[idx].Init(idx, true, false, OnStageSelected);
                }
            }
        }
        else
        {
            nodeUIMap[curStageIndex].Init(curStageIndex, false, true, null);
            List<int> nextList = campaignData.stageDataList[curStageIndex].nextStageIndexList;
            foreach (int idx in nextList)
            {
                nodeUIMap[idx].Init(idx, true, false, OnStageSelected);
            }
        }
    }

    private void OnStageSelected(int stageIndex)
    {
        campaignData.playerStateInCampaign.nextStageIndex = stageIndex;
        SceneLoader.Instance.LoadSceneWithFade("4_StageReadyScene2");
    }
}
