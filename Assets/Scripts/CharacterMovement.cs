using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class CharacterMovement : MonoBehaviour, IPressable
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

    private bool blockedMovement;

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

        FinalDestination.OnLevelEnd.AddListener(BlockMovement);
        StartCoroutine(StartBlockMovement());
    }
    
    IEnumerator StartBlockMovement()
    {
        blockedMovement = true;
        yield return new WaitForSeconds(4f);
        blockedMovement = false;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTheLevel();
            return;
        }

        // UNDO
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (recorder.UndoStep(out Vector3Int undoPos))
            {
                OnPreUndo.Invoke();
                gridPosition = undoPos;
                UpdateWorldPosition();
                OnUndo.Invoke();
            }
        }

        bool didMove = false;

        if (recorder.availableSteps <= 0) return;

        // MOVEMENT INPUT (Maps 2D Input to 3D Directions)
        Vector3Int moveDir = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W) && !blockedMovement) moveDir = Vector3Int.forward; // Z+
        else if (Input.GetKeyDown(KeyCode.S) && !blockedMovement) moveDir = Vector3Int.back;    // Z-
        else if (Input.GetKeyDown(KeyCode.D) && !blockedMovement) moveDir = Vector3Int.right;  // X+
        else if (Input.GetKeyDown(KeyCode.A) && !blockedMovement) moveDir = Vector3Int.left;   // X-

        if (moveDir != Vector3Int.zero)
        {
            if (TryMove(moveDir))
            {
                didMove = true;
            }
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
        Vector3Int targetPos = gridPosition + direction;

        // Hold a reference to the elevator if we find one
        ElevatorTile activeElevator = null;

        // 1. CHECK FLOOR (Is there an elevator below the target?)
        Vector3 checkFloorPos = new Vector3(targetPos.x * cellsize, targetPos.y * cellsize, targetPos.z * cellsize);

        Collider[] floorColliders = Physics.OverlapSphere(checkFloorPos + Vector3.down * (cellsize * 0.5f), 0.3f);

        foreach (var col in floorColliders)
        {
            if (col.TryGetComponent(out ElevatorTile elevator))
            {
                // Adjust player target height
                targetPos.y += elevator.heightDelta;

                // Store reference so we can move it LATER (only if move is valid)
                activeElevator = elevator;
            }
        }

        // 2. CHECK OBSTACLES (Walls/Boxes at the NEW target height)
        Vector3 checkPos = new Vector3(targetPos.x * cellsize, targetPos.y * cellsize, targetPos.z * cellsize);

        if (Physics.CheckSphere(checkPos, 0.3f, LayerMask.GetMask("Wall")))
            return false;

        Collider[] colliders = Physics.OverlapSphere(checkPos, 0.3f, LayerMask.GetMask("Box"));
        if (colliders.Length > 0)
        {
            PushableBox box = colliders[0].GetComponent<PushableBox>();
            if (box != null)
            {
                if (!box.Push(direction)) return false;
            }
        }

        // 3. COMMIT MOVE
        // If we got here, the move is valid. 

        // If we used an elevator, trigger its animation/logic now
        if (activeElevator != null)
        {
            activeElevator.Interact();
        }

    
        gridPosition = targetPos;
        _animator.SetTrigger("Walk");
        return true;
    }

    private void BlockMovement()
    {
        blockedMovement = true;
    }
}