using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodCollector : MonoBehaviour
{
    [Header("Game Stuff")]    
    public string foodTag; 
    public MovementController player;         
    public string snakeTag;
    public MovementController controllingPlayer;
    public float nodeCountMultipler = 150f;

    [Header("Collector Stats")]
    public float minRange;
    public float maxRange;
    public float localFoodNodesNum;
    public float timerDelay;
    public float foodToCollect;
    public float maxFood;

    [Header("UI")]
    public TMP_Text localFoodNodesTextP1;
    public TMP_Text foodToCollectTextP1;
    public TMP_Text localFoodNodesTextP2;
    public TMP_Text foodToCollectTextP2;

    [Header("Lists")]
    public List<FoodNode> allFoodNodes = new List<FoodNode>();
    public List<FoodNode> localFoodNodes = new List<FoodNode>();

    private float timer;
    private MovementController snakeCollsion;
    private void Start()
    {
        FoodNode[] _allFoodNodes = FindObjectsOfType<FoodNode>();
        allFoodNodes.AddRange(_allFoodNodes);
        CheckRange();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerDelay)
        {
            timer = 0;            
            CollectFood();
            UpdateUI();
        }        
    }
    public void UpdateUI()
    {
        localFoodNodesTextP1.text = "Nodes: " + localFoodNodesNum.ToString();
        foodToCollectTextP1.text = foodToCollect.ToString();
        localFoodNodesTextP2.text = "Nodes: " + localFoodNodesNum.ToString();
        foodToCollectTextP2.text = foodToCollect.ToString();
    }
    public void CheckRange()
    {
        foreach(FoodNode foodNode in allFoodNodes)
        {
            float distance = Vector3.Distance(transform.position, foodNode.transform.position);
            if ((distance >= minRange && distance <= maxRange) && (!localFoodNodes.Contains(foodNode)))
            {
                localFoodNodes.Add(foodNode);
            }
        }
        localFoodNodesNum = localFoodNodes.Count;
        maxFood = localFoodNodesNum * nodeCountMultipler;
    }
    public void CollectFood()
    {
        foreach(FoodNode foodNode in localFoodNodes)
        {
            foodNode.underCollector = true;
            if (foodToCollect < maxFood )
            {
                foodToCollect += foodNode.foodStored;
                foodNode.foodStored = 0;
            }
            else if (foodToCollect >= maxFood)
            {
                foodNode.foodStored = 0;
                foodToCollect = maxFood;
            }
                     
        }
    }
    public void SnakeCollectFood()
    {
        controllingPlayer.food += foodToCollect;
        foodToCollect = 0;
        UpdateUI();
        controllingPlayer.UpdateUI();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(snakeTag))
        {
            snakeCollsion = collision.gameObject.GetComponent<MovementController>();           
            if (snakeCollsion != controllingPlayer)
            {
                controllingPlayer = snakeCollsion;
                SnakeCollectFood();
                Destroy(gameObject);

            }
            if (snakeCollsion == controllingPlayer)
            {
                SnakeCollectFood();
            }
            

        }
        
    }
}
