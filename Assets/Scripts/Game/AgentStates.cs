using System.Collections.Generic;
using UnityEngine;

public sealed class WaitState : State
{
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        GrapfView grapfView = (GrapfView)parameters[0];
        Transform ownerTransform = (Transform)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            ownerTransform.position = new Vector3(grapfView.GetStartNode().GetCoordinate().x, grapfView.GetStartNode().GetCoordinate().y, 0);
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
        Agent agent = (Agent)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": Waiting...");
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (agent.GetAgentType() == AgentType.Caravan && mine.GetCurrentFood() <= 2)
            {
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.IsStartLoop())
            {
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }
        });

        return behaviours;
    }
}

public sealed class WalkState : State
{
    private int currentPos = 0;

    private GrapfView grapfView;
    private List<Node<Vector2>> path;
    private Pathfinder<Node<Vector2>> pathfinder;
    private Transform ownerTransform;

    private Agent agent;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        grapfView = (GrapfView)parameters[0];
        path = (List<Node<Vector2>>)parameters[1];
        pathfinder = (Pathfinder<Node<Vector2>>)parameters[2];
        agent = (Agent)parameters[3];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            currentPos = 0;

            if (agent.IsOnHome()) 
            {
                path = pathfinder.FindPath(grapfView.GetStartNode(), grapfView.GetOneMine(0), grapfView.grapf.nodes);
                agent.SetIsOnHome(false);

                Debug.Log(agent.GetAgentType() + ": Start walk to mine");
            }

            if (agent.IsOnMine()) 
            {
                path = pathfinder.FindPath(grapfView.GetOneMine(0), grapfView.GetStartNode(), grapfView.grapf.nodes);
                agent.SetIsOnMine(false);

                Debug.Log(agent.GetAgentType() + ": Start walk to home");
            }

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
        ownerTransform = (Transform)parameters[0];
        agent = (Agent)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!agent.IsTargetReach())
            {
                if (Vector2.Distance(ownerTransform.position, new Vector2(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y)) < agent.GetReachDistance())
                {
                    currentPos++;
                }

                else
                {
                    ownerTransform.position += (new Vector3(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y, 0f) - ownerTransform.position).normalized
                                               * agent.GetSpeed() * Time.deltaTime;
                }
            }
        });

        //TODO: Change the GetOneMine for Voronoid.
        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector2.Distance(grapfView.GetOneMine(0).GetCoordinate(), ownerTransform.position) < agent.GetReachDistance())
            {
                agent.SetIsTargetReach(true);
                agent.SetIsOnMine(true);
                OnFlag?.Invoke(Flags.OnReachMine);
            }

            if (Vector2.Distance(grapfView.GetStartNode().GetCoordinate(), ownerTransform.position) < agent.GetReachDistance())
            {
                agent.SetIsTargetReach(true);
                agent.SetIsOnHome(true);
                OnFlag?.Invoke(Flags.OnReachHome);
            }
        });

        return behaviours;
    }
}

public sealed class DeliverState : State
{
    float timer = 0;

    private Agent agent;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        agent = (Agent)parameters[0];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": Start Deliver");
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
        agent = (Agent)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            timer += Time.deltaTime;

            if (agent.GetAgentType() == AgentType.Caravan && timer >= agent.GetDeliveringTime())
            {
                if (agent.GetCurrentFood() <= agent.GetMaxFood() && agent.GetCurrentFood() > 0 &&
                    mine.GetCurrentFood() <= mine.GetMaxFood() && mine.GetCurrentFood() > 0)
                {
                    agent.RemoveFood(1);
                    mine.AddFood(1);

                    Debug.Log(agent.GetAgentType() + " food: " + agent.GetCurrentFood());
                }

                timer = 0;
            }

            if (agent.GetAgentType() == AgentType.Miner && timer >= agent.GetMiningTime())
            {
                if (agent.GetCurrentGold() <= agent.GetMaxGold() && agent.GetCurrentGold() > 0 &&
                    mine.GetCurrentFood() <= mine.GetMaxFood() && mine.GetCurrentFood() > 0)
                {
                    agent.RemoveGold(1);

                    Debug.Log(agent.GetAgentType() + " gold: " + agent.GetCurrentGold());
                }

                timer = 0;
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (agent.GetAgentType() == AgentType.Caravan && agent.GetCurrentFood() <= 0) 
            {
                agent.SetIsFoodFull(false);
                agent.SetIsTargetReach(false);
                OnFlag?.Invoke(Flags.OnFoodEmpty);
            }

            if (agent.GetAgentType() == AgentType.Caravan && mine.GetCurrentFood() >= mine.GetMaxFood())
            {
                agent.SetIsFoodFull(false);
                agent.SetIsTargetReach(false);
                OnFlag?.Invoke(Flags.OnFoodEmpty);
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.GetCurrentGold() <= 0)
            {
                agent.SetIsGoldFull(false);
                agent.SetIsTargetReach(false);
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }
        });

        return behaviours;
    }
}

