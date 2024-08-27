using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTraveler : MonoBehaviour
{
    private enum PathfinderType
    {
        AStar,
        Dijstra,
        Depth,
        Breadth
    };

    [Header("Pathfinder Type")]
    [SerializeField] private PathfinderType pathfinderType;

    [Header("Path")]
    [SerializeField] private List<MyNode> path;

    private MyAStarPathfinder aStarPF;
    private MyDijstraPathfinder dijstraPF;
    private MyDepthFirstPathfinder deppthPF;
    private MyBreadthPathfinder breadthPF;

    private MyPathfinder pathfinder;

    private MyNode startNode;
    private MyNode destinationNode;

    [Header("GrapfView")]
    public MyGrapfView grapfView;

    void Start()
    {
        path = new List<MyNode>();

        startNode = new MyNode();
        startNode.SetCoordinate(new Vector2Int(Random.Range(0, 10), Random.Range(0, 10)));

        destinationNode = new MyNode();
        destinationNode.SetCoordinate(new Vector2Int(Random.Range(0, 10), Random.Range(0, 10)));

        switch (pathfinderType)
        {
            case PathfinderType.AStar:

                pathfinder = new MyAStarPathfinder();

            break;

            case PathfinderType.Dijstra:

                pathfinder = new MyDijstraPathfinder();

            break;

            case PathfinderType.Depth:

                pathfinder = new MyBreadthPathfinder();

            break;

            case PathfinderType.Breadth:

                pathfinder = new MyBreadthPathfinder();

            break;
        }

        //path = pathfinder.FindPath(startNode, destinationNode, grapfView.grapf.nodes);
        //StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<MyNode> path)
    {
        foreach (MyNode node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }
}