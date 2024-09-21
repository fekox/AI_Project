//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class MinerAgent : MonoBehaviour
//{
//    [Header("Reference: GameManager")]
//    [SerializeField] private GameManager gameManager;

//    private FSM<Directions, Flags> fsm;

//    [Header("Reference: GrapfView")]
//    public GrapfView grapfView;

//    private List<Node<Vector2>> path = new List<Node<Vector2>>();
//    private Pathfinder<Node<Vector2>> pathfinder;

//    void Start()
//    {
//        InitPathfinder();
//        InitFSM();
//    }

//    private void Update()
//    {
//        fsm.Tick();
//    }

//    public void InitPathfinder() 
//    {
//        pathfinder = grapfView.GetPathfinderType() switch
//        {
//            PathfinderType.AStar => new AStarPathfinder<Node<Vector2>, Vector2>(),

//            PathfinderType.Dijkstra => new DijstraPathfinder<Node<Vector2>, Vector2>(),

//            PathfinderType.Breath => new BreadthPathfinder<Node<Vector2>, Vector2>(),

//            PathfinderType.Depth => new DepthFirstPathfinder<Node<Vector2>, Vector2>(),

//            _ => new AStarPathfinder<Node<Vector2>, Vector2>()
//        };
//    }

//    public void InitFSM() 
//    {
//        fsm = new FSM<Directions, Flags>();

//        fsm.AddBehaviour<MinerWaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters(), onEnterParameters: () => EnterWaitStateParameters());
//        fsm.AddBehaviour<MinerGoToMineState>(Directions.WalkToMine, onTickParameters: () => TickGoToMineStateParameters(), onEnterParameters: () => EnterGoToMineStateParameters());
//        fsm.AddBehaviour<MinerMiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());
//        fsm.AddBehaviour<MinerEatingState>(Directions.NeedFood, onTickParameters: () => EatStateParameters());
//        fsm.AddBehaviour<MinerWaitingForFoodState>(Directions.WaitFood, onTickParameters: () => WaitingForFoodStateParameters());
//        fsm.AddBehaviour<MinerWaitingForGoldState>(Directions.WaitGold, onTickParameters: () => WaitingForGoldStateParameters());

//        fsm.AddBehaviour<MinerGoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters(), onEnterParameters: () => EnterGoToHomeStateParameters());
//        fsm.AddBehaviour<MinerDeliverState>(Directions.Deliver, onTickParameters: () => DeliverStateParameters());

//        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });
//        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.GatherResurces, () => { Debug.Log("Reach Mine"); });

//        fsm.SetTransition(Directions.GatherResurces, Flags.OnHunger, Directions.NeedFood, () => { Debug.Log("Need Food"); });
//        fsm.SetTransition(Directions.NeedFood, Flags.OnNoFoodOnMine, Directions.WaitFood, () => { Debug.Log("Waiting for food"); });

//        fsm.SetTransition(Directions.GatherResurces, Flags.OnNoGoldOnMine, Directions.WaitGold, () => { Debug.Log("Waiting for Gold"); });
//        fsm.SetTransition(Directions.WaitFood, Flags.OnFoodFull, Directions.NeedFood, () => { Debug.Log("Food on mine"); });
//        fsm.SetTransition(Directions.NeedFood, Flags.OnFoodFull, Directions.GatherResurces, () => { Debug.Log("Food full"); });
//        fsm.SetTransition(Directions.GatherResurces, Flags.OnGoldFull, Directions.WalkToHome, () => { Debug.Log("Miner full"); });
//        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.Deliver, () => { Debug.Log("Reach Home"); });
//        fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });

//        fsm.ForceTransition(Directions.Wait);
//    }

//    private object[] EnterWaitStateParameters()
//    {
//        return new object[] { grapfView, transform };
//    }

//    private object[] WaitStateParameters()
//    {
//        return new object[] { gameManager.GetMiner() };
//    }

//    private object[] EnterGoToMineStateParameters()
//    {
//        return new object[] { grapfView, path, pathfinder };
//    }
//    private object[] TickGoToMineStateParameters() 
//    {
//        return new object[] { transform, gameManager.GetMiner() };
//    }
    
//    private object[] EnterGoToHomeStateParameters() 
//    {
//        return new object[] { grapfView, path, pathfinder };
//    }

//    private object[] GoToHomeStateParameters()
//    {
//        return new object[] { transform, gameManager.GetMiner() };
//    }


//    private object[] MiningStateParameters()
//    {
//        return new object[] { gameManager.GetMiner(), gameManager.GetMine()};
//    }

//    private object[] EatStateParameters() 
//    {
//        return new object[] { gameManager.GetMiner(), gameManager.GetMine() };
//    }

//    private object[] WaitingForFoodStateParameters()
//    {
//        return new object[] { gameManager.GetMine() };
//    }

//    private object[] WaitingForGoldStateParameters()
//    {
//        return new object[] { gameManager.GetMine() };
//    }

//    private object[] DeliverStateParameters()
//    {
//        return new object[] { gameManager.GetMiner() };
//    }
//}