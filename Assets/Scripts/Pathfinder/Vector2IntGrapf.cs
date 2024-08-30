using System;
using System.Collections.Generic;

public class Vector2IntGrapf<NodeType> 
    where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
{
    private PathfinderType pathfinderType;

    public List<NodeType> nodes = new List<NodeType>();

    public Vector2IntGrapf(int x, int y, PathfinderType pathfinderType) 
    {
        this.pathfinderType = pathfinderType;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new UnityEngine.Vector2Int(i, j));
                nodes.Add(node);
            }
        }

        foreach (NodeType node in nodes) 
        {
            AddNodeNeighbors(node);
        }
    }

    private void AddNodeNeighbors(NodeType currentNode)
    {
        foreach (NodeType neighbor in nodes)
        {
            if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
                Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1)
                currentNode.AddNeighbor(neighbor, 0);

            else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                currentNode.AddNeighbor(neighbor, 0);

            if (pathfinderType == PathfinderType.Dijkstra || pathfinderType == PathfinderType.AStar)
            {
                if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
                    currentNode.AddNeighbor(neighbor, 0);
            }
        }
    }

}
