using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class StageData
{
    public int floor = 0;
    public List<int> nextStageIndexList = new List<int>();
    public StageEnum.StageType stageType = StageEnum.StageType.Default;
    public int stageProgress = -1;

    public Dictionary<string, Dictionary<HexDirection, string>> tileConnections = new();

    public Dictionary<string, NPCData> npcs = new();
    public Queue<NPCData> npcDataQueue = new Queue<NPCData>();

    public Dictionary<string, TileData> tiles = new();
    public Queue<TileData> tileDataQueue = new Queue<TileData>();

    public PlayerStateInStage playerStateInStage = new PlayerStateInStage();

    public TurnEnum.TurnPhase turnPhase = TurnEnum.TurnPhase.Player;
    public int curTurn = 1;

    public ClearEnum.ClearCondition clearCondition = ClearEnum.ClearCondition.KillAllEnemy;
    public FailEnum.FailCondition failCondition = FailEnum.FailCondition.None;
    public int surviveTurn = 0;
    public int overTurn = 0;

    public int dealDamage = 0;
    public int earnGold = 0;

    public bool isClear = false;
    public bool isFail = false;
    public bool failCampaignIfFailThisStage = true;
    public bool clearCampaignIfClearThisStage = false;
}

[Serializable]
public struct HexCoord
{
    public int q;
    public int r;

    public static HexCoord operator +(HexCoord a, HexCoord b) => new HexCoord(a.q + b.q, a.r + b.r);
    public static HexCoord operator -(HexCoord a, HexCoord b) => new HexCoord(a.q - b.q, a.r - b.r);
    public static bool operator ==(HexCoord a, HexCoord b) => a.q == b.q && a.r == b.r;
    public static bool operator !=(HexCoord a, HexCoord b) => !(a == b);
    
    public HexCoord(int q, int r)
    {
        this.q = q;
        this.r = r;
    }

    public override string ToString()
    {
        return $"{q},{r}"; // 키로 쓸 문자열
    }

    public static HexCoord FromString(string key)
    {
        var parts = key.Split(',');
        return new HexCoord(int.Parse(parts[0]), int.Parse(parts[1]));
    }


    public static Vector3 HexToWorld(HexCoord coord, float tileSize = 1f)
    {
        float x = tileSize * 1.5f * coord.q;
        float y = tileSize * Mathf.Sqrt(3f) * (coord.r + coord.q * 0.5f);
        return new Vector3(x, y, 0f);
    }
    
    public static HexCoord WorldToHex(Vector3 worldPosition, float tileSize = 1f)
    {
        float q_float = (worldPosition.x * 2f / 3f) / tileSize;
        float r_float = (-worldPosition.x / 3f + worldPosition.y * Mathf.Sqrt(3f) / 3f) / tileSize;

        int q = Mathf.RoundToInt(q_float);
        int r = Mathf.RoundToInt(r_float);
        int s = Mathf.RoundToInt(-q_float - r_float); 

        float q_diff = Mathf.Abs(q - q_float);
        float r_diff = Mathf.Abs(r - r_float);
        float s_diff = Mathf.Abs(s - (-q_float - r_float));
        
        if (q_diff > r_diff && q_diff > s_diff)
        {
            q = -r - s;
        }
        else if (r_diff > s_diff)
        {
            r = -q - s;
        }
        else
        {
            s = -q - r; 
        }

        return new HexCoord(q, r);
    }

    public int Distance(HexCoord other) //두 좌표간의 최단 거리(이동칸 수) 계산하는 함수
    {
        int distanceQ = this.q - other.q;
        int distanceR = this.r - other.r;
        int distance = -distanceQ - distanceR;
        return Mathf.Max(Mathf.Abs(distanceQ), Mathf.Abs(distanceR), Mathf.Abs(distance));
    }

    public override int GetHashCode() => (q, r).GetHashCode();
    public override bool Equals(object obj) =>
        obj is HexCoord other && q == other.q && r == other.r;

    public static int Distance(HexCoord a, HexCoord b)
    {
        int dq = a.q - b.q;
        int dr = a.r - b.r;
        int ds = -(a.q + a.r) + (b.q + b.r);
        return (Mathf.Abs(dq) + Mathf.Abs(dr) + Mathf.Abs(ds)) / 2;
    }

    public List<HexCoord> GetNeighbors()
    {
        HexCoord[] directions = new HexCoord[]
        {
            new HexCoord(+1,  0), new HexCoord(+1, -1), new HexCoord(0, -1),
            new HexCoord(-1,  0), new HexCoord(-1, +1), new HexCoord(0, +1)
        };

        List<HexCoord> result = new List<HexCoord>();
        foreach (var dir in directions)
        {
            result.Add(new HexCoord(q + dir.q, r + dir.r));
        }
        return result;
    }
    
    public static readonly HexCoord[] Directions = new HexCoord[]
    {
        new HexCoord(+1,  0), 
        new HexCoord(+1, -1), 
        new HexCoord( 0, -1), 
        new HexCoord(-1,  0), 
        new HexCoord(-1, +1), 
        new HexCoord( 0, +1), 
    };

    public static HexCoord GetDirection(HexCoord from, HexCoord to)
    {
        HexCoord diff = to - from;
        int minDist = int.MaxValue;
        HexCoord bestDir = Directions[0];
        foreach (var dir in Directions)
        {
            int dist = Mathf.Abs(diff.q - dir.q) + Mathf.Abs(diff.r - dir.r);
            if (dist < minDist)
            {
                minDist = dist;
                bestDir = dir;
            }
        }
        return bestDir;
    }

    public static float GetAngleFromDirection(HexCoord dir)
    {
        if (dir == new HexCoord(0, +1)) return 0f;
        if (dir == new HexCoord(+1, 0)) return 60f;
        if (dir == new HexCoord(+1, -1)) return 120f;
        if (dir == new HexCoord(0, -1)) return 180f;
        if (dir == new HexCoord(-1, 0)) return 240f;
        if (dir == new HexCoord(-1, +1)) return 300f;

        return 0f;
    }

}