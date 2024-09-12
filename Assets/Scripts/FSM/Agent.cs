using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public enum Directions
{
    Wait,
    Walk,
    GatherResurces,
    Deliver
}

public enum Flags
{
    OnReachTarget,
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
        fsm.AddBehaviour<GoToTargetState>(Directions.Walk, onTickParameters: () => GoToMineStateParameters());
        fsm.AddBehaviour<MiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());
        fsm.AddBehaviour<MiningState>(Directions.Deliver, onTickParameters: () => DeliverStateParameters());


        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log("Go to target"); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachTarget, Directions.GatherResurces, () => { Debug.Log("Reach target"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnFull, Directions.Walk, () => { Debug.Log("Miner full"); });

        //fsm.SetTransition(Directions.Walk, Flags.OnGoToTarget, Directions.Wait, () => { Debug.Log("Reach target"); });
        //fsm.SetTransition(Directions.GatherResurces, Flags.OnHunger, Directions.Wait, () => { Debug.Log("Hungry"); });
        //fsm.SetTransition(Directions.GatherResurces, Flags.OnFull, Directions.Walk, () => { Debug.Log("Return Home"); });
        //fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log("Stop deliver"); });
        //fsm.SetTransition(Directions.Walk, Flags.OnGoToTarget, Directions.Deliver, () => { Debug.Log("Deliver"); });

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
        return new object[] { transform, target, home, miner};
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
