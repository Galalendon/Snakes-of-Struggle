using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName ="New Tile Data", menuName = "Custom Tile/Basic Tile")]


public class TileData : TileBase
{
    public Color tileColour;
    public Sprite tileSprite;
}
