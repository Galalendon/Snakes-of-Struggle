using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodNode : MonoBehaviour
{
    [Header("Game Stuff")]
    public GameController gameController;
    public MovementController controllingPlayer;
    public string snakeTag;
    public MovementController player1;
    public MovementController player2;

    [Header("Node Stats")]
    public float foodIncrement;
    public float foodIncrementTime;
    public float foodStored;    
    public bool underCollector;
    public float maxFood;

    [Header("UI")]
    public TMP_Text foodTextP1;
    public TMP_Text foodTextP2;

    private MovementController snakeCollsion;
    private SpriteRenderer spriteRenderer;
    private float timer;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        player1 = gameController.player1;
        player2 = gameController.player2;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= foodIncrementTime)
        {
            StoreFood();
            timer = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(snakeTag))
        {
            snakeCollsion = collision.gameObject.GetComponent<MovementController>();
            if (controllingPlayer == null)
            {
                controllingPlayer = snakeCollsion;
                FoodPickup();
            }
            if (snakeCollsion != controllingPlayer)
            {
                controllingPlayer = snakeCollsion;
                FoodPickup();
            }
            if (snakeCollsion == controllingPlayer)
            {
                FoodPickup();
            }

        }
        
    }

    public void FoodPickup()
    {
        controllingPlayer.food += foodStored;
        foodStored = 0;
        UpdateUI();
    }
    public void StoreFood()
    {
        if (foodStored >= maxFood)
        {
            foodStored = maxFood;
            UpdateUI();
        }
        else if (foodStored < maxFood)
        {
            foodStored += foodIncrement;
            UpdateUI();
        }
        
    }
    public void UpdateUI()
    {        
        if (underCollector == true)
        {
            foodTextP1.text = "$";
            foodTextP2.text = "$";
        }
        else if (underCollector != true)
        {
            foodTextP1.text = foodStored.ToString();
            foodTextP2.text = foodStored.ToString();
        }
        
    }
    

}
