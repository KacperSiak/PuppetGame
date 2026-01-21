using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour
{
    public Vector2Int gridPosition;
    public float cellSize = 1f;
    public LayerMask obstacleLayer;

    private List<Vector2Int> _history = new List<Vector2Int>();
    private Vector2Int _startPosition;

    private void Start()
    {
        // initialize grid info based on spawn location
        gridPosition = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / cellSize),
            Mathf.RoundToInt(transform.position.z / cellSize)
        );
        _startPosition = gridPosition;

        CloneRecorder.OnCloneSpawned.AddListener(ResetBox);
        CharacterMovement.OnLateStep.AddListener(RecordHistory);
        CharacterMovement.OnUndo.AddListener(Undo);
        CharacterMovement.OnReset.AddListener(ResetBox);


        UpdateWorldPosition();
    }

    // return true if move possiible
    public bool Push(Vector2Int direction)
    {
        Vector2Int targetPos = gridPosition + direction;

        // check for walls
        if (Physics.CheckSphere(new Vector3(targetPos.x * cellSize, 0, targetPos.y * cellSize), 0.3f, obstacleLayer))
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
        transform.position = new Vector3(gridPosition.x * cellSize, 0f, gridPosition.y * cellSize); // 0f is box height location, currently hard set to Y=0
    }

    private void OnDestroy() // gemini said this prevents memory leaks so here we go
    {
        CloneRecorder.OnCloneSpawned.RemoveListener(ResetBox);
        CharacterMovement.OnLateStep.RemoveListener(RecordHistory);
        CharacterMovement.OnUndo.RemoveListener(Undo);
        CharacterMovement.OnReset.RemoveListener(ResetBox);
    }
}