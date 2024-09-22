using System.Collections.Generic;
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
    [Header("Mines")]
    public List<Mine> mines;

    [Header("Miner")]
    public Agent minerAgent;

    [Header("Caravan")]
    public Agent caravanAgent;

    public void StartLoop()
    {
        minerAgent.SetIsStartLoop(true);
        caravanAgent.SetIsStartLoop(true);
    }

    public Agent GetMinerAgent() 
    {
        return minerAgent;
    }

    public List<Mine> GetMines()
    {
        return mines;
    }

    public Mine GetOneMine(int number) 
    {
        return mines[number];
    }

    public Agent GetCaravanAgent() 
    {
        return caravanAgent;
    }
}