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

    private Node<Vector2Int> startNode;
    private Node<Vector2Int> finalNode;


    void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(grafpSize.x, grafpSize.y, pathfinderType);

        startNode = grapf.nodes[(Random.Range(0, grapf.nodes.Count))];

        finalNode = grapf.nodes[(Random.Range(0, grapf.nodes.Count))];
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

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapf.nodes)
        {
            if (node.GetCoordinate() == startNode.GetCoordinate())
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y), 0.1f);
            }

            if (node.GetCoordinate() == finalNode.GetCoordinate())
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(new Vector3(finalNode.GetCoordinate().x, finalNode.GetCoordinate().y), 0.1f);
            }

            if (node.IsBloqued())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(new Vector3(startNode.GetCoordinate().x, startNode.GetCoordinate().y), 0.1f);
            }

            if (node.GetCoordinate() != startNode.GetCoordinate() && node.GetCoordinate() != finalNode.GetCoordinate())
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
            }
        }
    }
}