public sealed class GatherState : State
{
    float timer = 0;

    private Agent agent;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        agent = (Agent)parameters[0];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": Start gather");
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
        agent = (Agent)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (agent.GetAgentType() == AgentType.Caravan && !agent.IsFoodFull())
            {
                timer += Time.deltaTime;

                if (timer >= agent.GetDeliveringTime())
                {
                    if (agent.GetCurrentFood() <= agent.GetMaxFood())
                    {
                        agent.AddFood(1);

                        Debug.Log(agent.GetAgentType() + " food: " + agent.GetCurrentFood());
                    }

                    timer = 0;
                }
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.IsFoodFull())
            {
                timer += Time.deltaTime;

                if (timer >= agent.GetMiningTime())
                {
                    if (agent.GetCurrentGold() != agent.GetMaxGold() && mine.GetCurrentGold() > 0)
                    {
                        agent.AddGold(1);
                        mine.RemoveGold(1);

                        Debug.Log(agent.GetAgentType() + " gold: " + agent.GetCurrentGold());
                    }

                    if (agent.GetCurrentGold() % 3 == 0)
                    {
                        agent.RemoveFood(1);

                        Debug.Log(agent.GetAgentType() + " food: " + agent.GetCurrentFood());
                    }

                    timer = 0;
                }
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (agent.GetAgentType() == AgentType.Caravan && agent.GetCurrentFood() == agent.GetMaxFood())
            {
                agent.SetIsTargetReach(false);
                agent.SetIsFoodFull(true);
                OnFlag?.Invoke(Flags.OnFoodFull);
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.GetCurrentGold() == agent.GetMaxGold())
            {
                agent.SetIsTargetReach(false);
                agent.SetIsGoldFull(true);
                OnFlag?.Invoke(Flags.OnGoldFull);
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.GetCurrentFood() <= 0)
            {
                agent.SetIsFoodFull(false);
                OnFlag?.Invoke(Flags.OnHunger);
            }

            if (agent.GetAgentType() == AgentType.Miner && mine.GetCurrentGold() <= 0)
            {
                OnFlag?.Invoke(Flags.OnNoGoldOnMine);
            }
        });

        return behaviours;
    }
}

public sealed class EatingState : State
{
    float timer = 0;

    private Agent agent;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();
        agent = (Agent)parameters[0];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": eating");
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
        agent = (Agent)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if(agent.GetAgentType() == AgentType.Miner) 
            {
                timer += Time.deltaTime;

                if (timer >= agent.GetEatingTime())
                {
                    if (agent.GetCurrentFood() <= agent.GetMaxFood() && mine.GetCurrentFood() > 0)
                    {
                        mine.RemoveFood(1);
                        agent.AddFood(1);

                        Debug.Log(agent.GetAgentType() + " food: " + agent.GetCurrentFood());
                    }

                    timer = 0;
                }
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (agent.GetAgentType() == AgentType.Miner && agent.GetCurrentFood() >= agent.GetMaxFood())
            {
                agent.SetIsFoodFull(true);
                OnFlag?.Invoke(Flags.OnFoodFull);
            }

            if (agent.GetAgentType() == AgentType.Miner && agent.GetCurrentFood() <= 0 && mine.GetCurrentFood() <= 0)
            {
                OnFlag?.Invoke(Flags.OnNoFoodOnMine);
            }
        });

        return behaviours;
    }
}

public sealed class WaitingForFoodState : State
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
        BehavioursActions behaviours = new BehavioursActions();
        Mine mine = (Mine)parameters[0];
        Agent agent = (Agent)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": No food on mine, waiting for food");
        });


        behaviours.SetTransitionBehaviour(() =>
        {
            if (mine.GetCurrentFood() == mine.GetMaxFood())
            {
                OnFlag?.Invoke(Flags.OnFoodFull);
            }
        });

        return behaviours;
    }
}

public sealed class WaitingForGoldState : State
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
        BehavioursActions behaviours = new BehavioursActions();
        Mine mine = (Mine)parameters[0];
        Agent agent = (Agent)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            Debug.Log(agent.GetAgentType() + ": No gold on mine waiting for gold");
        });


        behaviours.SetTransitionBehaviour(() =>
        {
            if (mine.GetCurrentGold() == mine.GetMaxGold())
            {
                OnFlag?.Invoke(Flags.OnGoToNewTarget);
            }
        });

        return behaviours;
    }
}