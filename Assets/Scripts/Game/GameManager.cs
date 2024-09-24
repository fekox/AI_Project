using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Directions
{
    Wait,
    Walk,
    Gather,
    Eat,
    WaitFood,
    WaitGold,
    Deliver,
    Alarm
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
    OnGoToNewTarget,
    OnAlarmActive,
    OnAlarmDesactiveOnHome,
    OnAlarmDesactiveAutHome,
}

public class GameManager : MonoBehaviour
{
    [Header("Mines")]
    public List<Mine> mines;

    [Header("Miner")]
    public Agent minerAgent;

    [Header("Caravan")]
    public Agent caravanAgent;

    private bool isButtonAlarmActive = false;

    public Image button;

    public void StartLoop()
    {
        minerAgent.SetIsStartLoop(true);
        caravanAgent.SetIsStartLoop(true);
    }

    public void Alarm() 
    {
        isButtonAlarmActive =  !isButtonAlarmActive;

        minerAgent.SetIsAlarmActive(isButtonAlarmActive);
        caravanAgent.SetIsAlarmActive(isButtonAlarmActive);

        if (isButtonAlarmActive) 
        {
            button.color = Color.red;
        }

        else 
        {
            button.color = Color.white;
        }
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