using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum Directions
{
    Wait,
    WalkToMine,
    WalkToHome,
    GatherResurces,
    Deliver
}

public enum Flags
{
    OnReachMine,
    OnReachHome,
    OnWait,
    OnGather,
    OnFull,
    OnHunger,
    OnGoToTarget
}

public class Miner
{
    //Movement
    public Transform target;
    public float speed;
    public float reachDistance;
    public bool isTargetReach = true;

    //Inventory
    public int currentGold = 0;
    public int maxGoldToCharge = 3;

    public int currentFood = 0;
    public int maxFood = 3;

    public float miningTime = 1;

    public bool isMinerFull = true;

    public Miner(Transform target, float speed, float reachDistance, bool isTargetReach, int currentGold, int maxGoldToCharge, int currentFood, int maxFood, float miningTime, bool isMinerFull)
    {
        this.target = target;
        this.speed = speed;
        this.reachDistance = reachDistance;
        this.isTargetReach = isTargetReach;

        this.currentGold = currentGold;
        this.maxGoldToCharge = maxGoldToCharge;
        this.currentFood = currentFood;
        this.maxFood = maxFood;
        this.miningTime = miningTime;
        this.isMinerFull = isMinerFull;
    }

    public void SetCurrentGold(int newCurrentGold) 
    {
        currentGold = newCurrentGold;
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    public int GetMaxGoldToCharge() 
    {
        return maxGoldToCharge;
    }

    public void AddGold(int addGold) 
    {
        currentGold += addGold;
    }

    public void RemoveGold(int removeGold)
    {
        currentGold -= removeGold;
    }

    public int GetCurrentFood()
    {
        return currentFood;
    }

    public int GetMaxFood() 
    {
        return maxFood;
    }

    public float GetMiningTime() 
    {
        return miningTime;
    }
}

public class Agent : MonoBehaviour
{
    [Header("Chase State")]
    [SerializeField] private Transform target; 
    [SerializeField] private Transform home; 


    [SerializeField] private float speed;
    [SerializeField] private float reachDistance;
    [SerializeField] private bool isTargetReach;

    [Header("Mininer")]
    public Miner miner;

    [Header("Resources")]
    [SerializeField] private int maxResurcesToCharge;
    [SerializeField] private int currentResurces;
    [SerializeField] private bool isResurcesFull;

    [Header("Food")]
    [SerializeField] private int maxFoodToCharge;
    [SerializeField] private int currentFood;
    [SerializeField] private bool isFoodFull;

    [Header("Time")]
    [SerializeField] private float miningTime;

    private FSM<Directions, Flags> fsm;

    public Agent(Transform target, float speed, float reachDistance, 
                 int maxResurcesToCharge, int currentResurces, int maxFoodToCharge, int currentFood, float miningTime) 
    {
        this.target = target;
        this.speed = speed;
        this.reachDistance = reachDistance;

        this.maxResurcesToCharge = maxResurcesToCharge;
        this.currentResurces = currentResurces;
        this.maxFoodToCharge = maxFoodToCharge;
        this.currentFood = currentFood;
        this.miningTime = miningTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitAgent();

        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<WaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters());
        fsm.AddBehaviour<GoToMineState>(Directions.WalkToMine, onTickParameters: () => GoToMineStateParameters());
        fsm.AddBehaviour<MiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());
        fsm.AddBehaviour<GoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters());
        fsm.AddBehaviour<DeliverState>(Directions.Deliver, onTickParameters: () => DeliverStateParameters());

        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.GatherResurces, () => { Debug.Log("Reach Mine"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnFull, Directions.WalkToHome, () => { Debug.Log("Miner full"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.Deliver, () => { Debug.Log("Reach Home"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });

        fsm.ForceTransition(Directions.Wait);
    }

    public void InitAgent() 
    {
        miner = new Miner(target, speed, reachDistance, isTargetReach, currentResurces, maxResurcesToCharge, currentFood, maxFoodToCharge, miningTime, isResurcesFull);
        miner.target = target;
    }

    public void StartChace() 
    {
        miner.isMinerFull = false;
    }

    void Update()
    {
        fsm.Tick();

        isTargetReach = miner.isTargetReach;
        isResurcesFull = miner.isMinerFull;

        currentResurces = miner.currentGold;
        currentFood = miner.currentFood;
    }

    private object[] GoToMineStateParameters() 
    {
        return new object[] { transform, target, miner};
    }
    private object[] GoToHomeStateParameters()
    {
        return new object[] { transform, home, miner };
    }

    private object[] WaitStateParameters()
    {
        return new object[] { miner };
    }

    private object[] MiningStateParameters()
    {
        return new object[] { miner };
    }

    private object[] DeliverStateParameters()
    {
        return new object[] { miner };
    }
}
