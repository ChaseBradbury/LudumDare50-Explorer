using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;

    [SerializeField]
    private TilemapController terrainMap;

    [SerializeField]
    private TilemapController fogMap;

    [SerializeField]
    private Tile visitedFogTile;

    MovementInput movementInput;

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
        movementInput.Keys.North.performed += _ => MoveDirection(Direction.North);
        movementInput.Keys.Northeast.performed += _ => MoveDirection(Direction.Northeast);
        movementInput.Keys.East.performed += _ => MoveDirection(Direction.East);
        movementInput.Keys.Southeast.performed += _ => MoveDirection(Direction.Southeast);
        movementInput.Keys.South.performed += _ => MoveDirection(Direction.South);
        movementInput.Keys.Center.performed += _ => MoveDirection(Direction.South);
        movementInput.Keys.Southwest.performed += _ => MoveDirection(Direction.Southwest);
        movementInput.Keys.West.performed += _ => MoveDirection(Direction.West);
        movementInput.Keys.Northwest.performed += _ => MoveDirection(Direction.Northwest);
    }

    void OnEnable()
    {
        movementInput.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        SetFog(null);
    }

    void BeginClick()
    {
        SetFog(visitedFogTile);
        Vector2 clickPosition = mainCamera.ScreenToWorldPoint(movementInput.Click.Position.ReadValue<Vector2>());
        transform.position = terrainMap.MoveByAngle(transform.position, clickPosition);
    }

    void EndClick()
    {
        // dragStartPosition = mainCamera.ScreenToWorldPoint(dragStartPosition);
        // Show the drag selection sprite
    }

    void MoveDirection(Direction direction)
    {
        SetFog(visitedFogTile);
        transform.position = terrainMap.MoveDirectional(transform.position, direction);
    }

    void SetFog(Tile tile)
    {
        Vector3Int currentCell = fogMap.GetCellPos(transform.position);
        fogMap.SetTile(currentCell, tile);
        List<Vector3Int> neighbors = fogMap.GetAllNeighbors(currentCell);
        foreach (Vector3Int neighbor in neighbors)
        {
            fogMap.SetTile(neighbor, tile);
        }
    }
}
