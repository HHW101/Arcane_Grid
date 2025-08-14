using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NPCDropCardTable
{
    public List<int> fixedCardIDList = new List<int>();

    public List<WeightedCardEntry> randomCardEntryList = new List<WeightedCardEntry>();

    public int randomSpawnCount = 10;

    public List<int> MakeDropIDList()
    {
        List<int> result = new List<int>();

        result.AddRange(fixedCardIDList);

        for (int i = 0; i < randomSpawnCount; i++)
        {
            int selected = GetRandomCardIDByWeight();
            if (selected != -1)
            {
                result.Add(selected);
            }
        }

        return result;
    }

    private int GetRandomCardIDByWeight()
    {
        if (randomCardEntryList == null || randomCardEntryList.Count == 0)
            return -1;

        int totalWeight = 0;
        foreach (var entry in randomCardEntryList)
        {
            totalWeight += entry.weight;
        }

        int rand = UnityEngine.Random.Range(0, totalWeight);
        int cumulative = 0;

        foreach (var entry in randomCardEntryList)
        {
            cumulative += entry.weight;
            if (rand < cumulative)
            {
                return entry.cardID;
            }
        }

        return -1;
    }
}

[Serializable]
public class WeightedCardEntry
{
    public int cardID;
    public int weight;
}