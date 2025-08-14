using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AStarPathFinder
{
    private class PathNode 
    {
        public HexCoord Coordinate { get; }
        public PathNode Parent { get; }
        public float GCost { get; }
        public float HCost { get; }
        public float FCost => GCost + HCost;

        public PathNode(HexCoord coordinate, PathNode parent, float gCost, float hCost)
        {
            Coordinate = coordinate;
            Parent = parent;
            GCost = gCost;
            HCost = hCost;
        }
    }
    public List<HexCoord> FindPathWithWeightedCost(StageData stageData, HexCoord startCoord, HexCoord endCoord, Func<HexCoord, HexCoord, float> movementCostCalculator)
    {
        List<PathNode> openList = new List<PathNode>();
        HashSet<HexCoord> closedList = new HashSet<HexCoord>();
        Dictionary<HexCoord, PathNode> pathNodes = new Dictionary<HexCoord, PathNode>();

        PathNode startNode = new PathNode(startCoord, null, 0, CalculateHeuristic(startCoord, endCoord));
        openList.Add(startNode);
        pathNodes[startCoord] = startNode;

        while (openList.Count > 0)
        {
            PathNode currentNode = openList.OrderBy(n => n.FCost).First();

            if (currentNode.Coordinate.Equals(endCoord))
            {
                return ReconstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode.Coordinate);

            string currentCoordKey = currentNode.Coordinate.ToString();

            if (stageData.tileConnections.TryGetValue(currentCoordKey, out var connections))
            {
                foreach (var kvp in connections)
                {
                    HexCoord neighborCoord = HexCoord.FromString(kvp.Value);
                    string neighborCoordKey = kvp.Value;

                    if (closedList.Contains(neighborCoord) || !stageData.tiles.ContainsKey(neighborCoordKey))
                    {
                        continue;
                    }
                    
                    float movementCost = movementCostCalculator(currentNode.Coordinate, neighborCoord);
                    
                    if (movementCost >= float.PositiveInfinity)
                    {
                        continue;
                    }

                    float newGCost = currentNode.GCost + movementCost;

                    PathNode neighborNode;
                    pathNodes.TryGetValue(neighborCoord, out neighborNode);

                    if (neighborNode == null || newGCost < neighborNode.GCost)
                    {
                        float hCost = CalculateHeuristic(neighborCoord, endCoord);
                        neighborNode = new PathNode(neighborCoord, currentNode, newGCost, hCost);
                        pathNodes[neighborCoord] = neighborNode;

                        if (!openList.Contains(neighborNode))
                        {
                            openList.Add(neighborNode);
                        }
                    }
                }
            }
        }
        return null;
    }

    private float CalculateHeuristic(HexCoord a, HexCoord b)
    {
        int ax = a.q;
        int az = a.r;
        int ay = -ax - az;

        int bx = b.q;
        int bz = b.r;
        int by = -bx - bz;

        return (Mathf.Abs(ax - bx) + Mathf.Abs(ay - by) + Mathf.Abs(az - bz)) / 2.0f;
    }

    private List<HexCoord> ReconstructPath(PathNode endNode)
    {
        List<HexCoord> path = new List<HexCoord>();
        PathNode currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode.Coordinate); 
            currentNode = currentNode.Parent; 
        }
        path.Reverse(); 
        return path;
    }
}