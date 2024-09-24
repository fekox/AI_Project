using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static TreeEditor.TreeEditorHelper;

public enum PathfinderType
{
    AStar,
    Dijkstra,
    Depth,
    Breath
};

public class GrapfView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Pathfinder Type")]
    [SerializeField] private PathfinderType pathfinderType;

    public Vector2IntGrapf<Node<Vector2>> grapf;

    [Header("Grapf Size")]
    [SerializeField] private Vector2Int grafpSize;

    [Header("Distance Between Nodes")]
    [SerializeField] private int nodesDistance;

    [Header("Mines on the map")]
    [SerializeField] private int maxMines = 5;

    private Node<Vector2> startNode;

    [Header("Reference: MinePrefab")]
    [SerializeField] private GameObject minePrefab;

    [Header("Reference: HomePrefab")]
    [SerializeField] private GameObject homePrefab;

    private List<Node<Vector2>> mines = new List<Node<Vector2>>();

    private void OnEnable()
    {
        grapf = new Vector2IntGrapf<Node<Vector2>>(grafpSize.x, grafpSize.y, nodesDistance, pathfinderType);

        for (int i = 0; i < grapf.nodes.Count; i++)
        {
            grapf.nodes[i].nodesType = INode.NodesType.Walkable;
        }

        startNode = grapf.nodes[(Random.Range(0, grapf.nodes.Count))];
        startNode.nodesType = INode.NodesType.Start;
        Instantiate(homePrefab, new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y, 0), Quaternion.identity);

        mines = CreateMines(maxMines);
    }

    public PathfinderType GetPathfinderType()
    {
        return pathfinderType;
    }

    public Node<Vector2> GetStartNode()
    {
        return startNode;
    }

    public List<Node<Vector2>> GetMines() 
    {
        return mines;
    }

    public Node<Vector2> GetOneMine(int mineID)
    {
        return mines[mineID];
    }

    public Node<Vector2> GetCurrentNode(Vector3 targetPos) 
    {
        Node<Vector2> currentNode = grapf.nodes[0];

        foreach (Node<Vector2> node in grapf.nodes)
        {
            if ((int)node.GetCoordinate().x == (int)targetPos.x)
            {
                if ((int)node.GetCoordinate().y == (int)targetPos.y)
                {
                    return currentNode = node;
                }
            }
        }

        return currentNode;
    }

    public List<Node<Vector2>> CreateMines(int maxMines)
    {
        for (int i = 0; i < maxMines; i++)
        {
            Node<Vector2> potentialMine;
            GameObject instanceObj;

            do
            {
                potentialMine = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

            } while (potentialMine.GetCoordinate() == startNode.GetCoordinate() ||
                     mines.Exists(mine => mine.GetCoordinate() == potentialMine.GetCoordinate()));

            mines.Add(potentialMine);
            mines[i].nodesType = INode.NodesType.Mine;
            instanceObj = Instantiate(minePrefab, new Vector3(mines[i].GetCoordinate().x, mines[i].GetCoordinate().y, 0), Quaternion.identity);
            gameManager.mines.Add(instanceObj.GetComponent<Mine>());
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

        foreach (Node<Vector2> node in grapf.nodes)
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
