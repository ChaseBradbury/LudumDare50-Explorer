using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapController : MonoBehaviour
{

    private class DirectionInfo
    {
        public Vector3Int vector;
        public bool isCube;
        public DirectionInfo(Vector3Int vector, bool isCube = false)
        {
            this.vector = vector;
            this.isCube = isCube;
        }
    }


    private Tilemap map;

    private Dictionary<Direction, DirectionInfo> directionDict = new Dictionary<Direction, DirectionInfo>();

    void Awake()
    {
        map = GetComponent<Tilemap>();

        directionDict[Direction.North] = new DirectionInfo(new Vector3Int(0, 1, 0), false);
        directionDict[Direction.Northeast] = new DirectionInfo(new Vector3Int(0, 1, -1), true);
        directionDict[Direction.East] = new DirectionInfo(new Vector3Int(1, 0, 0), false);
        directionDict[Direction.Southeast] = new DirectionInfo(new Vector3Int(1, -1, 0), true);
        directionDict[Direction.South] = new DirectionInfo(new Vector3Int(0, -1, 0), false);
        directionDict[Direction.Southwest] = new DirectionInfo(new Vector3Int(0, -1, 1), true);
        directionDict[Direction.West] = new DirectionInfo(new Vector3Int(-1, 0, 0), false);
        directionDict[Direction.Northwest] = new DirectionInfo(new Vector3Int(-1, 1, 0), true);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        map = null;
    }

    // Converts a cell to cube space
    // See https://www.redblobgames.com/grids/hexagons/ for reference
    public Vector3Int ConvertCellToCube(Vector3Int cellPos)
    {
        int q = cellPos.x - (cellPos.y - Mathf.Abs(cellPos.y)%2) / 2;
        int r = cellPos.y;
        int s = -q - r;
        return new Vector3Int(q, r, s);
    }

    // Converts a cell in cube space back to hex space
    public Vector3Int ConvertCubeToCell(Vector3Int cubePos)
    {
        int x = cubePos.x + (cubePos.y - Mathf.Abs(cubePos.y)%2) / 2;
        int y = cubePos.y;
        return new Vector3Int(x, y, 0);
    }

    public Vector2 SnapToMap(Vector2 worldPos)
    {
        Vector3Int tilemapPos = map.WorldToCell(worldPos);
        Vector2 newPos = map.CellToWorld(tilemapPos);
        return newPos;
    }

    public Vector2 MoveDirectional(Vector2 worldPos, Direction direction)
    {
        Vector3Int tilemapPos = map.WorldToCell(worldPos);
        return map.CellToWorld(GetCellByDirection(tilemapPos, direction));
    }

    public Vector3Int GetCellByDirection(Vector3Int tilemapPos, Direction direction)
    {
        if (directionDict[direction].isCube)
        {
            Vector3Int cubePos = ConvertCellToCube(tilemapPos);
            Vector3Int cellPos = ConvertCubeToCell(cubePos + directionDict[direction].vector);
            return cellPos;
        }
        else
        {
            Vector3Int cellPos = tilemapPos + directionDict[direction].vector;
            return cellPos;
        }
    }

    public Vector3Int GetCellPos(Vector2 worldPos)
    {
        return map.WorldToCell(worldPos);
    }

    public List<Vector3Int> GetAllNeighbors(Vector3Int tilemapPos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (Direction direction in directionDict.Keys)
        {
            if (direction != Direction.North && direction != Direction.South)
            {
                neighbors.Add(GetCellByDirection(tilemapPos, direction));
            }
        }
        return neighbors;
    }

    public void SetTile(Vector3Int tilemapPos, Tile tile)
    {
        map.SetTile(tilemapPos, tile);
    }

    public TileBase GetTile(Vector3Int tilemapPos)
    {
        return map.GetTile(tilemapPos);
    }

    public bool HasTile(Vector3Int tilemapPos)
    {
        return map.GetTile(tilemapPos) != null;
    }

    // Returns an int between 0 and 5, 0 being positive along the x axis, and moving counterclockwise in 60 degree intervals
    public Vector2 MoveByAngle(Vector2 worldStart, Vector2 worldEnd)
    {
        // Find difference to compare
        float x = worldEnd.x - worldStart.x;
        float y =  worldEnd.y - worldStart.y;

        // Check if horizontal (3 or 9 o'clock)
        if (Mathf.Abs(x) > 6*Mathf.Abs(y))
        {
            if (x > 0)
            {
                // At 3 o'clock (default)
                return MoveDirectional(worldStart, Direction.East);
            }
            else
            {
                // At 9 o'clock
                return MoveDirectional(worldStart, Direction.West);
            }
        }
        // If not horizontal
        else
        {
            // Between 10 and 2 o'clock
            if (y > 0)
            {
                if (x > 0)
                {
                    // At 1 o'clock
                    return MoveDirectional(worldStart, Direction.Northeast);
                }
                else
                {
                    // At 11 o'clock
                    return MoveDirectional(worldStart, Direction.Northwest);
                }
            }
            else
            {
                if (x > 0)
                {
                    // At 5 o'clock
                    return MoveDirectional(worldStart, Direction.Southeast);
                }
                else
                {
                    // At 7 o'clock
                    return MoveDirectional(worldStart, Direction.Southwest);
                }
            }
        }
    }
}
