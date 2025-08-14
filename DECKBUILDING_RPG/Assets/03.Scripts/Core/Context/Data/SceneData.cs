using System.Collections.Generic;
using System;

[Serializable]
public class SceneData
{
    public Dictionary<string, Queue<NPCData>> npcDataQueues = new Dictionary<string, Queue<NPCData>>();
    public Dictionary<string, Queue<TileData>> tileDataQueues = new Dictionary<string, Queue<TileData>>();
}