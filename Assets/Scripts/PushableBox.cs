using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour, IPressable
{
    public Vector3Int gridPosition; // Changed to Vector3Int
    public float cellSize = 1f;
    public LayerMask obstacleLayer;

    private List<Vector3Int> _history = new List<Vector3Int>(); // Changed to Vector3Int
    private Vector3Int _startPosition; // Changed to Vector3Int

    private void Start()
    {
        // initialize grid info based on spawn location
        gridPosition = new Vector3Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.y / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );
        _startPosition = gridPosition;

        CloneRecorder.OnCloneSpawned.AddListener(ResetBox);
        CharacterMovement.OnLateStep.AddListener(RecordHistory);
        CharacterMovement.OnUndo.AddListener(Undo);
        CharacterMovement.OnReset.AddListener(ResetBox);
        CharacterMovement.OnConveyorStep.AddListener(CheckConveyor);

        UpdateWorldPosition();
    }

    public bool Push(Vector3Int direction) // Changed to Vector3Int
    {
        Vector3Int targetPos = gridPosition + direction;

        // Optional: Add Elevator logic here if boxes can ride elevators too
        // (Copy the Elevator Check block from CharacterMovement here if needed)

        // check for walls at the NEW height
        if (Physics.CheckSphere(new Vector3(targetPos.x * cellSize, targetPos.y * cellSize, targetPos.z * cellSize), 0.3f, obstacleLayer))
        {
            return false;
        }

        gridPosition = targetPos;
        UpdateWorldPosition();
        return true;
    }

    private void RecordHistory()
    {
        _history.Add(gridPosition);
    }

    private void Undo()
    {
        if (_history.Count > 0)
        {
            _history.RemoveAt(_history.Count - 1);

            if (_history.Count == 0)
                gridPosition = _startPosition;
            else
                gridPosition = _history[_history.Count - 1];

            UpdateWorldPosition();
        }
    }

    private void ResetBox()
    {
        gridPosition = _startPosition;
        _history.Clear();
        UpdateWorldPosition();
    }

    private void UpdateWorldPosition()
    {
        // Now uses Y coordinate
        transform.position = new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, gridPosition.z * cellSize);
    }

    private void CheckConveyor()
    {
        Vector3 checkFloorPos = new Vector3(gridPosition.x * cellSize, gridPosition.y * cellSize, gridPosition.z * cellSize);
        Collider[] floorColliders = Physics.OverlapSphere(checkFloorPos + Vector3.down * (cellSize * 0.5f), 0.3f);

        foreach (var col in floorColliders)
        {
            if (col.TryGetComponent(out ConveyorTile conveyor))
            {
                // Re-use existing push logic
                Push(conveyor.pushDirection);
                break;
            }
        }
    }

    private void OnDestroy() // gemini said this prevents memory leaks so here we go
    {
        CloneRecorder.OnCloneSpawned.RemoveListener(ResetBox);
        CharacterMovement.OnLateStep.RemoveListener(RecordHistory);
        CharacterMovement.OnUndo.RemoveListener(Undo);
        CharacterMovement.OnReset.RemoveListener(ResetBox);
        CharacterMovement.OnConveyorStep.RemoveListener(CheckConveyor);
    }
}