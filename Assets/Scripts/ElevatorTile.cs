using System.Collections.Generic;
using UnityEngine;

public class ElevatorTile : MonoBehaviour
{
    [Header("Settings")]
    public int heightDelta = 1;
    public float cellSize = 1f;

    // NEW: Toggle this to make it a "Bumper" instead of an "Elevator"
    [Tooltip("If true, the platform won't move, it just bumps the player.")]
    public bool isStatic = false;

    // History for Undo functionality
    private List<Vector3> _historyPos = new List<Vector3>();
    private List<int> _historyDelta = new List<int>();

    private Vector3 _startPos;
    private int _startDelta;

    private void Start()
    {
        _startPos = transform.position;
        _startDelta = heightDelta;

        CharacterMovement.OnLateStep.AddListener(RecordHistory);
        CharacterMovement.OnUndo.AddListener(Undo);
        CharacterMovement.OnReset.AddListener(ResetElevator);
    }

    public void Interact()
    {
        // NEW: If it's static, do nothing! 
        // The player has already received the "bump" from the CheckFloor logic in CharacterMovement.
        // We just skip the part where the platform moves or flips direction.
        if (isStatic) return;

        // 1. Move the visual object
        transform.position += new Vector3(0, heightDelta * cellSize, 0);

        // 2. Flip direction for the next interaction
        heightDelta = -heightDelta;
    }

    private void RecordHistory()
    {
        // We still record history for static tiles so the lists stay in sync with the Undo system
        _historyPos.Add(transform.position);
        _historyDelta.Add(heightDelta);
    }

    private void Undo()
    {
        if (_historyPos.Count > 0)
        {
            _historyPos.RemoveAt(_historyPos.Count - 1);
            _historyDelta.RemoveAt(_historyDelta.Count - 1);

            if (_historyPos.Count > 0)
            {
                transform.position = _historyPos[_historyPos.Count - 1];
                heightDelta = _historyDelta[_historyDelta.Count - 1];
            }
            else
            {
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