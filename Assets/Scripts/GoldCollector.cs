using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldCollector : MonoBehaviour
{
    [Header("Game Stuff")]
    public string goldTag; // Tag for identifying gold nodes
    public MovementController player; // Reference to the player
    public string snakeTag; // Tag for identifying the snake
    public MovementController controllingPlayer; // The player who is currently controlling the collector
    public float nodeCountMultipler = 150f;

    [Header("Collector Stats")]
    public float minRange; // Minimum range for collecting gold
    public float maxRange; // Maximum range for collecting gold
    public float localGoldNodesNum; // Number of local gold nodes in range
    public float timerDelay; // Delay between each collection attempt
    public float goldToCollect; // Amount of gold to collect
    public float goldMax;

    [Header("UI")]
    public TMP_Text localGoldNodesTextP1; 
    public TMP_Text goldToCollectTextP1;
    public TMP_Text localGoldNodesTextP2;
    public TMP_Text goldToCollectTextP2;

    [Header("Lists")]
    public List<GoldNode> allGoldNodes = new List<GoldNode>(); // List of all gold nodes in the game
    public List<GoldNode> localGoldNodes = new List<GoldNode>(); // List of gold nodes within range

    private float timer; // Timer for tracking collection attempts
    private MovementController snakeCollision; // To store reference to the snake that collides with the collector

    private void Start()
    {
        GoldNode[] _allGoldNodes = FindObjectsOfType<GoldNode>();
        allGoldNodes.AddRange(_allGoldNodes);
        CheckRange();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerDelay)
        {
            timer = 0;
            CollectGold();
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        localGoldNodesTextP1.text = "Nodes: " + localGoldNodesNum.ToString();
        goldToCollectTextP1.text = goldToCollect.ToString();
        localGoldNodesTextP2.text = "Nodes: " + localGoldNodesNum.ToString();
        goldToCollectTextP2.text = goldToCollect.ToString();
    }

    public void CheckRange()
    {
        foreach (GoldNode goldNode in allGoldNodes)
        {
            float distance = Vector3.Distance(transform.position, goldNode.transform.position);
            if (distance >= minRange && distance <= maxRange && !localGoldNodes.Contains(goldNode))
            {
                localGoldNodes.Add(goldNode);
            }
        }
        localGoldNodesNum = localGoldNodes.Count;
        goldMax = localGoldNodesNum * nodeCountMultipler;
    }

    public void CollectGold()
    {
        foreach (GoldNode goldNode in localGoldNodes)
        {
            goldNode.underCollector = true;
            if (goldToCollect < goldMax)
            {
                goldToCollect += goldNode.goldStored;
                goldNode.goldStored = 0;
            }
            else if (goldToCollect >= goldMax)
            {
                goldNode.goldStored = 0;
                goldToCollect = goldMax;
            }
            
        }
    }

    public void SnakeCollectGold()
    {
        controllingPlayer.gold += goldToCollect;
        goldToCollect = 0;
        UpdateUI();
        controllingPlayer.UpdateUI();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(snakeTag))
        {
            snakeCollision = collision.gameObject.GetComponent<MovementController>();
            if (snakeCollision != controllingPlayer)
            {
                controllingPlayer = snakeCollision;
                SnakeCollectGold();
                Destroy(gameObject);
            }
            else if (snakeCollision == controllingPlayer)
            {
                SnakeCollectGold();
            }
        }
       
    }
}
