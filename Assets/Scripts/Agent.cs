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
    [SerializeField] private Transform objectTransform;
    [SerializeField] private Transform target;

    [SerializeField] private float speed;
    [SerializeField] private float reachDistance;

    [Header("Patrol State")]
    [SerializeField] private Transform waypoint1;
    [SerializeField] private Transform waypoint2;

    [SerializeField] private float lostDistnace;

    [SerializeField] private float chaseDistance;

    private FSM fsm;

    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Directions)).Length, Enum.GetValues(typeof(Flags)).Length);

        fsm.AddBehaviour<PatrolState>((int)Directions.Patrol, PatrolStateParameters);
        fsm.AddBehaviour<ChaseState>((int)Directions.Chase, ChaseStateParameters);
        fsm.AddBehaviour<ExplodeState>((int)Directions.Explode, ExplodeStateParameters);

        fsm.SetTransition((int)Directions.Patrol, (int)Flags.OnTargetNear, (int)Directions.Chase);
        fsm.SetTransition((int)Directions.Chase, (int)Flags.OnTargetLost, (int)Directions.Patrol);
        fsm.SetTransition((int)Directions.Explode, (int)Flags.OnTargetReach, (int)Directions.Chase);
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }

    private object[] ChaseStateParameters() 
    {
        return new object[] { objectTransform, target, speed, reachDistance, lostDistnace };
    }

    private object[] PatrolStateParameters()
    {
        return new object[] { objectTransform, waypoint1, waypoint2, target, speed, chaseDistance};
    }

    private object[] ExplodeStateParameters()
    {
        return new object[] { };
    }
}
