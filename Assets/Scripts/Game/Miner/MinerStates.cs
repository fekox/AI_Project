using UnityEngine;

public sealed class MinerWaitState : State
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
        Miner miner = (Miner)parameters[0];

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Waiting...");
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (miner.startLoop)
            {
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }
        });

        return behaviours;
    }
}

public sealed class MinerGoToMineState : State
{
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Go to mine");
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
        Miner miner = (Miner)parameters[2];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!miner.isTargetReach)
            {
                ownerTransform.position += (mineTramsform.position - ownerTransform.position).normalized * miner.speed * Time.deltaTime;
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(mineTramsform.position, ownerTransform.position) < miner.reachDistance)
            {
                miner.isTargetReach = true;
                OnFlag?.Invoke(Flags.OnReachMine);
            }
        });

        return behaviours;
    }
}

public sealed class MinerMiningState : State
{
    float timer = 0;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Start Mining");
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
        Miner miner = (Miner)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (miner.isFoodFull)
            {
                timer += Time.deltaTime;

                if (timer >= miner.GetMiningTime())
                {
                    if (miner.GetCurrentGold() != miner.GetMaxGoldToCharge() && mine.GetCurrentGold() > 0)
                    {
                        miner.AddGold(1);
                        mine.RemoveGold(1);
                    }

                    if (miner.GetCurrentGold() % 3 == 0)
                    {
                        miner.RemoveFood(1);
                    }

                    timer = 0;
                }

                Debug.Log("Gold: " + miner.GetCurrentGold());
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (miner.GetCurrentGold() == miner.GetMaxGoldToCharge())
            {
                miner.isTargetReach = false;
                miner.isMinerFull = true;
                OnFlag?.Invoke(Flags.OnGoldFull);
            }

            if (miner.GetCurrentFood() == 0)
            {
                miner.isFoodFull = false;
                OnFlag?.Invoke(Flags.OnHunger);
            }

            if (mine.GetCurrentGold() == 0)
            {
                OnFlag?.Invoke(Flags.OnNoGoldOnMine);
            }
        });

        return behaviours;
    }
}

public sealed class MinerEatingState : State
{
    float timer = 0;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Eating");
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
        Miner miner = (Miner)parameters[0];
        Mine mine = (Mine)parameters[1];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            timer += Time.deltaTime;

            if (timer >= miner.GetEatingTime())
            {
                if (miner.GetCurrentFood() != miner.GetMaxFoodToCharge())
                {
                    if (mine.GetCurrentFood() > 0)
                    {
                        mine.RemoveFood(1);
                        miner.AddFood(1);
                    }
                }

                timer = 0;
            }

            Debug.Log("Food: " + miner.GetCurrentFood());
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (miner.GetCurrentFood() == miner.GetMaxFoodToCharge())
            {
                miner.isFoodFull = true;
                OnFlag?.Invoke(Flags.OnFoodFull);
            }

            if (miner.GetCurrentFood() == 0 && mine.GetCurrentFood() == 0)
            {
                OnFlag?.Invoke(Flags.OnNoFoodOnMine);
            }
        });

        return behaviours;
    }
}

public sealed class MinerWaitingForFoodState : State
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

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            Debug.Log("No food on mine: Waiting for food");
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

public sealed class MinerWaitingForGoldState : State
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

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            Debug.Log("No gold on mine: Waiting for gold");
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

            behaviours.AddMainThreadBehaviours(0, () =>
            {
                Debug.Log("No food on mine: Waiting for food");
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
}

public sealed class MinerGoToHomeState : State
{
    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Go to home");
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
        Miner miner = (Miner)parameters[2];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            if (!miner.isTargetReach)
            {
                ownerTransform.position += (homeTramsform.position - ownerTransform.position).normalized * miner.speed * Time.deltaTime;
            }
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(homeTramsform.position, ownerTransform.position) < miner.reachDistance)
            {
                miner.isTargetReach = true;
                OnFlag?.Invoke(Flags.OnReachHome);
            }
        });

        return behaviours;
    }
}

public sealed class MinerDeliverState : State
{
    float timer = 0;

    public override BehavioursActions GetOnEnterBehaviours(params object[] parameters)
    {
        BehavioursActions behaviours = new BehavioursActions();

        behaviours.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Start Deliver");
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
        Miner miner = (Miner)parameters[0];

        behaviours.AddMainThreadBehaviours(0, () =>
        {
            timer += Time.deltaTime;

            if (timer >= miner.GetMiningTime())
            {
                if (miner.GetCurrentGold() <= miner.GetMaxGoldToCharge() && miner.GetCurrentGold() > 0)
                {
                    miner.AddGold(-1);
                }

                timer = 0;
            }

            Debug.Log("Gold: " + miner.GetCurrentGold());
        });

        behaviours.SetTransitionBehaviour(() =>
        {
            if (miner.GetCurrentGold() == 0)
            {
                miner.isMinerFull = false;
                miner.isTargetReach = false;
                OnFlag?.Invoke(Flags.OnGoToTarget);
            }
        });

        return behaviours;
    }
}