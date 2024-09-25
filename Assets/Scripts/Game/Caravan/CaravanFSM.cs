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

        fsm.AddBehaviour<AlarmState>(Directions.Alarm, onTickParameters: () => OnTickParametersAlarmState(), onEnterParameters: () => OnEnterParametersAlarmState());

        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log(Directions.Wait + " to " + Directions.Walk); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachMine, Directions.Deliver, () => { Debug.Log(Directions.Walk + " to " + Directions.Deliver); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachHome, Directions.Gather, () => { Debug.Log(Directions.Walk + " to " + Directions.Gather); });
        fsm.SetTransition(Directions.Deliver, Flags.OnFoodEmpty, Directions.Walk, () => { Debug.Log(Directions.Deliver + " to " + Directions.Walk); });
        fsm.SetTransition(Directions.Gather, Flags.OnFoodFull, Directions.Wait, () => { Debug.Log(Directions.Gather + " to " + Directions.Wait); });

        //Alarm State
        fsm.SetTransition(Directions.Wait, Flags.OnAlarmActive, Directions.Alarm, () => { Debug.Log(Directions.Wait + " to " + Directions.Alarm); });
        fsm.SetTransition(Directions.Alarm, Flags.OnAlarmDesactiveOnHome, Directions.Wait, () => { Debug.Log(Directions.Alarm + " to " + Directions.Wait); });


        fsm.SetTransition(Directions.Walk, Flags.OnAlarmActive, Directions.Alarm, () => { Debug.Log(Directions.Walk + " to " + Directions.Alarm); });
        fsm.SetTransition(Directions.Deliver, Flags.OnAlarmActive, Directions.Alarm, () => { Debug.Log(Directions.Deliver + " to " + Directions.Alarm); });
        fsm.SetTransition(Directions.Gather, Flags.OnAlarmActive, Directions.Alarm, () => { Debug.Log(Directions.Gather + " to " + Directions.Alarm); });
        fsm.SetTransition(Directions.Alarm, Flags.OnAlarmDesactiveAutHome, Directions.Walk, () => { Debug.Log(Directions.Alarm + " to " + Directions.Walk); });

        fsm.ForceTransition(Directions.Wait);
    }

    public object[] OnTickParametersWaitState()
    {
        return new object[] { gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersWaitState()
    {
        return new object[] { grapfView, transform, gameManager.GetCaravanAgent() };
    }

    public object[] OnTickParametersWalkState()
    {
        return new object[] {  };
    }

    public object[] OnEnterParametersWalkState() 
    { 
        return new object[] { grapfView, path, pathfinder, gameManager.GetCaravanAgent(), transform }; 
    }

    public object[] OnTickParametersDeliverState()
    {
        return new object[] { gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersDeliverState()
    {
        return new object[] { gameManager.GetCaravanAgent() };
    }

    public object[] OnTickParametersGatherState()
    {
        return new object[] { gameManager.GetOneMine(0) };
    }

    public object[] OnEnterParametersGatherState()
    {
        return new object[] { gameManager.GetCaravanAgent() };
    }

    public object[] OnTickParametersAlarmState()
    {
        return new object[] { };
    }

    public object[] OnEnterParametersAlarmState()
    {
        return new object[] { grapfView, path, pathfinder, gameManager.GetCaravanAgent(), transform };
    }
}
