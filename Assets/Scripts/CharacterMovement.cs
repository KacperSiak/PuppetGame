using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    public Vector3Int gridPosition; // Changed to Vector3Int
    public float cellsize = 1f;
    public List<Transform> playerPositions = new List<Transform>();

    public static UnityEvent OnStep = new UnityEvent();
    public static UnityEvent OnReset = new UnityEvent();
    public static UnityEvent OnUndo = new UnityEvent();
    public static UnityEvent OnPreUndo = new UnityEvent();
    public static UnityEvent OnLateStep = new UnityEvent();

    [SerializeField] private CloneRecorder recorder;
    private int _spawnIndex = 1;
    private Animator _animator;

    private void Start()
    {
        _spawnIndex = 1;
        // Snap to nearest grid on start
        gridPosition = new Vector3Int(
            Mathf.RoundToInt(transform.position.x / cellsize),
            Mathf.RoundToInt(transform.position.y / cellsize), // Now capturing Y
            Mathf.RoundToInt(transform.position.z / cellsize)
        );
        UpdateWorldPosition();

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        bool didMove = false;

        if (recorder.availableSteps <= 0) return;

        // MOVEMENT INPUT (Maps 2D Input to 3D Directions)
        Vector3Int moveDir = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector3Int.forward; // Z+
        else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector3Int.back;    // Z-
        else if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector3Int.right;  // X+
        else if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector3Int.left;   // X-

        if (moveDir != Vector3Int.zero)
        {
            if (TryMove(moveDir))
            {
                didMove = true;
            }
        }

        // UNDO
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (recorder.UndoStep(out Vector3Int undoPos)) // Updated out type
            {
                OnPreUndo.Invoke();
                gridPosition = undoPos;
                UpdateWorldPosition();
                OnUndo.Invoke();
            }
        }

        // RESET
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTheLevel();
            return;
        }

        // COMMIT MOVE
        if (didMove)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);

            recorder.RecordStep(gridPosition);
            UpdateWorldPosition();
            OnStep.Invoke();
            OnLateStep.Invoke();
        }
    }

    private void UpdateWorldPosition()
    {
        // Now uses gridPosition.y for height
        transform.position = new Vector3(
            gridPosition.x * cellsize,
            gridPosition.y * cellsize,
            gridPosition.z * cellsize
        );
    }

    public void ChangePlace()
    {
        Transform destination = playerPositions[_spawnIndex];
        _spawnIndex++;

        transform.position = destination.position;
        SnapToGrid(destination.position);
    }

    public void ResetTheLevel()
    {
        Transform destination = playerPositions[0];
        transform.position = destination.position;
        SnapToGrid(destination.position);
        _spawnIndex = 1;
        OnReset.Invoke();
    }

    private void SnapToGrid(Vector3 worldPos)
    {
        gridPosition = new Vector3Int(
            Mathf.RoundToInt(worldPos.x / cellsize),
            Mathf.RoundToInt(worldPos.y / cellsize),
            Mathf.RoundToInt(worldPos.z / cellsize)
        );
    }

    private bool TryMove(Vector3Int direction)
    {
        // 1. Calculate the horizontal target
        Vector3Int targetPos = gridPosition + direction;

        // 2. CHECK FOR ELEVATOR at the target position (Are we walking ONTO an elevator?)
        // We check slightly below the target position to find the floor tile
        Vector3 checkFloorPos = new Vector3(targetPos.x * cellsize, targetPos.y * cellsize, targetPos.z * cellsize);

        // Check for elevator tile slightly below the feet level (offset -0.5f usually works for floor checks)
        Collider[] floorColliders = Physics.OverlapSphere(checkFloorPos + Vector3.down * (cellsize * 0.5f), 0.3f);

        foreach (var col in floorColliders)
        {
            if (col.TryGetComponent(out ElevatorTile elevator))
            {
                // If we found an elevator, adjust the target Y
                targetPos.y += elevator.heightDelta;
            }
        }

        // 3. Now validate the FINAL target position (Walls/Boxes)
        Vector3 checkPos = new Vector3(targetPos.x * cellsize, targetPos.y * cellsize, targetPos.z * cellsize);

        // Check for walls
        if (Physics.CheckSphere(checkPos, 0.3f, LayerMask.GetMask("Wall")))
            return false;

        // Check for boxes
        Collider[] colliders = Physics.OverlapSphere(checkPos, 0.3f, LayerMask.GetMask("Box"));
        if (colliders.Length > 0)
        {
            PushableBox box = colliders[0].GetComponent<PushableBox>();
            if (box != null)
            {
                // Pass direction to box. 
                // Note: Boxes usually don't ride elevators in standard Sokoban, 
                // but if they do, the Box script needs similar elevator logic.
                if (!box.Push(direction)) return false;
            }
        }

        gridPosition = targetPos;
        _animator.SetTrigger("Walk");
        return true;
    }
}