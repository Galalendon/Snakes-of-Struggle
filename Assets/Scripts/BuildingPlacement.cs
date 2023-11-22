using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacement : MonoBehaviour
{
    [Header("Player")]
    public MovementController player;

    [Header("Buildings")]
    public GameObject foodCollector;
    public GameObject stoneCollector;
    public GameObject goldCollector;
    public GameObject flag;
    public List<GameObject> flags = new List<GameObject>();
    public GameObject wall;

    [Header("Building Costs")]
    public float foodCollectorStoneCost;
    public float stoneCollectorStoneCost;
    public float goldCollectorStoneCost;
    public float flagStoneCost;
    public float flagGoldCost;
    public float wallStoneCost;

    [Header("Building Collection Ranges")]
    public float foodCollectorRange;
    public float stoneCollectorRange;
    public float goldCollectorRange;

    [Header("Building Placement Radius")]
    public float buildingRange;

    public void Start()
    {
        player = GetComponentInParent<MovementController>();
    }
    public void PlaceFoodCollector()
    {
        if (player.stone >= foodCollectorStoneCost)
        {
            if (!CanBuildBuilding(transform.position))
            {
                GameObject newfoodCollector = Instantiate(foodCollector, transform.position, Quaternion.identity);
                FoodCollector newFoodCollectorScript = newfoodCollector.GetComponent<FoodCollector>();
                newFoodCollectorScript.controllingPlayer = player;
                player.stone -= foodCollectorStoneCost;
                player.UpdateUI();
            }
            else
            {
                Debug.Log("Cannot place Farm here. Too close to another building.");
            }

        }
        else if (player.stone < foodCollectorStoneCost)
        {
            Debug.Log("Not enough stone");
        }
      
    }
    public void PlaceStoneCollector()
    {
        if (player.stone >= stoneCollectorStoneCost)
        {
            if (!CanBuildBuilding(transform.position))
            {
                GameObject newStoneCollector = Instantiate(stoneCollector, transform.position, Quaternion.identity);
                StoneCollector newStoneCollectorScript = newStoneCollector.GetComponent<StoneCollector>();
                newStoneCollectorScript.controllingPlayer = player;
                player.stone -= stoneCollectorStoneCost;
                player.UpdateUI();
            }
            else
            {
                Debug.Log("Cannot place Quarry here. Too close to another building.");
            }

        }
        else if (player.stone < foodCollectorStoneCost)
        {
            Debug.Log("Not enough stone");
        }

    }
    public void PlaceGoldCollector()
    {
        if (player.stone >= goldCollectorStoneCost)
        {
            if (!CanBuildBuilding(transform.position))
            {
                GameObject newGoldCollector = Instantiate(goldCollector, transform.position, Quaternion.identity);
                GoldCollector newGoldCollectorScript = newGoldCollector.GetComponent<GoldCollector>();
                newGoldCollectorScript.controllingPlayer = player;
                player.stone -= goldCollectorStoneCost;
                player.UpdateUI();
            }
            else
            {
                Debug.Log("Cannot place Mine here. Too close to another building.");
            }
        }
        else if (player.stone < goldCollectorStoneCost)
        {
            Debug.Log("Not enough stone");
        }
        
    }
    public void PlaceFlag()
    {
        if ((player.stone >= flagStoneCost) && (player.gold >= flagGoldCost))
        {
            if (!CanBuildBuilding(transform.position))
            {
                GameObject newFlag = Instantiate(flag, transform.position, Quaternion.identity);
                Flag newFlagScript = newFlag.GetComponent<Flag>();
                flags.Add(newFlag);
                newFlagScript.player = player;               
                player.stone -= flagStoneCost;
                player.gold -= flagGoldCost;
                player.UpdateUI();
            }
            else
            {
                Debug.Log("Cannot place flag here. Too close to another building.");
            }
        }
        else if (player.stone < flagStoneCost)
        {
            Debug.Log("Not enough stone");
        }
        else if (player.gold < flagGoldCost)
        {
            Debug.Log("Not enough gold");
        }
        
    }
    public void PlaceWall()
    {
        if (player.stone >= wallStoneCost)
        {
            if (!CanSpawnWall(transform.position))
            {
                Instantiate(wall, transform.position, Quaternion.identity);
                player.stone -= wallStoneCost;
            }
            else
            {
                Debug.Log("Cannot place wall here");
            }
        }
        else if (player.stone < wallStoneCost)
        {
            Debug.Log("Not enough stone");
        }
        
    }
    bool CanSpawnWall(Vector2 position)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, player.wallCheckDistance);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.layer == LayerMask.NameToLayer(player.wallLayer))
            {
                return true; // There's already a wall here
            }
        }
        return false;
    }
    bool CanBuildBuilding(Vector2 position)
    {
        Collider2D[] hitCollliders = Physics2D.OverlapCircleAll(position, player.buildingCheckDistance);

        foreach (var hitCollider in hitCollliders)
        {
            if (hitCollider.gameObject.CompareTag(player.buildingTag))
            {
                return true;
            }
        }
        return false;
    }
}

