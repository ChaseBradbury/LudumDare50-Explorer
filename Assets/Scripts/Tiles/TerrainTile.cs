using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Tiles/TerrainTile")]
public class TerrainTile : Tile
{
    [SerializeField]
    public ObstaclePlaceholderTile[] obstacles;
    [SerializeField]
    public ObstaclePlaceholderTile goal;

    [SerializeField]
    public TerrainTile[] neighborTerrain;

    public TerrainTile GetTile()
    {
        int rand = Random.Range(0, neighborTerrain.Length * 10);
        if (rand >= neighborTerrain.Length)
        {
            return this;
        }
        else
        {
            return neighborTerrain[rand];
        }
    }

    public ObstaclePlaceholderTile ObstacleTile()
    {
        int rand = Random.Range(0, obstacles.Length);
        return obstacles[rand];
    }
}
