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
    [Header("Mine")]
    public Mine mine;

    [Header("Mine: Food")]
    [SerializeField] private int maxFoodOnMine;
    [SerializeField] private int currentFoodOnMine;

    [Header("Mine: Gold")]
    [SerializeField] private int maxGoldOnMine;
    [SerializeField] private int currentGoldOnMine;

    [Header("Miner")]
    public Agent minerAgent;

    [Header("Caravan")]
    public Agent caravanAgent;


    private void Start()
    {
        InitMine();
    }

    void Update()
    {
        UpdateMineInfo();
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


    public void StartLoop()
    {
        minerAgent.SetIsStartLoop(true);
        caravanAgent.SetIsStartLoop(true);
    }

    public Agent GetMinerAgent() 
    {
        return minerAgent;
    }

    public Mine GetMine()
    {
        return mine;
    }

    public Agent GetCaravanAgent() 
    {
        return caravanAgent;
    }
}