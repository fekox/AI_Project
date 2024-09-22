using System.Collections.Generic;
using UnityEngine;

public class CaravanFSM : MonoBehaviour
{
    [Header("Reference: GameMananger")]
    [SerializeField] private GameManager gameManager;

    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private List<Node<Vector2>> path = new List<Node<Vector2>>();
    private Pathfinder<Node<Vector2>> pathfinder;

    private FSM<Directions, Flags> fsm;

    void Start()
    {
        InitPathfinder();
        InitFSM();
    }

    void Update()
    {
        fsm.Tick();
    }

    public void InitPathfinder()
    {
        pathfinder = grapfView.GetPathfinderType() switch
        {
            PathfinderType.AStar => new AStarPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Dijkstra => new DijstraPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Breath => new BreadthPathfinder<Node<Vector2>, Vector2>(),

            PathfinderType.Depth => new DepthFirstPathfinder<Node<Vector2>, Vector2>(),

            _ => new AStarPathfinder<Node<Vector2>, Vector2>()
        };
    }

    public void InitFSM()
    {
        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<WaitState>(Directions.Wait, onTickParameters: () => OnTickParametersWaitState(), onEnterParameters: () => OnEnterParametersWaitState());

        fsm.AddBehaviour<WalkState>(Directions.Walk, onTickParameters: () => OnTickParametersWalkState(), onEnterParameters: () => OnEnterParametersWalkState());

        fsm.AddBehaviour<DeliverState>(Directions.Deliver, onTickParameters: () => OnTickParametersDeliverState(), onEnterParameters: () => OnEnterParametersDeliverState());

        fsm.AddBehaviour<GatherState>(Directions.Gather, onTickParameters: () => OnTickParametersGatherState(), onEnterParameters: () => OnEnterParametersGatherState());

        fsm.AddBehaviour<EatingState>(Directions.Eat, onTickParameters: () => OnTickParametersEatingState(), onEnterParameters: () => OnEnterParametersEatingState());


        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log(Directions.Wait + " to " + Directions.Walk); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachMine, Directions.Deliver, () => { Debug.Log(Directions.Walk + " to " + Directions.Deliver); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachHome, Directions.Gather, () => { Debug.Log(Directions.Walk + " to " + Directions.Gather); });
        fsm.SetTransition(Directions.Deliver, Flags.OnFoodEmpty, Directions.Walk, () => { Debug.Log(Directions.Deliver + " to " + Directions.Walk); });
        fsm.SetTransition(Directions.Gather, Flags.OnFoodFull, Directions.Wait, () => { Debug.Log(Directions.Gather + " to " + Directions.Wait); });

        fsm.ForceTransition(Directions.Wait);
    }

    public object[] OnTickParametersWaitState()
    {
        return new object[] { gameManager.GetCaravanAgent(), gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersWaitState()
    {
        return new object[] { grapfView, transform };
    }

    public object[] OnTickParametersWalkState()
    {
        return new object[] { transform, gameManager.GetCaravanAgent() };
    }

    public object[] OnEnterParametersWalkState() 
    { 
        return new object[] { grapfView, path, pathfinder, gameManager.GetCaravanAgent() }; 
    }

    public object[] OnTickParametersDeliverState()
    {
        return new object[] { gameManager.GetCaravanAgent(), gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersDeliverState()
    {
        return new object[] { gameManager.GetCaravanAgent() };
    }

    public object[] OnTickParametersGatherState()
    {
        return new object[] { gameManager.GetCaravanAgent(), gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersGatherState()
    {
        return new object[] { gameManager.GetCaravanAgent() };
    }

    public object[] OnTickParametersEatingState()
    {
        return new object[] { gameManager.GetCaravanAgent(), gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersEatingState()
    {
        return new object[] { gameManager.GetCaravanAgent() };
    }
}
