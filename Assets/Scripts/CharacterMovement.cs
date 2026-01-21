using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    public Vector2Int gridPosition;
    public float cellsize = 1f;
    public List<Transform> playerPositions = new List<Transform>();

    // STEP-BASED EVENT
    public static UnityEvent OnStep = new UnityEvent();
    public static UnityEvent OnReset = new UnityEvent();
    public static UnityEvent OnUndo = new UnityEvent();

    [SerializeField] private CloneRecorder recorder;

    private int _spawnIndex = 1;

    private void Start()
    {
        _spawnIndex = 1;
        UpdateWorldPosition();
    }

    private void Update()
    {
        bool didMove = false;
        Vector2Int prevPos = gridPosition;

        if (recorder.availableSteps <= 0)
            return;

        // MOVEMENT
        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector2Int.right;
        else if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector2Int.left;

        if (moveDir != Vector2Int.zero)
        {
            if (TryMove(moveDir))
            {
                didMove = true;
            }
        }

        // UNDO
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (recorder.UndoStep(out Vector2Int undoPos))
            {
                // undo player
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
            recorder.RecordStep(gridPosition);
            UpdateWorldPosition();
            OnStep.Invoke();
        }
    }

    private void UpdateWorldPosition()
    {
        transform.position = new Vector3(
            gridPosition.x * cellsize,
            0f,
            gridPosition.y * cellsize
        );
    }

    public void ChangePlace()
    {
        Transform destination = playerPositions[_spawnIndex];
        _spawnIndex++;

        transform.position = destination.position;

        gridPosition = new Vector2Int(
            Mathf.RoundToInt(destination.position.x / cellsize),
            Mathf.RoundToInt(destination.position.z / cellsize)
        );
    }

    public void ResetTheLevel()
    {
        Transform destination = playerPositions[0];

        transform.position = destination.position;
        gridPosition = new Vector2Int(
            Mathf.RoundToInt(destination.position.x / cellsize),
            Mathf.RoundToInt(destination.position.z / cellsize)
        );

        _spawnIndex = 1;
        OnReset.Invoke();
    }

    private bool TryMove(Vector2Int direction)
    {
        Vector2Int targetPos = gridPosition + direction;
        Vector3 checkPos = new Vector3(targetPos.x * cellsize, 0, targetPos.y * cellsize);

        // 1. Check for Walls (use a Wall layer)
        if (Physics.CheckSphere(checkPos, 0.3f, LayerMask.GetMask("Wall")))
            return false;

        // 2. Check for Boxes
        Collider[] colliders = Physics.OverlapSphere(checkPos, 0.3f, LayerMask.GetMask("Box"));
        if (colliders.Length > 0)
        {
            PushableBox box = colliders[0].GetComponent<PushableBox>();
            if (box != null)
            {
                // Try pushing the box. If the box is blocked, the player is blocked.
                if (!box.Push(direction)) return false;
            }
        }

        gridPosition = targetPos;
        return true;
    }


}