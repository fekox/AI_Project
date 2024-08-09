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
    [SerializeField] private Transform omnerTransform;
    [SerializeField] private Transform waypont1;
    [SerializeField] private Transform waypoint2;
    [SerializeField] private Transform target;

    [SerializeField] private float speed;
    [SerializeField] private float chaseDistance;

    private FSM fsm;

    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Directions)).Length, Enum.GetValues(typeof(Flags)).Length);

        fsm.AddBehaviour<ChaseState>((int)Directions.Chase, onTickParameters: () => { return new object[] { omnerTransform, waypont1, waypoint2, target, speed, chaseDistance }; });
        fsm.AddBehaviour<PatrolState>((int)Directions.Patrol);
        fsm.AddBehaviour<ExplodeState>((int)Directions.Explode);


        fsm.SetTransition((int)Directions.Chase, (int)Flags.OnTargetLost, (int)Directions.Patrol);
        fsm.SetTransition((int)Directions.Patrol, (int)Flags.OnTargetReach, (int)Directions.Explode);
        fsm.SetTransition((int)Directions.Explode, (int)Flags.OnTargetLost, (int)Directions.Chase);
    }



    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }
}
