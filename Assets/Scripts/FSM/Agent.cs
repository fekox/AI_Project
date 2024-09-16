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
    NeedFood,
    GatherResurces,
    Deliver
}

public enum Flags
{
    OnReachMine,
    OnReachHome,
    OnWait,
    OnGather,
    OnGoldFull,
    OnHunger,
    OnFoodFull,
    OnGoToTarget
}

public class Miner
{
    //Movement
    public Transform target;
    public float speed;
    public float reachDistance;
    public bool isTargetReach = true;
    public bool startLoop = false;

    //Inventory
    public int currentGold = 0;
    public int maxGoldToCharge = 15;

    public int currentFood = 0;
    public int maxFood = 3;

    public float miningTime = 0.5f;
    public float eatingTime = 0.5f;

    public bool isMinerFull = true;
    public bool isFoodFull = true;

    public Miner(Transform target, float speed, float reachDistance, bool startLoop, 
        bool isTargetReach, int currentGold, int maxGoldToCharge, float miningTime, bool isMinerFull, 
        int currentFood, int maxFood, float eatingTime, bool isFoodFull)
    {
        //Movement
        this.target = target;
        this.speed = speed;
        this.reachDistance = reachDistance;
        this.isTargetReach = isTargetReach;
        this.startLoop = startLoop;

        //Mining
        this.currentGold = currentGold;
        this.maxGoldToCharge = maxGoldToCharge;
        this.miningTime = miningTime;
        this.isMinerFull = isMinerFull;

        //Eating
        this.currentFood = currentFood;
        this.maxFood = maxFood;
        this.eatingTime = eatingTime;
        this.isFoodFull = isFoodFull;
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

    public float GetMiningTime()
    {
        return miningTime;
    }

    public int GetCurrentFood()
    {
        return currentFood;
    }

    public int GetMaxFoodToCharge() 
    {
        return maxFood;
    }

    public void AddFood(int addFood)
    {
        currentFood += addFood;
    }

    public void RemoveFood(int removeFood)
    {
        currentFood -= removeFood;
    }

    public float GetEatingTime()
    {
        return eatingTime;
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
    [SerializeField] private bool startLoop;

    [Header("Mininer")]
    public Miner miner;

    [Header("Resources")]
    [SerializeField] private int maxGoldToCharge;
    [SerializeField] private int currentGold;
    [SerializeField] private bool isMinerFull;

    [Header("Food")]
    [SerializeField] private int maxFoodToCharge;
    [SerializeField] private int currentFood;
    [SerializeField] private bool isFoodFull;

    [Header("Time")]
    [SerializeField] private float miningTime;
    [SerializeField] private float eatingTime;

    private FSM<Directions, Flags> fsm;

    // Start is called before the first frame update
    void Start()
    {
        InitAgent();

        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<WaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters());
        fsm.AddBehaviour<GoToMineState>(Directions.WalkToMine, onTickParameters: () => GoToMineStateParameters());
        fsm.AddBehaviour<MiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());
        fsm.AddBehaviour<EatingState>(Directions.NeedFood, onTickParameters: () => EatStateParameters());
        fsm.AddBehaviour<GoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters());
        fsm.AddBehaviour<DeliverState>(Directions.Deliver, onTickParameters: () => DeliverStateParameters());

        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.GatherResurces, () => { Debug.Log("Reach Mine"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnHunger, Directions.NeedFood, () => { Debug.Log("Need Food"); });
        fsm.SetTransition(Directions.NeedFood, Flags.OnFoodFull, Directions.GatherResurces, () => { Debug.Log("Food full"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnGoldFull, Directions.WalkToHome, () => { Debug.Log("Miner full"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.Deliver, () => { Debug.Log("Reach Home"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });

        fsm.ForceTransition(Directions.Wait);
    }

    public void InitAgent() 
    {
        miner = new Miner(target, speed, reachDistance, isTargetReach, startLoop, 
                          currentGold, maxGoldToCharge, miningTime, isMinerFull, 
                          currentFood, maxFoodToCharge, eatingTime, isFoodFull);

        miner.target = target;
    }

    public void StartChace() 
    {
        miner.startLoop = true;
    }

    void Update()
    {
        fsm.Tick();

        isTargetReach = miner.isTargetReach;
        isMinerFull = miner.isMinerFull;
        isFoodFull = miner.isFoodFull;

        currentGold = miner.currentGold;
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

    private object[] EatStateParameters() 
    {
        return new object[] { miner };
    }

    private object[] DeliverStateParameters()
    {
        return new object[] { miner };
    }
}
