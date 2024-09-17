public class Miner
{
    //Movement
    public float speed;
    public float reachDistance;
    public bool isTargetReach = true;
    public bool startLoop = false;

    //Inventory
    public int currentGold = 0;
    public int maxGoldToCharge = 15;

    public int currentFood = 0;
    public int maxFood = 3;

    public float miningTime = 0.5f;
    public float eatingTime = 0.5f;

    public bool isMinerFull = true;
    public bool isFoodFull = true;

    public Miner(float speed, float reachDistance, bool startLoop,
        bool isTargetReach, int currentGold, int maxGoldToCharge, float miningTime, bool isMinerFull,
        int currentFood, int maxFood, float eatingTime, bool isFoodFull)
    {
        //Movement
        this.speed = speed;
        this.reachDistance = reachDistance;
        this.isTargetReach = isTargetReach;
        this.startLoop = startLoop;

        //Mining
        this.currentGold = currentGold;
        this.maxGoldToCharge = maxGoldToCharge;
        this.miningTime = miningTime;
        this.isMinerFull = isMinerFull;

        //Eating
        this.currentFood = currentFood;
        this.maxFood = maxFood;
        this.eatingTime = eatingTime;
        this.isFoodFull = isFoodFull;
    }

    public void SetCurrentGold(int newCurrentGold)
    {
        currentGold = newCurrentGold;
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    public int GetMaxGoldToCharge()
    {
        return maxGoldToCharge;
    }

    public void AddGold(int addGold)
    {
        currentGold += addGold;
    }

    public void RemoveGold(int removeGold)
    {
        currentGold -= removeGold;
    }

    public float GetMiningTime()
    {
        return miningTime;
    }

    public int GetCurrentFood()
    {
        return currentFood;
    }

    public int GetMaxFoodToCharge()
    {
        return maxFood;
    }

    public void AddFood(int addFood)
    {
        currentFood += addFood;
    }

    public void RemoveFood(int removeFood)
    {
        currentFood -= removeFood;
    }

    public float GetEatingTime()
    {
        return eatingTime;
    }
}