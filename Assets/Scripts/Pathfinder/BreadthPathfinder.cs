using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreadthPathfinder<NodeType, Coorninate> : Pathfinder<NodeType> where NodeType : INode<Coorninate> where Coorninate : IEquatable<Coorninate>
{
    protected override int Distance(NodeType A, NodeType B)
    {
        return 0;
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        ICollection<NodeType> neighbors = new List<NodeType>();

        foreach (NodeType Neightbors in node.GetNeighbords())
        {
            neighbors.Add(Neightbors);
        }

        neighbors.Reverse();
        return neighbors;
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return Equals(A, B);
    }
}
