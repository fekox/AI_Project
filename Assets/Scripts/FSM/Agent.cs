using System;
using System.Collections;
using System.Collections.Generic;
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

public class Agent : MonoBehaviour
{
    [Header("Chase State")]
    [SerializeField] private Transform target; 

    [SerializeField] private float speed;
    [SerializeField] private float reachDistance;
    private bool startChase;

    [Header("Mining State")]
    [SerializeField] private int maxResurcesToCharge;
    [SerializeField] private int currentResurces;
    [SerializeField] private float miningSpeed;

    [Header("Food")]
    [SerializeField] private int food; 
    [SerializeField] private int maxFood;


    private FSM<Directions, Flags> fsm;

    // Start is called before the first frame update
    void Start()
    {
        InitAgent();

        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<WaitState>(Directions.Wait, onTickParameters: () => WaitStateParameters());
        fsm.AddBehaviour<GoToTargetState>(Directions.Walk, onTickParameters: () => GoToMnieStateParameters());
        fsm.AddBehaviour<WaitState>(Directions.GatherResurces, onTickParameters: () => WaitStateParameters());

        fsm.AddBehaviour<MiningState>(Directions.GatherResurces, onTickParameters: () => MiningStateParameters());


        fsm.SetTransition(Directions.Wait, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log("Go to target"); });
        fsm.SetTransition(Directions.Walk, Flags.OnReachTarget, Directions.GatherResurces, () => { Debug.Log("Reach target"); });

        fsm.SetTransition(Directions.GatherResurces, Flags.OnGather, Directions.Walk, () => { Debug.Log("Get resurces"); });

        //fsm.SetTransition(Directions.Walk, Flags.OnGoToTarget, Directions.Wait, () => { Debug.Log("Reach target"); });
        //fsm.SetTransition(Directions.GatherResurces, Flags.OnHunger, Directions.Wait, () => { Debug.Log("Hungry"); });
        //fsm.SetTransition(Directions.GatherResurces, Flags.OnFull, Directions.Walk, () => { Debug.Log("Return Home"); });
        //fsm.SetTransition(Directions.Deliver, Flags.OnGoToTarget, Directions.Walk, () => { Debug.Log("Stop deliver"); });
        //fsm.SetTransition(Directions.Walk, Flags.OnGoToTarget, Directions.Deliver, () => { Debug.Log("Deliver"); });

        fsm.ForceTransition(Directions.Wait);
    }

    public void InitAgent() 
    {
        food = maxFood;
    }

    public void StartChace() 
    {
        startChase = true;
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }

    private object[] GoToMnieStateParameters() 
    {
        return new object[] { transform, target, speed, reachDistance};
    }

    private object[] WaitStateParameters()
    {
        return new object[] { startChase };
    }

    private object[] MiningStateParameters()
    {
        return new object[] { maxResurcesToCharge, currentResurces, miningSpeed };
    }

    private object[] GatherResurcesStateParameters()
    {
        return new object[] { transform, currentResurces };
    }

    private object[] DeliverStateParameters()
    {
        return new object[] { transform, currentResurces };
    }
}
