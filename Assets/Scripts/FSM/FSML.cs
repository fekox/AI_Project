using System;
using System.Collections.Generic;

public class FSML
{
    private const int UNNASSIGNED_TRANSITION = -1;
    public int currentState = 0;
    private Dictionary<int, State> behaviours;
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private int[,] transitions;

    public FSML(int states, int flags)
    {
        behaviours = new Dictionary<int, State>();
        transitions = new int[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASSIGNED_TRANSITION;
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTickParameters = null,
        Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        if (!behaviours.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            newBehaviour.OnFlag += Transition;
            behaviours.Add(stateIndex, newBehaviour);
            behaviourTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

    public void ForceState(int state) 
    {
        currentState = state;
    }

    public void SetTransition(int originState, int flag, int destinationState)
    {
        transitions[originState, flag] = destinationState;
    }

    public void Transition(int flag)
    {
        if (transitions[currentState, flag] != UNNASSIGNED_TRANSITION)
        {
            foreach (Action behaviour in behaviours[currentState].
                GetOnExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }

            currentState = transitions[currentState, flag];
            
            foreach (Action behaviour in behaviours[currentState].
                GetOnEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }

        }
    }

    public void Tick()
    {
        if (behaviours.ContainsKey(currentState))
        {
            foreach (Action behaviour in behaviours[currentState].
                GetTickBehaviours(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }
}
