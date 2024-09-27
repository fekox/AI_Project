using UnityEngine;

public class Mine: MonoBehaviour
{
    [Header("Mine: ID")]
    [SerializeField] private int ID = -1;

    [Header("Mine: Food")]
    [SerializeField] private int currentFood = 0;
    [SerializeField] private int maxFood = 10;

    [Header("Mine: Gold")]
    [SerializeField] private int currentGold = 0;
    [SerializeField] private int maxGold = 30;

    private void Start()
    {
        currentFood = maxFood;
        currentGold = maxGold; 
    }

    public int GetID() 
    {
        return ID;
    }

    public void SetID(int number) 
    {
        ID = number;
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