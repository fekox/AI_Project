using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Action<int> OnFlag;

    public abstract List<Action> GetTickBehaviours(params object[] parameters);
    public abstract List<Action> GetOnEnterBehaviours(params object[] parameters);
    public abstract List<Action> GetOnExitBehaviours(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform TargetTramsform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        List<Action> behaiours = new List<Action>();

        behaiours.Add(() =>
        {
            ownerTransform.position += (TargetTramsform.position - ownerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaiours.Add(() =>
        {
            Debug.Log("Whistle!");
        });

        behaiours.Add(() =>
        {
            if (Vector3.Distance(TargetTramsform.position, ownerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetReach);
            }

            else if (Vector3.Distance(TargetTramsform.position, ownerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetLost);
            }

        });

        return behaiours;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;

    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform chaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);

        List<Action> behaiours = new List<Action>();

        behaiours.Add(() =>
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

            ownerTransform.position += (actualTarget.position - ownerTransform.position).normalized * speed * Time.deltaTime;

        });

        behaiours.Add(() =>
        {
            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance) 
            {
                OnFlag?.Invoke((int)Flags.OnTargetNear);
            }
        });

        return behaiours;
    }

}

public sealed class ExplodeState : State
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        List<Action> behaviours = new List<Action>();
        behaviours.Add(() => 
        { 
            Debug.Log("boom"); 
        });

        return behaviours;
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {
        List<Action> behaiours = new List<Action>();

        behaiours.Add(() =>
        {
            Debug.Log("Explode: BOOM!");
        });

        return behaiours;
    }
}
