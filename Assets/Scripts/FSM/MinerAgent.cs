using UnityEngine;

public class MinerAgent : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private FSM<Directions, Flags> fsm;

    void Start()
    {
        InitFSM();
    }

    private void Update()
    {
        fsm.Tick();
    }

    public void InitFSM() 
    {
        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<MinerWaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters());
        fsm.AddBehaviour<MinerGoToMineState>(Directions.WalkToMine, onTickParameters: () => GoToMineStateParameters());
        fsm.AddBehaviour<MinerMiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());
        fsm.AddBehaviour<MinerEatingState>(Directions.NeedFood, onTickParameters: () => EatStateParameters());

        fsm.AddBehaviour<MinerWaitingForFoodState>(Directions.WaitFood, onTickParameters: () => WaitingForFoodStateParameters());

        fsm.AddBehaviour<MinerWaitingForGoldState>(Directions.WaitGold, onTickParameters: () => WaitingForGoldStateParameters());


        fsm.AddBehaviour<MinerGoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters());
        fsm.AddBehaviour<MinerDeliverState>(Directions.Deliver, onTickParameters: () => DeliverStateParameters());

        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.GatherResurces, () => { Debug.Log("Reach Mine"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnHunger, Directions.NeedFood, () => { Debug.Log("Need Food"); });
        fsm.SetTransition(Directions.NeedFood, Flags.OnNoFoodOnMine, Directions.WaitFood, () => { Debug.Log("Waiting for food"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnNoGoldOnMine, Directions.WaitGold, () => { Debug.Log("Waiting for Gold"); });
        fsm.SetTransition(Directions.WaitFood, Flags.OnFoodFull, Directions.NeedFood, () => { Debug.Log("Food on mine"); });
        fsm.SetTransition(Directions.NeedFood, Flags.OnFoodFull, Directions.GatherResurces, () => { Debug.Log("Food full"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnGoldFull, Directions.WalkToHome, () => { Debug.Log("Miner full"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.Deliver, () => { Debug.Log("Reach Home"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Start"); });

        fsm.ForceTransition(Directions.Wait);
    }
    private object[] GoToMineStateParameters() 
    {
        return new object[] { gameManager.minerTransform, gameManager.target, gameManager.GetMiner()};
    }
    private object[] GoToHomeStateParameters()
    {
        return new object[] { gameManager.minerTransform, gameManager.home, gameManager.GetMiner() };
    }

    private object[] WaitStateParameters()
    {
        return new object[] { gameManager.GetMiner() };
    }

    private object[] MiningStateParameters()
    {
        return new object[] { gameManager.GetMiner(), gameManager.GetMine()};
    }

    private object[] EatStateParameters() 
    {
        return new object[] { gameManager.GetMiner(), gameManager.GetMine() };
    }

    private object[] WaitingForFoodStateParameters()
    {
        return new object[] { gameManager.GetMine() };
    }

    private object[] WaitingForGoldStateParameters()
    {
        return new object[] { gameManager.GetMine() };
    }

    private object[] DeliverStateParameters()
    {
        return new object[] { gameManager.GetMiner() };
    }
}