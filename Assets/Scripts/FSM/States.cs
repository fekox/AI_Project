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
        if (multiThreadablesBehaviours == null)
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