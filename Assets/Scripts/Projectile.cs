using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float selfDestructTime = 5f;
    public float damage;
    public string otherPlayerWall;
    public string wall;
    public string playerLayer;
    public string snakeTag;

    private int playerLayerIndex;
    private void Start()
    {
        playerLayerIndex = LayerMask.NameToLayer(playerLayer);
        Destroy(gameObject, selfDestructTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(otherPlayerWall))
        {
            Destroy(collision.gameObject);

            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag(wall))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag(snakeTag))
        {
            if (collision.gameObject.layer != playerLayerIndex)
            {
                MovementController hitPlayer = collision.gameObject.GetComponent<MovementController>();
                hitPlayer.health -= damage;                
                Destroy(gameObject);
            }            
        }
    }
}
