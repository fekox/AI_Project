using System.Collections.Generic;
using UnityEngine;

public enum AgentType
{
    Miner,
    Caravan
}

public class Agent : MonoBehaviour
{
    //AgentType
    [Header("AgentType")]
    [SerializeField] private AgentType agentType;

    //Movement
    [Header("Movement")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float reachDistance = 0.01f;
    [SerializeField] private bool isTargetReach = false;
    [SerializeField] private bool startLoop = false;

    //Food
    [Header("Food")]
    [SerializeField] private int currentFood = 0;
    [SerializeField] private int maxFoodToCharge = 10;
    [SerializeField] private bool isFoodFull = true;

    //Gold
    [Header("Gold")]
    [SerializeField] private int currentGold = 0;
    [SerializeField] private int maxGoldToCharge = 15;
    [SerializeField] private bool isGoldFull = false;

    //Timer
    [Header("Timers")]
    [SerializeField] private float deliveringTime = 0.5f;
    [SerializeField] private float miningTime = 0.5f;
    [SerializeField] private float eatingTime = 0.5f;

    //Location
    [Header("Location")]
    [SerializeField] private bool isOnMine = false;
    [SerializeField] private bool isOnHome = true;

    [Header("Reference: GrapfView")]
    public GrapfView grapfView;

    private List<Node<Vector2>> path = new List<Node<Vector2>>();
    private Pathfinder<Node<Vector2>> pathfinder;

    private FSM<Directions, Flags> fsm;

    public Agent() { }
    public Agent(AgentType agentType,
                 float speed, float reachDistance, bool startLoop, bool isTargetReach,  //Movement
                 int currentFood, int maxFood, bool isFoodFull,                         //Food
                 int currentGold, int maxGold, bool isGoldFull,                         //Gold
                 float deliveringTime, float miningTime, float eatingTime,              //Timers
                 bool isOnMine, bool isOnHome)                                          //Location
    {
        //AgentType
        this.agentType = agentType;

        //Movement
        this.speed = speed;
        this.reachDistance = reachDistance;
        this.isTargetReach = isTargetReach;
        this.startLoop = startLoop;

        //Food
        this.currentFood = currentFood;
        this.maxFoodToCharge = maxFood;
        this.isFoodFull = isFoodFull;

        //Gold
        this.currentGold = currentGold;
        this.maxGoldToCharge = maxGold;
        this.isGoldFull = isGoldFull;

        //Timers
        this.deliveringTime = deliveringTime;
        this.miningTime = miningTime;
        this.eatingTime = eatingTime;

        //Location
        this.isOnMine = isOnMine;
        this.isOnHome = isOnHome;
    }

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




        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Caravan: Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.Deliver, () => { Debug.Log("Caravan: Reach Mine"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnFoodEmpty, Directions.WalkToHome, () => { Debug.Log("Caravan: Deliver"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.GatherResurces, () => { Debug.Log("Caravan: Reach Home"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnFoodFull, Directions.Wait, () => { Debug.Log("Caravan: Wait"); });

        fsm.ForceTransition(Directions.Wait);
    }


    //Agent Type
    public AgentType GetAgentType() { return agentType; }
    public void SetAgetnType(AgentType newAgentType) { agentType = newAgentType; }


    //Movement
    public float GetSpeed() { return speed; }
    public void SetSpeed(int number) { speed = number; }
    public float GetReachDistance() { return reachDistance;}
    public void SetReachDistance(int number) { reachDistance = number; }
    public bool IsTargetReach() { return isTargetReach;}
    public void SetIsTargetReach(bool value) { isTargetReach = value; }
    public bool IsStartLoop() { return startLoop; }
    public void SetIsStartLoop(bool value) { startLoop = value; }


    //Food
    public int GetCurrentFood() { return currentFood; }
    public void SetCurrentFood(int value) {  currentFood = value; }
    public int GetMaxFood() { return maxFoodToCharge; }
    public void SetMaxFood(int value) {  maxFoodToCharge = value; }
    public bool IsFoodFull() { return isFoodFull; }
    public void SetIsFoodFull(bool value) { isFoodFull = value; }
    public void RemoveFood(int number) { currentFood -= number; }
    public void AddFood(int number) { currentFood += number; }


    //Gold
    public int GetCurrentGold() { return currentGold; }
    public void SetCurrentGold(int value) { currentGold = value; }
    public int GetMaxGold() { return maxGoldToCharge; }
    public void SetMaxGold(int value) { maxGoldToCharge = value; }
    public bool IsGoldFull() {  return isGoldFull; }
    public void SetIsGoldFull(bool value) { isGoldFull = value; }
    public void AddGold(int addGold) { currentGold += addGold; }
    public void RemoveGold(int removeGold) { currentGold -= removeGold; }


    //Timers
    public float GetDeliveringTime() { return deliveringTime; }
    public void SetDeliveringTime(float value) {  deliveringTime = value; }
    public float GetMiningTime() {return miningTime; }
    public void SetMiningTime(float number) { miningTime = number; }
    public float GetEatingTime() { return eatingTime; }
    public void SetEatingTime(float number) {  eatingTime = number; }


    //Location
    public bool IsOnMine() { return isOnMine; }
    public void SetIsOnMine(bool value) { isOnMine = value; }
    public bool IsOnHome() { return isOnHome; }
    public void SetIsOnHome(bool value) { isOnHome = value; }
}
