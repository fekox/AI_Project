using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public struct BehavioursActions 
{
    private Dictionary<int, List<Action>> mainThreadBehaviours;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multiThreadablesBehaviours;

    private Action transitionBehaviour;
    public void AddMainThreadBehaviours(int executionOrder, Action behaviour)
    {
        if (mainThreadBehaviours == null) 
        {
            mainThreadBehaviours = new Dictionary<int, List<Action>>();
        }

        if (!mainThreadBehaviours.ContainsKey(executionOrder)) 
        {
            mainThreadBehaviours.Add(executionOrder, new List<Action>());
        }

        mainThreadBehaviours[executionOrder].Add(behaviour);
    }

    public void AddMultitreadableBehaviours(int executionOrder, Action behaviour) 
    {
        if(multiThreadablesBehaviours == null) 
        {
            multiThreadablesBehaviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
        }

        if (!multiThreadablesBehaviours.ContainsKey(executionOrder)) 
        {
            multiThreadablesBehaviours.TryAdd(executionOrder, new ConcurrentBag<Action>());
        }

        multiThreadablesBehaviours[executionOrder].Add(behaviour);
    }

    public void SetTransitionBehaviour(Action behaviour) 
    {
        transitionBehaviour = behaviour;
    }

    public Dictionary<int, List<Action>> MainThreadBehaviour => mainThreadBehaviours;

    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultithreadblesBehavoiurs => multiThreadablesBehaviours;

    public Action TransitionBehavour => transitionBehaviour;
}

public abstract class State
{
    public Action<Enum> OnFlag;

    public abstract BehavioursActions GetTickBehaviours(params object[] parameters);
    public abstract BehavioursActions GetOnEnterBehaviours(params object[] parameters);
    public abstract BehavioursActions GetOnExitBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickBehaviours(params object[] parameters)
    {
        Debug.Log("Chase");

        Transform ownerTransform = parameters[0] as Transform;
        Transform TargetTramsform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            ownerTransform.position += (TargetTramsform.position - ownerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaviours.AddMultitreadableBehaviours(0,() =>
        {
            Debug.Log("Whistle!");
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(TargetTramsform.position, ownerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }

            else if (Vector3.Distance(TargetTramsform.position, ownerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetLost);
            }

        });

        return behaviours;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickBehaviours(params object[] parameters)
    {
        Debug.Log("Patrol");
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);

        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0,() =>
        {
            if (actualTarget == null)
            {
                actualTarget = wayPoint1;
            }

            if (Vector3.Distance(ownerTransform.position, actualTarget.position) < 0.2f)
            {
                if (actualTarget == wayPoint1)
                {
                    actualTarget = wayPoint2;
                }

                else
                {
                    actualTarget = wayPoint1;
                }
            }
        });

        behaviours.AddMainThreadBehaviours(1,() =>
        {

            ownerTransform.position += (actualTarget.position - ownerTransform.position).normalized * speed * Time.deltaTime;
        
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            //Debug.Log("Distance: " + Vector3.Distance(ownerTransform.position, chaseTarget.position));
            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance) 
            {
                OnFlag?.Invoke(Flags.OnTargetNear);
            }
        });

        return behaviours;
    }
}

public sealed class ExplodeState : State
{
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        behaviours.AddMultitreadableBehaviours(0,() => 
        { 
            Debug.Log("boom"); 
        });

        return behaviours;
    }

    public override BehavioursActions GetOnExitBehaviours(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0,() =>
        {
            Debug.Log("Explode: BOOM!");
        });

        return behaviours;
    }
}
