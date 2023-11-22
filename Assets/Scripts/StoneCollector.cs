using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StoneCollector : MonoBehaviour
{
    [Header("Game Stuff")]
    public string stoneTag;
    public MovementController player;
    public string snakeTag;
    public MovementController controllingPlayer;
    public float nodeCountMultipler = 150f;

    [Header("Collector Stats")]
    public float minRange;
    public float maxRange;
    public float localStoneNodesNum;
    public float timerDelay;
    public float stoneToCollect;
    public float stoneMax;

    [Header("UI")]
    public TMP_Text localStoneNodesTextP1;
    public TMP_Text stoneToCollectTextP1;
    public TMP_Text localStoneNodesTextP2;
    public TMP_Text stoneToCollectTextP2;

    [Header("Lists")]
    public List<StoneNode> allStoneNodes = new List<StoneNode>();
    public List<StoneNode> localStoneNodes = new List<StoneNode>();

    private float timer;
    private MovementController snakeCollsion;

    private void Start()
    {
        StoneNode[] _allStoneNodes= FindObjectsOfType<StoneNode>();
        allStoneNodes.AddRange(_allStoneNodes);
        CheckRange();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, maxRange);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timerDelay)
        {
            timer = 0;
            CollectStone();
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        localStoneNodesTextP1.text = "Nodes: " + localStoneNodesNum.ToString();
        stoneToCollectTextP1.text = stoneToCollect.ToString();
        localStoneNodesTextP2.text = "Nodes: " + localStoneNodesNum.ToString();
        stoneToCollectTextP2.text = stoneToCollect.ToString();
    }

    public void CheckRange()
    {
        foreach (StoneNode stoneNode in allStoneNodes)
        {
            float distance = Vector3.Distance(transform.position, stoneNode.transform.position);
            if ((distance >= minRange && distance <= maxRange) && (!localStoneNodes.Contains(stoneNode)))
            {
                localStoneNodes.Add(stoneNode);
            }
        }
        localStoneNodesNum = localStoneNodes.Count;
        stoneMax = localStoneNodesNum * nodeCountMultipler;
    }
    public void CollectStone()
    {
        foreach(StoneNode stoneNode in localStoneNodes)
        {
            stoneNode.underCollector = true;
            if (stoneToCollect < stoneMax)
            {
                stoneToCollect += stoneNode.stoneStored;
                stoneNode.stoneStored = 0;
            }
            else if (stoneToCollect >= stoneMax)
            {
                stoneNode.stoneStored = 0;
                stoneToCollect = stoneMax;
            }
            
        }
    }
    public void SnakeCollectStone()
    {
        controllingPlayer.stone += stoneToCollect;
        stoneToCollect = 0;
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
                SnakeCollectStone();
                Destroy(gameObject);
            }
            if (snakeCollsion == controllingPlayer)
            {
                SnakeCollectStone();
            }
        }
        
    }
}
