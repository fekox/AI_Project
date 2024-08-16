using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    Chase,
    Patrol,
    Explode
}

public enum Flags
{
    OnTargetReach,
    OnTargetLost,
    OnTargetNear
}

public class Agent : MonoBehaviour
{
    [Header("Chase State")]
    [SerializeField] private Transform target; 

    [SerializeField] private float speed;
    [SerializeField] private float reachDistance;

    [Header("Patrol State")]
    [SerializeField] private Transform waypoint1;
    [SerializeField] private Transform waypoint2;

    [SerializeField] private float lostDistnace;

    [SerializeField] private float chaseDistance;

    private FSM<Directions, Flags> fsm;


    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSM<Directions, Flags>();

        fsm.AddBehaviour<PatrolState>(Directions.Patrol, onTickParameters: () => { return new object[] { transform, waypoint1, waypoint2, target, speed, chaseDistance }; });
        fsm.AddBehaviour<ChaseState>(Directions.Chase, onTickParameters: () => { return new object[] { transform, target, speed, reachDistance, lostDistnace }; });
        fsm.AddBehaviour<ExplodeState>(Directions.Explode);

        fsm.SetTransition(Directions.Patrol, Flags.OnTargetNear, Directions.Chase, () => { Debug.Log("Te vi!"); });
        fsm.SetTransition(Directions.Chase, Flags.OnTargetLost, Directions.Patrol, () => { Debug.Log("Te perdi!"); });
        fsm.SetTransition(Directions.Chase, Flags.OnTargetReach, Directions.Explode, () => { Debug.Log("Explote!"); });

        fsm.ForceTransition(Directions.Patrol);
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }

    private object[] ChaseStateParameters() 
    {
        return new object[] { transform, target, speed, reachDistance, lostDistnace };
    }

    private object[] PatrolStateParameters()
    {
        return new object[] { transform, waypoint1, waypoint2, target, speed, chaseDistance};
    }

    private object[] ExplodeStateParameters()
    {
        return new object[] { };
    }
}
