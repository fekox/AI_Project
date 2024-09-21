using UnityEngine;

public class Caravan : MonoBehaviour 
{
    //Movement
    public float speed;
    public float reachDistance;
    public bool isTargetReach = true;
    public bool startLoop = false;

    //Inventory
    public int currentFood = 0;
    public int maxFoodToCharge = 10;
    public bool isFoodFull = true;

    //Timer
    public float deliveringTime = 0.5f;

    public Caravan(float speed, float reachDistance, bool startLoop,
        bool isTargetReach, int currentFood, int maxFoodToCharge, 
        bool isFoodFull, float deliveringTime) 
    {
        //Movement
        this.speed = speed;
        this.reachDistance = reachDistance;
        this.startLoop = startLoop;
        this.isTargetReach = isTargetReach;

        //Food
        this.currentFood = currentFood;
        this.maxFoodToCharge = maxFoodToCharge;
        this.isFoodFull = isFoodFull;

        this.currentFood = this.maxFoodToCharge;
        this.isFoodFull = true;

        //Timer
        this.deliveringTime = deliveringTime;
    }

    public void SetCurrentFood(int number) 
    {
        currentFood = number;
    }

    public int GetCurrentFood() 
    {
        return currentFood;
    }

    public void SetMaxFoodToCharge(int number)
    {
        maxFoodToCharge = number;
    }

    public int GetMaxFoodToCharge() 
    {
        return maxFoodToCharge;
    }

    public void RemoveFood(int number) 
    {
        currentFood -= number;
    }

    public void AddFood(int number) 
    {
        currentFood += number;
    }

    public float GetDeliverTime() 
    {
        return deliveringTime;
    }
}