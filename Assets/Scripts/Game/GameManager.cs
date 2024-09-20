using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
public enum Directions
{
    Wait,
    WalkToMine,
    WalkToHome,
    NeedFood,
    GatherResurces,
    CollectFood,
    WaitFood,
    WaitGold,
    Deliver,
}

public enum Flags
{
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

    [Header("Miner: Movement")]
    public Vector2 minerTransform;
    public Vector2 home;

    [SerializeField] private float minerSpeed;
    [SerializeField] private float minerReachDistance;
    [SerializeField] private bool minerIsTargetReach;
    [SerializeField] private bool minerStartLoop;

    [Header("Miner: Resources")]
    [SerializeField] private int minerMaxGoldToCharge;
    [SerializeField] private int minerCurrentGold;
    [SerializeField] private bool isMinerFull;

    [Header("Miner: Food")]
    [SerializeField] private int minerMaxFoodToCharge;
    [SerializeField] private int minerCurrentFood;
    [SerializeField] private bool minerIsFoodFull;

    [Header("Miner: Time")]
    [SerializeField] private float minerMiningTime;
    [SerializeField] private float minerEatingTime;

    [Header("Caravan")]
    public Caravan caravan;

    [Header("Caravan: Food")]
    [SerializeField] public int caravanMaxFoodToCharge;
    [SerializeField] public int caravanCurrentFood;
    [SerializeField] public bool caravanIsFoodFull;

    [Header("Caravan: Movement")]
    public Transform caravanTransform;
    [SerializeField] public float caravanSpeed;
    [SerializeField] public float caravanReachDistance;
    [SerializeField] public bool caravanIsTargetReach;
    [SerializeField] public bool caravanStartLoop;

    [Header("Caravan: Time")]
    [SerializeField] public float caravanDeliveringTime;


    private void Start()
    {
        InitMine();
        InitMiner();
        InitCaravna();
    }

    void Update()
    {
        UpdateMineInfo();
        UpdateMinerInfo();
        UpdateCaravanInfo();
    }

    public void InitMiner()
    {
        miner = new Miner(minerSpeed, minerReachDistance, minerIsTargetReach, minerStartLoop,
                          minerCurrentGold, minerMaxGoldToCharge, minerMiningTime, isMinerFull,
                          minerCurrentFood, minerMaxFoodToCharge, minerEatingTime, minerIsFoodFull);
    }

    public void InitMine()
    {
        mine = new Mine(currentGoldOnMine, maxGoldOnMine, currentFoodOnMine, maxFoodOnMine);
    }

    public void InitCaravna() 
    {
        caravan = new Caravan(caravanSpeed, caravanReachDistance, caravanStartLoop, caravanIsTargetReach, 
                              caravanCurrentFood, caravanMaxFoodToCharge, caravanIsFoodFull, caravanDeliveringTime);
    }

    public void UpdateMineInfo() 
    {
        currentFoodOnMine = mine.GetCurrentFood();
        currentGoldOnMine = mine.GetCurrentGold();
    }

    public void UpdateMinerInfo() 
    {
        minerIsTargetReach = miner.isTargetReach;
        isMinerFull = miner.isMinerFull;
        minerIsFoodFull = miner.isFoodFull;

        minerCurrentGold = miner.GetCurrentGold();
        minerCurrentFood = miner.GetCurrentFood();
    }

    public void UpdateCaravanInfo() 
    {
        caravanCurrentFood = caravan.GetCurrentFood();
        caravanMaxFoodToCharge = caravan.GetMaxFoodToCharge();

        caravanIsTargetReach = caravan.isTargetReach;
        caravanIsFoodFull = caravan.isFoodFull;
    }

    public void StartChace()
    {
        miner.startLoop = true;
        caravan.startLoop = true;
    }

    public Miner GetMiner() 
    {
        return miner;
    }

    public Mine GetMine()
    {
        return mine;
    }

    public Caravan GetCaravan() 
    {
        return caravan;
    }
}