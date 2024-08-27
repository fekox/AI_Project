using System;
using System.Collections.Generic;

public class MyAStarPathfinder : MyPathfinder
{
    protected override int GetDistance(MyNode nodeA, MyNode nodeB)
    {
        return (int)Math.Sqrt(Math.Pow(nodeB.GetCoordinate().x - nodeA.GetCoordinate().x, 2) + Math.Pow(nodeB.GetCoordinate().y - nodeA.GetCoordinate().y, 2));
    }

    protected override ICollection<MyNode> GetNeighbors(MyNode node)
    {
        throw new NotImplementedException();
    }

    protected override bool IsBloqued(MyNode node)
    {
        return node.IsBloqued();
    }

    protected override int MoveToNeighborCost(MyNode nodeA, MyNode nodeb)
    {
        throw new NotImplementedException();
    }

    protected override bool NodesEquals(MyNode nodeA, MyNode nodeB)
    {
        return nodeA.EqualsTo(nodeB);
    }
}