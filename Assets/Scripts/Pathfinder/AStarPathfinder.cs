using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    protected override int Distance(NodeType A, NodeType B)
    {
        var nodeA = A as Node<Vector2Int>;
        var nodeB = B as Node<Vector2Int>;

        return (int)Math.Sqrt(Math.Pow(nodeB.GetCoordinate().x - nodeA.GetCoordinate().x, 2) + Math.Pow(nodeB.GetCoordinate().y - nodeA.GetCoordinate().y, 2));
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        throw new System.NotImplementedException();
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        var nodeA = A as Node<Vector2Int>;
        var nodeB = B as Node<Vector2Int>;

        return nodeA.GetCoordinate().x == nodeB.GetCoordinate().x && nodeA.GetCoordinate().y == nodeB.GetCoordinate().y;
    }
}
