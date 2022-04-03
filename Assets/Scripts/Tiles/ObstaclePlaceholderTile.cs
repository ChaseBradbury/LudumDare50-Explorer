using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/ObstaclePlaceholderTile")]
public class ObstaclePlaceholderTile : Tile
{
    [SerializeField]
    public Tile revealedTile;

    [SerializeField]
    public bool isWalkable;

    [SerializeField]
    public bool isGoal;
}
