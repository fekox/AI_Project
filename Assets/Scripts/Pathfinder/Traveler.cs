using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private List<Node<Vector2>> path = new List<Node<Vector2>>();

    void Start()
    {
        Pathfinder<Node<Vector2>> pathfinder = grapfView.GetPathfinderType() switch
        {
            PathfinderType.AStar => new AStarPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Dijkstra => new DijstraPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Breath => new BreadthPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Depth => new DepthFirstPathfinder<Node<Vector2>, Vector2>(),

            _ => new AStarPathfinder<Node<Vector2>, Vector2>()
        };

        path = pathfinder.FindPath(grapfView.GetStartNode(), grapfView.GetFinalNode(), grapfView.grapf.nodes);
        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2>> path) 
    {
        foreach (Node<Vector2> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
