using System;
using System.Collections.Generic;
using UnityEngine;

public class CampaignDataGenerator
{
	private StageDataGenerator stageDataGenerator;

	public CampaignDataGenerator(TileMapSO tileMapSO)
	{
		this.stageDataGenerator = new StageDataGenerator(tileMapSO);
	}

	public CampaignData GenerateNewCampaignData(CampaignPreviewSO campaignPreview)

    {
		CampaignData campaignData = new CampaignData();
		List<List<int>> floorIndexList = new();

		int floorCount = campaignPreview.floorStageTableList.Count;

		for (int floor = 0; floor < floorCount; floor++)
		{
			FloorStageTable floorTable = campaignPreview.floorStageTableList[floor];
			int stageCount = campaignPreview.floorStageTableList[floor].floorStageCount;
            List<int> currentFloor = new();

			for (int i = 0; i < stageCount; i++)
			{
				StagePreviewSO stagePreview = PickRandomStagePreview(floorTable.weightedStageEntryList);
				if (stagePreview == null)
				{
					Debug.LogWarning($"[CampaignDataGenerator] floor {floor} has no valid StagePreviewSO.");
					continue;
				}

				StageData stageData = stageDataGenerator.GenerateStageData(stagePreview);
				stageData.floor = floor;
				campaignData.stageDataList.Add(stageData);
				currentFloor.Add(campaignData.stageDataList.Count - 1);
			}

			floorIndexList.Add(currentFloor);
		}

		ConnectFloors(campaignData, floorIndexList);

		// 마지막 스테이지 캠페인 종료 플래그
		int lastStageIndex = floorIndexList[^1][0];
		campaignData.stageDataList[lastStageIndex].clearCampaignIfClearThisStage = true;

		return campaignData;
	}

	private void ConnectFloors(CampaignData campaignData, List<List<int>> floorIndexList)
	{
		int floorCount = floorIndexList.Count;

		for (int floor = 0; floor < floorCount - 1; floor++)
		{
			List<int> current = floorIndexList[floor];
			List<int> next = floorIndexList[floor + 1];

			Dictionary<int, List<int>> reverseLinks = new();

			foreach (int curIdx in current)
			{
				List<int> shuffled = new(next);
				Shuffle(shuffled);

				HashSet<int> links = new();
				int linkCount = UnityEngine.Random.Range(1, next.Count + 1);

				for (int i = 0; i < linkCount; i++)
				{
					links.Add(shuffled[i]);
				}

				campaignData.stageDataList[curIdx].nextStageIndexList = new List<int>(links);

				foreach (int nextIdx in links)
				{
					if (!reverseLinks.ContainsKey(nextIdx))
						reverseLinks[nextIdx] = new List<int>();

					reverseLinks[nextIdx].Add(curIdx);
				}
			}

			foreach (int nextIdx in next)
			{
				if (!reverseLinks.ContainsKey(nextIdx))
				{
					int randomCurrent = current[UnityEngine.Random.Range(0, current.Count)];
					campaignData.stageDataList[randomCurrent].nextStageIndexList.Add(nextIdx);
				}
			}
		}
	}

	private StagePreviewSO PickRandomStagePreview(List<WeightedStageEntry> entries)
	{
		if (entries == null || entries.Count == 0)
			return null;

		int totalWeight = 0;
		foreach (var entry in entries)
		{
			if (entry.stagePreviewSO != null)
				totalWeight += entry.weight;
		}

		int roll = UnityEngine.Random.Range(0, totalWeight);
		int acc = 0;

		foreach (var entry in entries)
		{
			if (entry.stagePreviewSO == null) continue;
			acc += entry.weight;
			if (roll < acc)
				return entry.stagePreviewSO;
		}

		return entries[^1].stagePreviewSO;
	}

	private void Shuffle<T>(List<T> list)
	{
		for (int i = list.Count - 1; i > 0; i--)
		{
			int j = UnityEngine.Random.Range(0, i + 1);
			(list[i], list[j]) = (list[j], list[i]);
		}
	}
}
