using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapGenManager : MonoBehaviour
{
    [Header("Tilemaps")]

    [SerializeField]
    private TilemapController terrainMap;

    [SerializeField]
    private TilemapController obstacleMap;

    [SerializeField]
    private TerrainTile startingTerrainTile;

    [SerializeField]
    private TerrainTile boundaryTile;

    private Dictionary<Vector3Int, TerrainTile> generatedDict = new Dictionary<Vector3Int, TerrainTile>();

    // Static singleton instance
    private static TilemapGenManager instance;

    // Static singleton property
    public static TilemapGenManager Instance
    {
        get
        {
            return instance ?? (instance = FindObjectOfType<TilemapGenManager>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateTerrain(new Vector3Int(0, 0, 0), startingTerrainTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnDestroy()
    {
        instance = null;
    }

    public void GenerateTerrain(Vector3Int position, TerrainTile tile)
    {
        
        GenerateTile(position, tile);
        List<Vector3Int> neighbors = terrainMap.GetAllNeighbors(position);
        foreach (Vector3Int neighbor in neighbors)
        {
            GenerateTile(neighbor, tile);
        }
    }

    public void GenerateTile(Vector3Int position, TerrainTile tile)
    {
        if (!terrainMap.HasTile(position))
        {
            TerrainTile newTile = CalculateTile(position, tile);
            terrainMap.SetTile(position, newTile);
            
            int rand = Random.Range(0, 10);
            if (rand == 0)
            {
                ObstaclePlaceholderTile obstacleTile = newTile.ObstacleTile();
                obstacleMap.SetTile(position, obstacleTile);
            }
            rand = Random.Range(0, 10);
            if (rand == 0)
            {
                ObstaclePlaceholderTile obstacleTile = newTile.goal;
                obstacleMap.SetTile(position, obstacleTile);
            }
        }
        else
        {
            // Handle boundaries
            TerrainTile neighborTile = terrainMap.GetTile(position) as TerrainTile;
            if (neighborTile == boundaryTile)
            {
                TerrainTile newTile = CalculateTile(position, tile);
                terrainMap.SetTile(position, newTile);
                ObstaclePlaceholderTile obstacleTile = newTile.ObstacleTile();
                obstacleMap.SetTile(position, obstacleTile);
            }
        }
    }

    public TerrainTile CalculateTile(Vector3Int position, TerrainTile fallbackTile)
    {
        Dictionary<TerrainTile, int> possibleDict = new Dictionary<TerrainTile, int>();
        int neighborCount = 0;
        List<Vector3Int> neighbors = terrainMap.GetAllNeighbors(position);
        foreach (Vector3Int neighbor in neighbors)
        {
            TerrainTile neighborTile = terrainMap.GetTile(neighbor) as TerrainTile;
            if (neighborTile != null && neighborTile != boundaryTile)
            {
                if (possibleDict.ContainsKey(neighborTile))
                {
                    possibleDict[neighborTile] += 1;
                }
                else
                {
                    possibleDict[neighborTile] = 1;
                }
                neighborCount++;
                foreach (TerrainTile neighborPossible in neighborTile.neighborTerrain)
                {
                    if (possibleDict.ContainsKey(neighborPossible))
                    {
                        // Add weight?
                        possibleDict[neighborPossible] += 1;
                    }
                    else
                    {
                        possibleDict[neighborPossible] = 1;
                    }
                }
            }
        }
        List<TerrainTile> possibleList = new List<TerrainTile>();
        foreach (TerrainTile tile in possibleDict.Keys)
        {
            if (possibleDict[tile] >= neighborCount)
            {
                possibleList.Add(tile);
            }
        }
        if (possibleList.Count == 0)
        {
            return fallbackTile;
        }
        else
        {
            int rand = Random.Range(0, possibleList.Count);
            return possibleList[rand];
        }
    }

    public void GenerateTerrain(Vector2 position)
    {
        Vector3Int position3 = terrainMap.GetCellPos(position);
        TerrainTile tile = terrainMap.GetTile(position3) as TerrainTile;
        if (tile == null)
        {
            tile = startingTerrainTile;
        }
        GenerateTerrain(position3, tile);
    }

    void GenerateTerrainOld(Vector3Int position, TerrainTile tile, int size)
    {
        if (size <= 0)
        {
            return;
        }
        if (!terrainMap.HasTile(position))
        {
            TerrainTile newTile = tile.GetTile();
            terrainMap.SetTile(position, newTile);
            // if (size == 1)
            // {
            //     ObstaclePlaceholderTile obstacleTile = tile.ObstacleTile();
            //     obstacleMap.SetTile(position, obstacleTile);
            // }
            List<Vector3Int> neighbors = terrainMap.GetAllNeighbors(position);
            foreach (Vector3Int neighbor in neighbors)
            {
                GenerateTerrainOld(neighbor, newTile, size - 1);
            }
        }
    }
}
