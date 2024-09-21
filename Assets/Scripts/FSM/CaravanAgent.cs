using System.Collections.Generic;
using UnityEngine;

public class CaravanAgent : MonoBehaviour
{
    [Header("Reference: GameManager")]
    [SerializeField] private GameManager gameManager;

    private FSM<Directions, Flags> fsm;

    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private List<Node<Vector2>> path = new List<Node<Vector2>>();
    private Pathfinder<Node<Vector2>> pathfinder;

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

        fsm.AddBehaviour<CaravanWaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters(), onEnterParameters: () => EnterWaitStateParameters());

        fsm.AddBehaviour<CaravanGoToMineState>(Directions.WalkToMine, onTickParameters: () => GoToMineStateParameters(), onEnterParameters: () => EnterGoToMineStateParameters());

        fsm.AddBehaviour<CaravanDeliverState>(Directions.Deliver, onTickParameters: () => DeliveringStateParameters());

        fsm.AddBehaviour<CaravanGoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters(), onEnterParameters: () => EnterGoToHomeStateParameters());

        fsm.AddBehaviour<CaravanCollectFoodState>(Directions.GatherResurces, onTickParameters: () => CollectFoodStateParameters());

        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Caravan: Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.Deliver, () => { Debug.Log("Caravan: Reach Mine"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnFoodEmpty, Directions.WalkToHome, () => { Debug.Log("Caravan: Deliver"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.GatherResurces, () => { Debug.Log("Caravan: Reach Home"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnFoodFull, Directions.Wait, () => { Debug.Log("Caravan: Wait"); });

        fsm.ForceTransition(Directions.Wait);
    }

    private object[] EnterWaitStateParameters()
    {
        return new object[] { grapfView, transform };
    }

    private object[] WaitStateParameters()
    {
        return new object[] { gameManager.GetMine() };
    }

    private object[] EnterGoToMineStateParameters()
    {
        return new object[] { grapfView, path, pathfinder };
    }

    private object[] GoToMineStateParameters()
    {
        return new object[] { transform, gameManager.GetCaravan() };
    }

    private object[] DeliveringStateParameters() 
    {
        return new object[] { gameManager.GetCaravan(), gameManager.GetMine()};
    }

    private object[] EnterGoToHomeStateParameters() 
    {
        return new object[] { grapfView, path, pathfinder };
    }

    private object[] GoToHomeStateParameters() 
    {
        return new object[] { transform, gameManager.GetCaravan() };
    }

    private object[] CollectFoodStateParameters()
    {
        return new object[] { gameManager.GetCaravan() };
    }
}
