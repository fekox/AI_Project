using System;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class FSM
{
    private const int UNNASSIGNED_TRASITION = -1;

    public int currentState = 0;

    private Dictionary<int, State> behaviour;

    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;

    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviour = new Dictionary<int, State>();
        transitions = new int[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASSIGNED_TRASITION;
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTickParameters = null, 
      Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new() 
    {
        if (!behaviour.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            newBehaviour.OnFlag += Transition;
            behaviour.Add(stateIndex, newBehaviour);
            behaviourTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

    public void ForceTransition(int state) 
    {
        currentState = state;
    }

    public void SetTransition(int originState, int flag, int destinationState) 
    {
        transitions[originState, flag] = destinationState;
    }

    public void Transition(int flag) 
    {
        if (transitions[currentState, flag] != UNNASSIGNED_TRASITION)
        {
            foreach (Action behaviour in behaviour[currentState].
                GetTickBehaviours(behaviourOnExitParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }

            currentState = transitions[currentState, flag];

            foreach (Action behaviour in behaviour[currentState].
                GetTickBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }

    public void Tick() 
    {
        if (behaviour.ContainsKey(currentState))
        {
            foreach (Action behaviour in behaviour[currentState].
                GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke())) 
            {
                behaviour?.Invoke();
            }
        }
    }
}
