using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class CaravanWaitState : State
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
        Mine mine = (Mine)parameters[0];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Caravan: Waiting...");
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (mine.GetCurrentFood() <= 2)
            {
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }
        });

        return behaviours;
    }
}

public sealed class CaravanGoToMineState : State
{
    private int currentPos = 0;

    private GrapfView grapfView;
    private List<Node<Vector2>> path;
    private Pathfinder<Node<Vector2>> pathfinder;
    private Transform ownerTransform;

    private Caravan caravan;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        grapfView = (GrapfView)parameters[0];
        path = (List<Node<Vector2>>)parameters[1];
        pathfinder = (Pathfinder<Node<Vector2>>)parameters[2];

        behaviours.AddMultitreadableBehaviours(0,() =>
        {
            path = pathfinder.FindPath(grapfView.GetStartNode(), grapfView.GetOneMine(0), grapfView.grapf.nodes);

            Debug.Log("Caravan: Go to mine");
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
        caravan = (Caravan)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!caravan.isTargetReach)
            {
                if (Vector2.Distance(ownerTransform.position, new Vector2(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y)) < caravan.reachDistance)
                {
                    currentPos++;
                }

                else
                {
                    ownerTransform.position += (new Vector3(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y, 0f) - ownerTransform.position).normalized
                                               * caravan.speed * Time.deltaTime;
                }
            }
        });

        //TODO: Change the GetOneMine for Voronoid.
        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector2.Distance(grapfView.GetOneMine(0).GetCoordinate(), ownerTransform.position) < caravan.reachDistance)
            {
                caravan.isTargetReach = true;
                OnFlag?.Invoke(Flags.OnReachMine);
            }
        });

        return behaviours;
    }
}

public sealed class CaravanDeliverState : State
{
    float timer = 0;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Caravan: Start Deliver");
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
        Caravan caravan = (Caravan)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            timer += Time.deltaTime;

            if (timer >= caravan.GetDeliverTime())
            {
                if (caravan.GetCurrentFood() <= caravan.GetMaxFoodToCharge() && caravan.GetCurrentFood() > 0 && 
                    mine.GetCurrentFood() <= mine.GetMaxFood())
                {
                    caravan.RemoveFood(1);
                    mine.AddFood(1);
                }

                timer = 0;
            }

            Debug.Log("Caravan Food: " + caravan.GetCurrentFood());
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (caravan.GetCurrentFood() <= 0)
            {
                caravan.isFoodFull = false;
                caravan.isTargetReach = false;
                OnFlag?.Invoke(Flags.OnFoodEmpty);
            }

            if (mine.GetCurrentFood() >= mine.GetMaxFood())
            {
                caravan.isFoodFull = false;
                caravan.isTargetReach = false;
                OnFlag?.Invoke(Flags.OnFoodEmpty);
            }
        });

        return behaviours;
    }
}

public sealed class CaravanGoToHomeState : State
{
    private int currentPos = 0;

    private GrapfView grapfView;
    private List<Node<Vector2>> path;
    private Pathfinder<Node<Vector2>> pathfinder;
    private Transform ownerTransform;

    private Caravan caravan;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        grapfView = (GrapfView)parameters[0];
        path = (List<Node<Vector2>>)parameters[1];
        pathfinder = (Pathfinder<Node<Vector2>>)parameters[2];

        //TODO: Change the GetOneMine for Voronoid.
        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            path = pathfinder.FindPath(grapfView.GetOneMine(0), grapfView.GetStartNode(), grapfView.grapf.nodes);
            Debug.Log("Caravan: Go to home");
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
        ownerTransform = parameters[0] as Transform;
        caravan = (Caravan)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!caravan.isTargetReach)
            {
                if (Vector2.Distance(ownerTransform.position, new Vector2(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y)) < caravan.reachDistance)
                {
                    currentPos++;
                }

                else
                {
                    ownerTransform.position += (new Vector3(path[currentPos].GetCoordinate().x, path[currentPos].GetCoordinate().y, 0f) - ownerTransform.position).normalized
                                               * caravan.speed * Time.deltaTime;
                }
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector2.Distance(grapfView.GetStartNode().GetCoordinate(), ownerTransform.position) < caravan.reachDistance)
            {
                caravan.isTargetReach = true;
                OnFlag?.Invoke(Flags.OnReachHome);
            }
        });

        return behaviours;
    }
}

public sealed class CaravanCollectFoodState : State
{
    float timer = 0;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Caravan: Start collecting food");
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
        Caravan caravan = (Caravan)parameters[0];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!caravan.isFoodFull)
            {
                timer += Time.deltaTime;

                if (timer >= caravan.GetDeliverTime())
                {
                    if (caravan.GetCurrentFood() != caravan.GetMaxFoodToCharge())
                    {
                        caravan.AddFood(1);
                    }

                    timer = 0;
                }

                Debug.Log("Caravan food: " + caravan.GetCurrentFood());
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (caravan.GetCurrentFood() == caravan.GetMaxFoodToCharge())
            {
                caravan.isTargetReach = false;
                caravan.isFoodFull = true;
                OnFlag?.Invoke(Flags.OnFoodFull);
            }
        });

        return behaviours;
    }
}