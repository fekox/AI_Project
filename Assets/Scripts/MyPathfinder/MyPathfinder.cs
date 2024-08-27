using System.Collections.Generic;

public abstract class MyPathfinder
{
    public List<MyNode> FindPath(MyNode startNode, MyNode destinationNode, ICollection<MyNode> graph)
    {
        Dictionary<MyNode, (MyNode Parent, int AcumulativeCost, int Heuristic)> nodes =
            new Dictionary<MyNode, (MyNode Parent, int AcumulativeCost, int Heuristic)>();

        foreach (MyNode node in graph)
        {
            nodes.Add(node, (default, 0, 0));
        }

        List<MyNode> openList = new List<MyNode>();
        List<MyNode> closedList = new List<MyNode>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            MyNode currentNode = openList[0];
            int currentIndex = 0;

            for (int i = 1; i < openList.Count; i++)
            {
                if (nodes[openList[i]].AcumulativeCost + nodes[openList[i]].Heuristic <
                nodes[currentNode].AcumulativeCost + nodes[currentNode].Heuristic)
                {
                    currentNode = openList[i];
                    currentIndex = i;
                }
            }

            openList.RemoveAt(currentIndex);
            closedList.Add(currentNode);

            if (NodesEquals(currentNode, destinationNode))
            {
                return GeneratePath(startNode, destinationNode);
            }

            foreach (MyNode neighbor in GetNeighbors(currentNode))
            {
                if (!nodes.ContainsKey(neighbor) ||
                IsBloqued(neighbor) ||
                closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeNewAcumulatedCost = 0;
                tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, neighbor);

                if (!openList.Contains(neighbor) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                {
                    nodes[neighbor] = (currentNode, tentativeNewAcumulatedCost, GetDistance(neighbor, destinationNode));

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;

        List<MyNode> GeneratePath(MyNode startNode, MyNode goalNode)
        {
            List<MyNode> path = new List<MyNode>();
            MyNode currentNode = goalNode;

            while (!NodesEquals(currentNode, startNode))
            {
                path.Add(currentNode);
                currentNode = nodes[currentNode].Parent;
            }

            path.Reverse();
            return path;
        }
    }

    protected abstract ICollection<MyNode> GetNeighbors(MyNode node);

    protected abstract int GetDistance(MyNode nodeA, MyNode nodeB);

    protected abstract bool NodesEquals(MyNode nodeA, MyNode nodeB);

    protected abstract int MoveToNeighborCost(MyNode nodeA, MyNode nodeb);

    protected abstract bool IsBloqued(MyNode node);
}
