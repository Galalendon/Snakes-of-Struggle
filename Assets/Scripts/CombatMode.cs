using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMode : MonoBehaviour
{
    [Header("Player Stuff")]
    public MovementController player;
    public int layerIndex;

    [Header("Combat Stuff")]
    public GameObject projectilePrefab;
    public GameObject minePrefab;
   

    [Header("Combat Mods")]
    public float projectileOffset;
    public float projectileSpeed = 10f;
    public float projectileGoldCost;

    [Header("Combat Juice")]
    public AudioSource audioSource;
    public AudioClip projectileFire;

    private string playerLayerName;
    private void Start()
    {
        player = GetComponentInParent<MovementController>();
        layerIndex = player.gameObject.layer;
        playerLayerName = LayerMask.LayerToName(layerIndex);
    }

    public void FireProjectile()
    {
        // Instantiate the projectile at the position and rotation of this GameObject
        GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        projectileScript.playerLayer = playerLayerName;
        audioSource.clip = projectileFire;
        audioSource.Play();
        player.gold -= projectileGoldCost;
        // Assuming the projectile has a Rigidbody component
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Set the velocity of the projectile
            rb.velocity = transform.right * projectileSpeed; 
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Rigidbody2D component.");
        }
    }
    public void SpawnMine()
    {
        GameObject newMine = Instantiate(minePrefab, transform.position, transform.rotation);
        Mine newMineScript = newMine.GetComponent<Mine>();
        newMineScript.player = player;
    }
}
