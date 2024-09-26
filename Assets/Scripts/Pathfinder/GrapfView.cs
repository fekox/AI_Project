using System.Collections.Generic;
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

    public List<GameObject> minesGO = new List<GameObject>();

    [Header("Bloqued nodes on the map")]
    [SerializeField] private int maxBloquedNodes = 5;

    [Header("Cost nodes on the map")]
    [SerializeField] private int maxCostNodes = 5;

    private Node<Vector2> startNode;

    [Header("Reference: MinePrefab")]
    [SerializeField] private GameObject minePrefab;

    [Header("Reference: HomePrefab")]
    [SerializeField] private GameObject homePrefab;

    [Header("Reference: CrossPrefab")]
    [SerializeField] private GameObject crossPrebaf;

    [Header("Reference: CostPrefab")]
    [SerializeField] private GameObject costPrebaf;

    [Header("Refence: VoronoiDiagrma")]
    [SerializeField] private VoronoiDiagram voronoiDiagram;

    private List<Node<Vector2>> mines = new List<Node<Vector2>>();

    private List<Node<Vector2>> bloquedNodes = new List<Node<Vector2>>();
    
    private List<Node<Vector2>> costNodes = new List<Node<Vector2>>();

    [Header("Reference: Corners")]
    [SerializeField] public List<GameObject> corners = new List<GameObject>();

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

        InitCorners();

        mines = CreateMines();
        bloquedNodes = CreateBloquedNodes();
        costNodes = CreateCostNodes();
    }

    private void Update()
    {
        DeleteMine();
    }

    public void InitCorners() 
    {
        corners[0].transform.position = new Vector3(grapf.nodes[0].GetCoordinate().x + 0.01f, grapf.nodes[0].GetCoordinate().y + 0.01f, 0);
        corners[1].transform.position = new Vector3(grapf.nodes[0].GetCoordinate().x + 0.01f, grapf.nodes[grapf.nodes.Count - 1].GetCoordinate().y + 0.01f, 0);
        corners[2].transform.position = new Vector3(grapf.nodes[grapf.nodes.Count -1].GetCoordinate().y + 0.01f, grapf.nodes[grapf.nodes.Count -1].GetCoordinate().y + 0.01f, 0);
        corners[3].transform.position = new Vector3(grapf.nodes[grapf.nodes.Count - 1].GetCoordinate().y + 0.01f, grapf.nodes[0].GetCoordinate().y + 0.01f, 0);
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

    public Node<Vector2> GetNearbyMine()
    {
        Node<Vector2> mine = null;
        
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < maxMines; i++)
        {
            float distanceToNode = Vector2.Distance(startNode.GetCoordinate(), voronoiDiagram.GetPointsToCheck()[i]);

            if (distanceToNode < closestDistance)
            {
                closestDistance = distanceToNode;
                mine = mines[i];
            }
        }

        return mine;
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

    public List<Node<Vector2>> CreateMines()
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
            minesGO.Add(Instantiate(minePrefab, new Vector3(mines[i].GetCoordinate().x, mines[i].GetCoordinate().y, 0), Quaternion.identity));
            gameManager.mines.Add(minesGO[i].GetComponent<Mine>());
        }

        return mines;
    }

    public void DeleteMine() 
    {
        for (int i = 0; i < maxMines; i++)
        {
            GameObject instanceObj;

            if (gameManager.mines[i].GetCurrentGold() <= 0) 
            {
                gameManager.mines.RemoveAt(i);
                mines[i].nodesType = INode.NodesType.Bloqued;
                Destroy(minesGO[i]);
                minesGO.Remove(minesGO[i]);
                instanceObj = Instantiate(crossPrebaf, new Vector3(mines[i].GetCoordinate().x, mines[i].GetCoordinate().y, 0), Quaternion.identity);
                maxMines--;
            }
        }
    }

    public List<Node<Vector2>> CreateBloquedNodes()
    {
        for (int i = 0; i < maxBloquedNodes; i++)
        {
            Node<Vector2> potentialBloquedNode;
            GameObject instanceObj;

            do
            {
                potentialBloquedNode = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

            } while (potentialBloquedNode.GetCoordinate() == startNode.GetCoordinate() ||
                        bloquedNodes.Exists(bloquedNode => bloquedNode.GetCoordinate() == potentialBloquedNode.GetCoordinate()) ||
                        mines.Exists(mine => mine.GetCoordinate() == potentialBloquedNode.GetCoordinate()));

            bloquedNodes.Add(potentialBloquedNode);
            bloquedNodes[i].nodesType = INode.NodesType.Bloqued;
            bloquedNodes[i].SetIsBloqued(true);
            instanceObj = Instantiate(crossPrebaf, new Vector3(bloquedNodes[i].GetCoordinate().x, bloquedNodes[i].GetCoordinate().y, 0), Quaternion.identity);
        }

        return bloquedNodes;
    }

    public List<Node<Vector2>> CreateCostNodes()
    {
        for (int i = 0; i < maxCostNodes; i++)
        {
            Node<Vector2> potentialCostNode;
            GameObject instanceObj;

            do
            {
                potentialCostNode = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

            } while (potentialCostNode.GetCoordinate() == startNode.GetCoordinate() ||
                        costNodes.Exists(costNodes => costNodes.GetCoordinate() == potentialCostNode.GetCoordinate()) || 
                        mines.Exists(mine => mine.GetCoordinate() == potentialCostNode.GetCoordinate()) ||
                        bloquedNodes.Exists(bloquedNode => bloquedNode.GetCoordinate() == potentialCostNode.GetCoordinate()));

            costNodes.Add(potentialCostNode);
            costNodes[i].nodesType = INode.NodesType.Cost;
            instanceObj = Instantiate(costPrebaf, new Vector3(costNodes[i].GetCoordinate().x, costNodes[i].GetCoordinate().y, 0), Quaternion.identity);
        }

        return costNodes;
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

                case INode.NodesType.Cost:

                    Gizmos.color = Color.black;

                    break;

                case INode.NodesType.End:

                    Gizmos.color = Color.magenta;

                    break;
            }

            Gizmos.DrawSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), nodesRadius);
        }
    }
}
