using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    private enum PathfinderType
    {
        AStar,
        Dijkstra,
        Depth,
        Breath
    };

    [Header("Pathfinder Type")]
    [SerializeField] private PathfinderType pathfinderType;

    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private List<Node<Vector2Int>> path = new List<Node<Vector2Int>>();

    void Start()
    {
        Pathfinder<Node<Vector2Int>> pathfinder = pathfinderType switch
        {
            PathfinderType.AStar => new AStarPathfinder<Node<Vector2Int>, Vector2Int>(),

            PathfinderType.Dijkstra => new DijstraPathfinder<Node<Vector2Int>, Vector2Int>(),

            PathfinderType.Breath => new BreadthPathfinder<Node<Vector2Int>, Vector2Int>(),

            PathfinderType.Depth => new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>(),

            _ => new AStarPathfinder<Node<Vector2Int>, Vector2Int>()
        };

        path = pathfinder.FindPath(grapfView.GetStartNode(), grapfView.GetFinalNode(), grapfView.grapf.nodes);
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
