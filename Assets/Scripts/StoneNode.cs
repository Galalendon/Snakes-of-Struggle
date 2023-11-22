using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoneNode : MonoBehaviour
{
    [Header("Game Stuff")]
    public GameController gameController;
    public MovementController controllingPlayer;
    public string snakeTag;
    public MovementController player1;
    public MovementController player2;

    [Header("Node Stats")]
    public float stoneIncrement;
    public float stoneIncrementTime;
    public float stoneStored;
    public bool underCollector;
    public float maxStone;

    [Header("UI")]
    public TMP_Text stoneTextP1;
    public TMP_Text stoneTextP2;

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
        if (timer >= stoneIncrementTime)
        {            
            timer = 0;
            StoreStone();
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
                StonePickup();
            }
            if (snakeCollsion != controllingPlayer)
            {
                controllingPlayer = snakeCollsion;
                StonePickup();
            }
            if (snakeCollsion == controllingPlayer)
            {
                StonePickup();
            }

        }
        
    }

    public void StonePickup()
    {
        controllingPlayer.stone += stoneStored;
        stoneStored = 0;
        UpdateUI();

    }
    public void StoreStone()
    {
        if (stoneStored >= maxStone)
        {
            stoneStored = maxStone;
            UpdateUI();
        }
        else if (stoneStored < maxStone)
        {
            stoneStored += stoneIncrement;
            UpdateUI();
        }
        

    }

    public void UpdateUI()
    {
        if (underCollector == true)
        {
            stoneTextP1.text = "$";
            stoneTextP2.text = "$";
        }
        else if (underCollector != true)
        {
            stoneTextP1.text = stoneStored.ToString();
            stoneTextP2.text = stoneStored.ToString();
        }
        
    }
}
