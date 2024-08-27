using System;
using System.Collections.Generic;

public class AStarPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode
{
    protected override int GetDistance(NodeType A, NodeType B)
    {
        throw new System.NotImplementedException();
        //return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2))
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node)
    {
        throw new System.NotImplementedException();
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(NodeType nodeA, NodeType nodeB)
    {
        throw new System.NotImplementedException();
    }

    protected override bool NodesEquals(NodeType nodeA, NodeType nodeB)
    {
        return nodeA.EqualsTo(nodeB);
    }
}
