using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;

    [Header("UI")]
    [SerializeField]
    private GameObject winScreen;
    [SerializeField]
    private Text visitedCounter;

    [Header("Tilemaps")]

    [SerializeField]
    private TilemapController terrainMap;

    [SerializeField]
    private TilemapController obstacleMap;

    [SerializeField]
    private TilemapController fogMap;

    [SerializeField]
    private TilemapController obstacleRevealMap;

    [Header("Tiles")]

    [SerializeField]
    private Tile visitedFogTile;

    MovementInput movementInput;

    private int step = 0;
    private Dictionary<Vector3Int, int> visitedTiles = new Dictionary<Vector3Int, int>();

    void Awake()
    {
        // Instantiate Input interfaces
        movementInput = new MovementInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Click inputs
        movementInput.Click.PrimaryClick.performed += _ => BeginClick();
        movementInput.Click.PrimaryClick.canceled += _ => EndClick();

        // Keyboard inputs
        movementInput.Keys.North.performed += _ => InputDirection(Direction.North);
        movementInput.Keys.Northeast.performed += _ => InputDirection(Direction.Northeast);
        movementInput.Keys.East.performed += _ => InputDirection(Direction.East);
        movementInput.Keys.Southeast.performed += _ => InputDirection(Direction.Southeast);
        movementInput.Keys.South.performed += _ => InputDirection(Direction.South);
        movementInput.Keys.Center.performed += _ => InputDirection(Direction.South);
        movementInput.Keys.Southwest.performed += _ => InputDirection(Direction.Southwest);
        movementInput.Keys.West.performed += _ => InputDirection(Direction.West);
        movementInput.Keys.Northwest.performed += _ => InputDirection(Direction.Northwest);
    }

    void OnEnable()
    {
        movementInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        SetFog(null);
        RevealObstacles();
        visitedCounter.text = visitedTiles.Count.ToString();
    }

    void BeginClick()
    {
        SetFog(visitedFogTile);
        Vector2 clickPosition = mainCamera.ScreenToWorldPoint(movementInput.Click.Position.ReadValue<Vector2>());
        MoveTo(terrainMap.MoveByAngle(transform.position, clickPosition));
    }

    void EndClick()
    {
        // dragStartPosition = mainCamera.ScreenToWorldPoint(dragStartPosition);
        // Show the drag selection sprite
    }

    void InputDirection(Direction direction)
    {
        SetFog(visitedFogTile);
        MoveTo(terrainMap.MoveDirectional(transform.position, direction));
    }

    void MoveTo(Vector2 newPosition)
    {
        if (!obstacleRevealMap.HasTile(obstacleMap.GetCellPos(newPosition)))
        {
            transform.position = newPosition;
        }
    }

    void RevealObstacles()
    {
        Vector3Int currentCell = obstacleMap.GetCellPos(transform.position);
        List<Vector3Int> neighbors = obstacleMap.GetAllNeighbors(currentCell);
        ObstaclePlaceholderTile tile;
        foreach (Vector3Int neighbor in neighbors)
        {
            tile = obstacleMap.GetTile(neighbor) as ObstaclePlaceholderTile;
            if (tile != null)
            {
                if (tile.isGoal)
                {
                    FinishGame();
                }
                obstacleRevealMap.SetTile(neighbor, tile.revealedTile);
                obstacleMap.SetTile(neighbor, null);
            }
        }
    }


    void SetFog(Tile tile)
    {
        Vector3Int currentCell = fogMap.GetCellPos(transform.position);
        visitedTiles[currentCell] = step;
        fogMap.SetTile(currentCell, tile);
        List<Vector3Int> neighbors = fogMap.GetAllNeighbors(currentCell);
        foreach (Vector3Int neighbor in neighbors)
        {
            visitedTiles[neighbor] = step;
            fogMap.SetTile(neighbor, tile);
        }
    }

    void FinishGame()
    {
        winScreen.SetActive(true);
        movementInput.Disable();
    }
}
