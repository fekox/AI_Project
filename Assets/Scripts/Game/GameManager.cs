using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Mine")]
    public Mine mine;

    [Header("Mine: Food")]
    [SerializeField] private int maxFoodOnMine;
    [SerializeField] private int currentFoodOnMine;

    [Header("Mine: Gold")]
    [SerializeField] private int maxGoldOnMine;
    [SerializeField] private int currentGoldOnMine;

    [Header("Miner")]
    public Miner miner;

    [Header("Miner: Movement")]
    public Transform minerTransform;
    public Transform target;
    public Transform home;

    [SerializeField] private float speed;
    [SerializeField] private float reachDistance;
    [SerializeField] private bool isTargetReach;
    [SerializeField] private bool startLoop;

    [Header("Miner: Resources")]
    [SerializeField] private int maxGoldToCharge;
    [SerializeField] private int currentGold;
    [SerializeField] private bool isMinerFull;

    [Header("Miner: Food")]
    [SerializeField] private int maxFoodToCharge;
    [SerializeField] private int currentFood;
    [SerializeField] private bool isFoodFull;

    [Header("Miner: Time")]
    [SerializeField] private float miningTime;
    [SerializeField] private float eatingTime;

    private void Start()
    {
        InitMine();
        InitMiner();
    }

    void Update()
    {
        //Mine
        currentFoodOnMine = mine.GetCurrentFood();

        currentGoldOnMine = mine.GetCurrentGold();

        //Miner
        isTargetReach = miner.isTargetReach;
        isMinerFull = miner.isMinerFull;
        isFoodFull = miner.isFoodFull;

        currentGold = miner.GetCurrentGold();
        currentFood = miner.GetCurrentFood();
    }

    public void InitMiner()
    {
        miner = new Miner(speed, reachDistance, isTargetReach, startLoop,
                          currentGold, maxGoldToCharge, miningTime, isMinerFull,
                          currentFood, maxFoodToCharge, eatingTime, isFoodFull);
    }

    public void InitMine()
    {
        mine = new Mine(currentGold, maxGoldOnMine, currentFoodOnMine, maxFoodOnMine);
    }

    public void StartChace()
    {
        miner.startLoop = true;
    }

    public Miner GetMiner() 
    {
        return miner;
    }

    public Mine GetMine()
    {
        return mine;
    }
}