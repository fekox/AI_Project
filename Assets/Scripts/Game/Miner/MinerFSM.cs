using System.Collections.Generic;
using UnityEngine;

public class MinerFSM : MonoBehaviour
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

        fsm.AddBehaviour<WaitingForFoodState>(Directions.WaitFood, onTickParameters: () => OnTickParametersWaitingForFoodState());

        fsm.AddBehaviour<WaitingForGoldState>(Directions.WaitGold, onTickParameters: () => OnTickParametersWaitingForGoldState());



        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log(Directions.Wait + " to " + Directions.Walk); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachMine, Directions.Gather, () => { Debug.Log(Directions.Walk + " to " + Directions.Deliver); });


        fsm.SetTransition(Directions.Walk, Flags.OnReachHome, Directions.Deliver, () => { Debug.Log(Directions.Walk + " to " + Directions.Gather); });
        fsm.SetTransition(Directions.Gather, Flags.OnFoodEmpty, Directions.Walk, () => { Debug.Log(Directions.Deliver + " to " + Directions.Walk); });

        fsm.SetTransition(Directions.Gather, Flags.OnHunger, Directions.Eat, () => { Debug.Log(Directions.Gather + " to " + Directions.NeedFood); });
        fsm.SetTransition(Directions.Eat, Flags.OnNoFoodOnMine, Directions.WaitFood, () => { Debug.Log(Directions.NeedFood + " to " + Directions.WaitFood); });

        fsm.SetTransition(Directions.Gather, Flags.OnNoGoldOnMine, Directions.WaitGold, () => { Debug.Log(Directions.Gather + " to " + Directions.WaitGold); });
        fsm.SetTransition(Directions.Gather, Flags.OnGoldFull, Directions.Walk, () => { Debug.Log(Directions.Gather + " to " + Directions.Walk); });

        fsm.SetTransition(Directions.WaitFood, Flags.OnFoodFull, Directions.Eat, () => { Debug.Log(Directions.WaitFood + " to " + Directions.Eat); });
        fsm.SetTransition(Directions.Eat, Flags.OnFoodFull, Directions.Gather, () => { Debug.Log(Directions.Eat + " to " + Directions.Gather); });

        fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log(Directions.Deliver + " to " + Directions.Walk); });



        fsm.ForceTransition(Directions.Wait);
    }

    public object[] OnTickParametersWaitState()
    {
        return new object[] { gameManager.GetMinerAgent(), gameManager.GetMine() };
    }

    public object[] OnEnterParametersWaitState()
    {
        return new object[] { grapfView, transform };
    }

    public object[] OnTickParametersWalkState()
    {
        return new object[] { transform, gameManager.GetMinerAgent() };
    }

    public object[] OnEnterParametersWalkState()
    {
        return new object[] { grapfView, path, pathfinder, gameManager.GetMinerAgent() };
    }

    public object[] OnTickParametersDeliverState()
    {
        return new object[] { gameManager.GetMinerAgent(), gameManager.GetMine() };
    }

    public object[] OnEnterParametersDeliverState()
    {
        return new object[] { gameManager.GetMinerAgent() };
    }

    public object[] OnTickParametersGatherState()
    {
        return new object[] { gameManager.GetMinerAgent(), gameManager.GetMine() };
    }

    public object[] OnEnterParametersGatherState()
    {
        return new object[] { gameManager.GetMinerAgent() };
    }

    public object[] OnTickParametersEatingState()
    {
        return new object[] { gameManager.GetMinerAgent(), gameManager.GetMine() };
    }

    public object[] OnEnterParametersEatingState()
    {
        return new object[] { gameManager.GetMinerAgent() };
    }

    public object[] OnTickParametersWaitingForFoodState()
    {
        return new object[] { gameManager.GetMine() };
    }

    public object[] OnTickParametersWaitingForGoldState()
    {
        return new object[] { gameManager.GetMine() };
    }
}
