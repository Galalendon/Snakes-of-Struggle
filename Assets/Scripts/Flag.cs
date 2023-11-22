using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Flag : MonoBehaviour
{
    public MovementController player;
    public int radius = 3;
    public Vector3Int flagPOS;
    public Tilemap tilemap;
    private SpriteRenderer spriteRenderer;
    public string snakeTag;
     

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        tilemap = FindObjectOfType<Tilemap>();        
        FlagPlaced();
    }
    public void FlagPlaced()
    {
        flagPOS = tilemap.WorldToCell(transform.position);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int tilePosition = new Vector3Int(flagPOS.x + x, flagPOS.y + y, flagPOS.z);

                if (tilemap.HasTile(tilePosition))
                {
                    tilemap.SetTileFlags(tilePosition, TileFlags.None);
                    tilemap.SetColor(tilePosition, player.playerColour);
                    player.playerTiles.Add(tilePosition);
                }
            }
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(snakeTag))
        {
            if (collision.gameObject.GetComponent<MovementController>() != player)
            {
                if (player.buildingController.flags.Contains(gameObject))
                {
                    player.buildingController.flags.Remove(gameObject);
                    Destroy(gameObject);
                }
                else if (!player.buildingController.flags.Contains(gameObject))
                {
                    Debug.LogError("Flag not in other player's flag list");
                }

            }
        }
        
    }
}
