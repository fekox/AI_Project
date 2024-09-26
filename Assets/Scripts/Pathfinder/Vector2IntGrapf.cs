using System;
using System.Collections.Generic;

public class Vector2IntGrapf<NodeType> 
    where NodeType : INode<UnityEngine.Vector2>, INode, new()
{
    private PathfinderType pathfinderType;

    public List<NodeType> nodes = new List<NodeType>();

    private UnityEngine.Vector2 nodesCordinates;

    public Vector2IntGrapf(int x, int y, int distanceBetweenNodes, PathfinderType pathfinderType) 
    {
        this.pathfinderType = pathfinderType;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new UnityEngine.Vector2(nodesCordinates.x + 0.01f, nodesCordinates.y + 0.01f));
                nodes.Add(node);
                nodesCordinates.y += distanceBetweenNodes;
            }
            nodesCordinates.y = 0;
            nodesCordinates.x += distanceBetweenNodes;
        }

        foreach (NodeType node in nodes) 
        {
            AddNodeNeighbors(node, distanceBetweenNodes);
        }
    }

    private void AddNodeNeighbors(NodeType currentNode, int distanceBetweenNodes)
    {
        foreach (NodeType neighbor in nodes)
        {
            if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
                Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == distanceBetweenNodes)
                currentNode.AddNeighbor(neighbor, 0);

            else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == distanceBetweenNodes)
                currentNode.AddNeighbor(neighbor, 0);

            if (pathfinderType == PathfinderType.Dijkstra || pathfinderType == PathfinderType.AStar)
            {
                if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == distanceBetweenNodes && Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == distanceBetweenNodes)
                    currentNode.AddNeighbor(neighbor, 0);
            }
        }
    }

}
