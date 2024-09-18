using UnityEngine;

public class CaravanAgent : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private FSM<Directions, Flags> fsm;

    void Start()
    {
        InitFSM();
    }

    void Update()
    {
        fsm.Tick();
    }

    public void InitFSM() 
    {
        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<CaravanWaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters());
        fsm.AddBehaviour<CaravanGoToMineState>(Directions.WalkToMine, onTickParameters: () => GoToMineStateParameters());
        fsm.AddBehaviour<CaravanDeliverState>(Directions.Deliver, onTickParameters: () => DeliveringStateParameters());
        fsm.AddBehaviour<CaravanGoToHomeState>(Directions.WalkToHome, onTickParameters: () => GoToHomeStateParameters());
        fsm.AddBehaviour<CaravanCollectFoodState>(Directions.GatherResurces, onTickParameters: () => CollectFoodStateParameters());

        
        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.WalkToMine, () => { Debug.Log("Caravan: Start"); });
        fsm.SetTransition(Directions.WalkToMine, Flags.OnReachMine, Directions.Deliver, () => { Debug.Log("Caravan: Reach Mine"); });
        fsm.SetTransition(Directions.Deliver, Flags.OnFoodEmpty, Directions.WalkToHome, () => { Debug.Log("Caravan: Deliver"); });
        fsm.SetTransition(Directions.WalkToHome, Flags.OnReachHome, Directions.GatherResurces, () => { Debug.Log("Caravan: Reach Home"); });
        fsm.SetTransition(Directions.GatherResurces, Flags.OnFoodFull, Directions.Wait, () => { Debug.Log("Caravan: Wait"); });

        fsm.ForceTransition(Directions.Wait);
    }

    private object[] WaitStateParameters()
    {
        return new object[] { gameManager.GetMine() };
    }

    private object[] GoToMineStateParameters()
    {
        return new object[] { gameManager.caravanTransform, gameManager.target, gameManager.GetCaravan() };
    }

    private object[] DeliveringStateParameters() 
    {
        return new object[] { gameManager.GetCaravan(), gameManager.GetMine()};
    }

    private object[] GoToHomeStateParameters() 
    {
        return new object[] { gameManager.caravanTransform, gameManager.home, gameManager.GetCaravan() };
    }

    private object[] CollectFoodStateParameters()
    {
        return new object[] { gameManager.GetCaravan() };
    }
}
