using System.Collections.Generic;
using UnityEngine;

public class ElevatorTile : MonoBehaviour
{
    // 1 = Up, -1 = Down
    public int heightDelta = 1;
    public float cellSize = 1f;

    // History for Undo functionality
    private List<Vector3> _historyPos = new List<Vector3>();
    private List<int> _historyDelta = new List<int>();

    private Vector3 _startPos;
    private int _startDelta;

    private void Start()
    {
        _startPos = transform.position;
        _startDelta = heightDelta;

        // Subscribe to the same events as the player/boxes
        CharacterMovement.OnLateStep.AddListener(RecordHistory);
        CharacterMovement.OnUndo.AddListener(Undo);
        CharacterMovement.OnReset.AddListener(ResetElevator);
    }

    // Called by the Player when they step on this tile
    public void Interact()
    {
        // 1. Move the visual object
        transform.position += new Vector3(0, heightDelta * cellSize, 0);

        // 2. Flip direction for the next interaction (Up -> Down)
        heightDelta = -heightDelta;
    }

    private void RecordHistory()
    {
        // Save current state (whether we moved or not)
        _historyPos.Add(transform.position);
        _historyDelta.Add(heightDelta);
    }

    private void Undo()
    {
        if (_historyPos.Count > 0)
        {
            // Remove the latest frame
            _historyPos.RemoveAt(_historyPos.Count - 1);
            _historyDelta.RemoveAt(_historyDelta.Count - 1);

            // Restore state
            if (_historyPos.Count > 0)
            {
                transform.position = _historyPos[_historyPos.Count - 1];
                heightDelta = _historyDelta[_historyDelta.Count - 1];
            }
            else
            {
                // If history is empty, go to start
                ResetElevator();
            }
        }
    }

    private void ResetElevator()
    {
        transform.position = _startPos;
        heightDelta = _startDelta;
        _historyPos.Clear();
        _historyDelta.Clear();
    }

    private void OnDestroy()
    {
        CharacterMovement.OnLateStep.RemoveListener(RecordHistory);
        CharacterMovement.OnUndo.RemoveListener(Undo);
        CharacterMovement.OnReset.RemoveListener(ResetElevator);
    }
}