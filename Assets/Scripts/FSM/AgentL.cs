using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehavioursL
{
    Chase, Patrol, Expode
}

public enum FlagsL
{
    OnTargetReach, OnTargetNear, OnTargetLost
}
public class AgentL : MonoBehaviour
{
    private FSML fsm;

    // Start is called before the first frame update
    void Start()
    {
        fsm = new FSML(Enum.GetValues(typeof(Behaviour)).Length, Enum.GetValues(typeof(FlagsL)).Length);
    }

    // Update is called once per frame
    void Update()
    {
        fsm.Tick();
    }

}