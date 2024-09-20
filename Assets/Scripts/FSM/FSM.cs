using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FSM<EnumState, EnumFlag>
    where EnumState : Enum
    where EnumFlag : Enum
{
    private const int UNNASSIGNED_TRASITION = -1;

    public int currentState = 0;

    private Dictionary<int, State> behaviour;

    private Dictionary<int, Func<object[]>> behaviourOnTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;

    private (int dertinationState, Action onTransition)[,] transitions;

    ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

    private BehavioursActions GetCurrentStateOnEnterBehaviours => behaviour[currentState].
        GetOnEnterBehaviours(behaviourOnEnterParameters[currentState]?.Invoke());

    private BehavioursActions GetCurrentStateOnExitBehaviours => behaviour[currentState].
    GetOnExitBehaviours(behaviourOnExitParameters[currentState]?.Invoke());

    private BehavioursActions GetCurrentStateOnTickBehaviours => behaviour[currentState].
    GetTickBehaviours(behaviourOnTickParameters[currentState]?.Invoke());

    public FSM()
    {
        int states = Enum.GetValues(typeof(EnumState)).Length;
        int flags = Enum.GetValues(typeof(EnumFlag)).Length;

        behaviour = new Dictionary<int, State>();
        transitions = new (int, Action)[states, flags];

        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = (UNNASSIGNED_TRASITION, null);
            }
        }

        behaviourOnTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(EnumState state, Func<object[]> onTickParameters = null,
      Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        int stateIndex = Convert.ToInt32(state);

        if (!behaviour.ContainsKey(stateIndex))
        {
            State newBehaviour = new T();
            newBehaviour.OnFlag += Transition;
            behaviour.Add(stateIndex, newBehaviour);
            behaviourOnTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

    public void ForceTransition(EnumState state)
    {
        currentState = Convert.ToInt32(state);
        ExecuteBehaviour(GetCurrentStateOnEnterBehaviours);
    }

    public void SetTransition(EnumState originState, EnumFlag flag, EnumState destinationState, Action onTransition = null)
    {
        transitions[Convert.ToInt32(originState), Convert.ToInt32(flag)] = (Convert.ToInt32(destinationState), onTransition);
    }

    public void Transition(Enum flag)
    {
        if (transitions[currentState, Convert.ToInt32(flag)].dertinationState != UNNASSIGNED_TRASITION)
        {
            ExecuteBehaviour(GetCurrentStateOnExitBehaviours);

            transitions[currentState, Convert.ToInt32(flag)].onTransition?.Invoke();

            currentState = transitions[currentState, Convert.ToInt32(flag)].dertinationState;

            ExecuteBehaviour(GetCurrentStateOnEnterBehaviours);
        }
    }

    public void Tick()
    {
        if (behaviour.ContainsKey(currentState))
        {
            ExecuteBehaviour(GetCurrentStateOnTickBehaviours);
        }
    }

    private void ExecuteBehaviour(BehavioursActions behavioursActions)
    {
        if (behavioursActions.Equals(default(BehavioursActions)))
        {
            return;
        }

        int executionOrder = 0;

        while ((behavioursActions.MainThreadBehaviour != null       && behavioursActions.MainThreadBehaviour.Count > 0) || 
               (behavioursActions.MultithreadblesBehavoiurs != null && behavioursActions.MultithreadblesBehavoiurs.Count > 0))
        {
            Task multithreadableBehaviour = new Task(() =>
            {
                if(behavioursActions.MultithreadblesBehavoiurs != null) 
                {
                    if (behavioursActions.MultithreadblesBehavoiurs.ContainsKey(executionOrder))
                    {
                        Parallel.ForEach(behavioursActions.MultithreadblesBehavoiurs[executionOrder], parallelOptions, (behaviour) =>
                        {
                            behaviour?.Invoke();
                        });

                        behavioursActions.MultithreadblesBehavoiurs.TryRemove(executionOrder, out _);
                    }
                }

            });

            multithreadableBehaviour.Start();

            if (behavioursActions.MainThreadBehaviour != null)
            {
                if (behavioursActions.MainThreadBehaviour.ContainsKey(executionOrder))
                {
                    foreach (Action behaviour in behavioursActions.MainThreadBehaviour[executionOrder])
                    {
                        behaviour?.Invoke();
                    }

                    behavioursActions.MainThreadBehaviour.Remove(executionOrder);
                }
            }

            multithreadableBehaviour.Wait();
            executionOrder++;
        }

        behavioursActions.TransitionBehavour?.Invoke();
    }
}