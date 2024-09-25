﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class DijstraPathfinder<NodeType, Coorninate> : Pathfinder<NodeType> where NodeType : INode<Coorninate> where Coorninate : IEquatable<Coorninate>
{
    protected override int Distance(NodeType A, NodeType B)
    {
        var nodeA = A as Node<Vector2Int>;
        var nodeB = B as Node<Vector2Int>;

        return (int)Math.Sqrt(Math.Pow(nodeB.GetCoordinate().x - nodeA.GetCoordinate().x, 2) + Math.Pow(nodeB.GetCoordinate().y - nodeA.GetCoordinate().y, 2));
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        ICollection<NodeType> neighbors = new List<NodeType>();

        foreach (NodeType Neightbors in node.GetNeighbords())
        {
            neighbors.Add(Neightbors);
        }

        return neighbors;
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b, Agent agent)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return Equals(A, B);
    }

    protected override void SetBloqued(NodeType node, bool value)
    {
        node.SetIsBloqued(value);
    }
}
