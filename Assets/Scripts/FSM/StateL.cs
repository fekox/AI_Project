using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class StateL
{
    public Action<int> OnFlag;
    public abstract List<Action> GetOnEnterBehaviours(params object[] parameters);
    public abstract List<Action> GetTickBehaviours(params object[] parameters);
    public abstract List<Action> GetOnExitBehaviours(params object[] parameters);
}

public sealed class ChaseStateL : StateL
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
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);
        List<Action> behaviours = new List<Action>();
        behaviours.Add(() =>
        {
            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        });
        behaviours.Add(() =>
        {
            Debug.Log("Whistle!");
        });
        behaviours.Add(() =>
        {
            if (Vector3.Distance(TargetTransform.position , OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetReach);
            }
            else if(Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetLost);
            }
        });

        return behaviours;
    }

}

public sealed class PatrolStateL : StateL
{
    private Transform actualTrget;

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

        List<Action> behaviours = new List<Action>();
        behaviours.Add(() => 
        {
            if (actualTrget == null)
            {
                actualTrget = wayPoint1;
            }

            if (Vector3.Distance(ownerTransform.position, actualTrget.position) < 0.2f)
            {
                if (actualTrget == wayPoint1)
                    actualTrget = wayPoint2;
                else
                    actualTrget = wayPoint1;
            }

            ownerTransform.position += (actualTrget.position - ownerTransform.position).normalized * speed * Time.deltaTime;

        });

        behaviours.Add(() =>
        {
            if (Vector3.Distance(ownerTransform.position, chaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetNear);
            }
        });
        return behaviours;
    }

}

public sealed class ExplodeStateL : StateL
{
    public override List<Action> GetOnEnterBehaviours(params object[] parameters)
    {
        List<Action> behaviours = new List<Action>();
        behaviours.Add(() => { Debug.Log("boom"); });

        return behaviours;
    }

    public override List<Action> GetOnExitBehaviours(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickBehaviours(params object[] parameters)
    {

        List<Action> behaviours = new List<Action>();
        behaviours.Add(() => { Debug.Log("F"); });

        return behaviours;
    }

}
