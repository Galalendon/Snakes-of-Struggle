using TMPro;
using UnityEngine;

public class GoldNode : MonoBehaviour
{
    [Header("Game Stuff")]
    public GameController gameController;
    public MovementController controllingPlayer;
    public string snakeTag;
    public MovementController player1;
    public MovementController player2;

    [Header("Node Stats")]
    public float goldIncrement;
    public float goldIncrementTime;
    public float goldStored;
    public bool underCollector;
    public float maxGold;

    [Header("UI")]
    public TMP_Text goldTextP1;
    public TMP_Text goldTextP2;

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
        if (timer >= goldIncrementTime)
        {
            timer = 0;
            StoreGold();
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
                GoldPickup();
            }
            if (snakeCollsion != controllingPlayer)
            {
                controllingPlayer = snakeCollsion;
                GoldPickup();
            }
            if (snakeCollsion == controllingPlayer)
            {
                GoldPickup();
            }

        }
        
    }

    public void GoldPickup()
    {
        controllingPlayer.gold += goldStored;
        goldStored = 0;
        UpdateUI();

    }
    public void StoreGold()
    {
        if (goldStored >= maxGold)
        {
            goldStored = maxGold;
            UpdateUI();
        }
        else if (goldStored <= maxGold)
        {
            goldStored += goldIncrement;
            UpdateUI();
        }       

    }

    public void UpdateUI()
    {
        if (underCollector == true)
        {
            goldTextP1.text = "$";
            goldTextP2.text = "$";
        }
        else if (underCollector != true)
        {
            goldTextP1.text = goldStored.ToString();
            goldTextP2.text = goldStored.ToString();
        }
    }
}
