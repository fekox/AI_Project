using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepthFirstPathfinder<NodeType, Coorninate> : Pathfinder<NodeType> where NodeType : INode<Coorninate> where Coorninate : IEquatable<Coorninate>
{
    //Distancia 0
    //Costo 0
    //GetNeighbors es lo mismo 

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
