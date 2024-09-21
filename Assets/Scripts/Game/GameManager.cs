using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
public enum Directions
{
    Wait,
    Walk,
    WalkToMine,
    WalkToHome,
    NeedFood,
    Gather,
    GatherResurces,
    Eat,
    CollectFood,
    WaitFood,
    WaitGold,
    Deliver
}

public enum Flags
{
    OnReachTarget,
    OnReachMine,
    OnReachHome,
    OnWait,
    OnGather,
    OnGoldFull,
    OnHunger,
    OnNoFoodOnMine,
    OnNoGoldOnMine,
    OnFoodFull,
    OnFoodEmpty,
    OnGoToTarget,
    OnGoToNewTarget
}

public class GameManager : MonoBehaviour
{
    [Header("Home")]
    public Transform home;

    [Header("Mine")]
    public Vector2 target;

    public Mine mine;

    [Header("Mine: Food")]
    [SerializeField] private int maxFoodOnMine;
    [SerializeField] private int currentFoodOnMine;

    [Header("Mine: Gold")]
    [SerializeField] private int maxGoldOnMine;
    [SerializeField] private int currentGoldOnMine;

    [Header("Miner")]
    public Miner miner;
    //public Agent minerAgent;

    [Header("Miner: Movement")]

    [SerializeField] private float minerSpeed;
    [SerializeField] private float minerReachDistance;
    [SerializeField] private bool minerIsTargetReach;
    [SerializeField] private bool minerStartLoop;

    [Header("Miner: Location")]
    [SerializeField] private bool minerIsOnHome;
    [SerializeField] private bool minerIsOnMine;

    [Header("Miner: Resources")]
    [SerializeField] private int minerMaxGoldToCharge;
    [SerializeField] private int minerCurrentGold;
    [SerializeField] private bool minerIsGoldFull;

    [Header("Miner: Food")]
    [SerializeField] private int minerMaxFoodToCharge;
    [SerializeField] private int minerCurrentFood;
    [SerializeField] private bool minerIsFoodFull;

    [Header("Miner: Time")]
    [SerializeField] private float minerMiningTime;
    [SerializeField] private float minerEatingTime;

    [Header("Caravan")]
    public Agent caravanAgent;

    //[Header("Caravan: Movement")]
    //public Transform caravanTransform;
    //[SerializeField] public float caravanSpeed;
    //[SerializeField] public float caravanReachDistance;
    //[SerializeField] public bool caravanIsTargetReach;
    //[SerializeField] public bool caravanStartLoop;

    //[Header("Miner: Location")]
    //[SerializeField] private bool caravanIsOnHome;
    //[SerializeField] private bool caravanIsOnMine;

    //[Header("Caravan: Food")]
    //[SerializeField] public int caravanMaxFoodToCharge;
    //[SerializeField] public int caravanCurrentFood;
    //[SerializeField] public bool caravanIsFoodFull;

    //[Header("Caravan: Time")]
    //[SerializeField] public float caravanDeliveringTime;


    private void Start()
    {
        InitMine();
        InitMiner();
    }

    void Update()
    {
        UpdateMineInfo();
        UpdateMinerInfo();
        //UpdateCaravanInfo();
    }

    public void InitMiner()
    {
        miner = new Miner(minerSpeed, minerReachDistance, minerIsTargetReach, minerStartLoop,
                          minerCurrentGold, minerMaxGoldToCharge, minerMiningTime, minerIsGoldFull,
                          minerCurrentFood, minerMaxFoodToCharge, minerEatingTime, minerIsFoodFull);
    }

    public void InitMine()
    {
        mine = new Mine(currentGoldOnMine, maxGoldOnMine, currentFoodOnMine, maxFoodOnMine);
    }


    public void UpdateMineInfo() 
    {
        currentFoodOnMine = mine.GetCurrentFood();
        currentGoldOnMine = mine.GetCurrentGold();
    }

    public void UpdateMinerInfo() 
    {
        minerIsTargetReach = miner.isTargetReach;
        minerIsGoldFull = miner.isMinerFull;
        minerIsFoodFull = miner.isFoodFull;

        minerCurrentGold = miner.GetCurrentGold();
        minerCurrentFood = miner.GetCurrentFood();
    }

    public void StartLoop()
    {
        //minerAgent.SetIsStartLoop(true);
        miner.startLoop = true;
        
        caravanAgent.SetIsStartLoop(true);
    }

    public Miner GetMiner() 
    {
        return miner;
    }

    public Mine GetMine()
    {
        return mine;
    }

    public Agent GetCaravanAgent() 
    {
        return caravanAgent;
    }

    //public Agent GetMinerAgent()
    //{
    //    return minerAgent;
    //}

}