using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum PathfinderType
{
    AStar,
    Dijkstra,
    Depth,
    Breath
};

public class GrapfView : MonoBehaviour
{
    [Header("Pathfinder Type")]
    [SerializeField] private PathfinderType pathfinderType;

    public Vector2IntGrapf<Node<Vector2Int>> grapf;

    [Header("Grapf Size")]
    [SerializeField] private Vector2Int grafpSize;

    [Header("Distance Between Nodes")]
    [SerializeField] private int nodesDistance;

    [Header("Mines on the map")]
    [SerializeField] private int maxMines = 5;

    private Node<Vector2Int> startNode;
    private Node<Vector2Int> finalNode;

    private List<Node<Vector2Int>> mines = new List<Node<Vector2Int>>();


    void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(grafpSize.x, grafpSize.y, nodesDistance, pathfinderType);

        for (int i = 0; i < grapf.nodes.Count; i++)
        {
            grapf.nodes[i].nodesType = INode.NodesType.Walkable;
        }

        startNode = grapf.nodes[(Random.Range(0, grapf.nodes.Count))];
        startNode.nodesType = INode.NodesType.Start;

        finalNode = grapf.nodes[(Random.Range(0, grapf.nodes.Count))];
        finalNode.nodesType = INode.NodesType.End;

        mines = CreateMines(maxMines);
    }

    public PathfinderType GetPathfinderType()
    {
        return pathfinderType;
    }

    public Node<Vector2Int> GetStartNode()
    {
        return startNode;
    }

    public Node<Vector2Int> GetFinalNode()
    {
        return finalNode;
    }

    public List<Node<Vector2Int>> CreateMines(int maxMines)
    {
        for (int i = 0; i < maxMines; i++)
        {
            Node<Vector2Int> potentialMine;

            do
            {
                potentialMine = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

            } while (potentialMine.GetCoordinate() == startNode.GetCoordinate() ||
                     potentialMine.GetCoordinate() == finalNode.GetCoordinate() ||
                     mines.Exists(mine => mine.GetCoordinate() == potentialMine.GetCoordinate()));

            mines.Add(potentialMine);
            mines[i].nodesType = INode.NodesType.Mine;
        }

        return mines;
    }

    private void OnDrawGizmos()
    {
        float nodesRadius = 1;

        if (nodesDistance >= 5)
        {
            nodesRadius = 0.6f;
        }

        else
        {
            nodesRadius = 0.1f;
        }

        if (!Application.isPlaying)
            return;

        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            switch (node.nodesType)
            {
                case INode.NodesType.Start:

                    Gizmos.color = Color.blue;

                    break;

                case INode.NodesType.Walkable:

                    Gizmos.color = Color.green;

                    break;

                case INode.NodesType.Bloqued:
                    
                    Gizmos.color = Color.red;

                    break;

                case INode.NodesType.Mine:

                    Gizmos.color = Color.yellow;

                    break;

                case INode.NodesType.End:

                    Gizmos.color = Color.magenta;

                    break;
            }

            Gizmos.DrawSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), nodesRadius);
        }
    }
}
