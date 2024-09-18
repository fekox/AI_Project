using UnityEngine;

public sealed class CaravanWaitState : State
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
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
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
        Transform ownerTransform = parameters[0] as Transform;
        Transform mineTramsform = parameters[1] as Transform;
        Caravan caravan = (Caravan)parameters[2];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!caravan.isTargetReach)
            {
                ownerTransform.position += (mineTramsform.position - ownerTransform.position).normalized * caravan.speed * Time.deltaTime;
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(mineTramsform.position, ownerTransform.position) < caravan.reachDistance)
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
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
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
        Transform ownerTransform = parameters[0] as Transform;
        Transform homeTramsform = parameters[1] as Transform;
        Caravan caravan = (Caravan)parameters[2];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!caravan.isTargetReach)
            {
                ownerTransform.position += (homeTramsform.position - ownerTransform.position).normalized * caravan.speed * Time.deltaTime;
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(homeTramsform.position, ownerTransform.position) < caravan.reachDistance)
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