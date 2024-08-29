using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveler : MonoBehaviour
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

    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private Pathfinder<Node<Vector2Int>> pathfinder;

    void Start()
    {
        switch (pathfinderType)
        {
            case PathfinderType.AStar:

                pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

                break;

            case PathfinderType.Dijstra:

                pathfinder = new DijstraPathfinder<Node<Vector2Int>, Vector2Int>();

                break;

            case PathfinderType.Depth:

                pathfinder = new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>();

                break;

            case PathfinderType.Breadth:

                pathfinder = new BreadthPathfinder<Node<Vector2Int>, Vector2Int>();

                break;
        }

        List<Node<Vector2Int>> path = pathfinder.FindPath(grapfView.GetStartNode(), grapfView.GetFinalNode(), grapfView.grapf.nodes);
        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path) 
    {
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
