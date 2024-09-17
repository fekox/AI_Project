public class Mine
{
    private int currentFood;
    private int maxFood = 10;

    private int currentGold;
    private int maxGold = 30;

    public Mine(int currentGold, int maxGold, int currentFood, int maxFood) 
    {
        this.currentGold = currentGold;
        this.maxFood = maxFood;

        this.currentGold = this.maxGold;

        this.currentFood = currentFood;
        this.maxFood = maxFood;

        this.currentFood = this.maxFood;
    }

    public int GetCurrentFood() 
    {
        return currentFood;
    }

    public int GetMaxFood() 
    {
        return maxFood;
    }

    public void RemoveFood(int number) 
    {
        currentFood -= number;
    }

    public void AddFood(int number) 
    {
        currentFood += number;
    }

    public int GetCurrentGold() 
    {
        return currentGold;
    }

    public int GetMaxGold() 
    {
        return maxGold;
    }

    public void RemoveGold(int number) 
    {
        currentGold -= number;
    }
}
