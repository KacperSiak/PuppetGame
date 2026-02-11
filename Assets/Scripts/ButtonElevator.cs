using UnityEngine;
using System.Collections.Generic;

public class ButtonElevator : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Must match the buttonID of the FloorButton that controls it.")]
    public int targetButtonID;

    [Tooltip("How many grid units to move up when pressed.")]
    public int heightDelta = 1;
    public float cellSize = 1f;

    private Vector3 _startPos;
    private bool _isUp = false;

    private void Start()
    {
        _startPos = transform.position;

        // Listen to the button events
        FloorButton.OnButtonDown.AddListener(OnButtonDown);
        FloorButton.OnButtonUp.AddListener(OnButtonUp);
    }

    private void OnButtonDown(int id)
    {
        if (id == targetButtonID && !_isUp)
        {
            _isUp = true;
            MoveElevator(heightDelta); // Move UP
        }
    }

    private void OnButtonUp(int id)
    {
        if (id == targetButtonID && _isUp)
        {
            _isUp = false;
            MoveElevator(-heightDelta); // Move DOWN
        }
    }

    private void MoveElevator(int delta)
    {
        // 1. Use a Box shape instead of a Sphere for perfect grid detection
        Vector3 checkPos = transform.position + Vector3.up * (cellSize * 0.5f);

        // Make the box slightly smaller than the cell size (0.4f) so it doesn't accidentally grab adjacent tiles
        Vector3 halfExtents = new Vector3(cellSize * 0.4f, cellSize * 0.4f, cellSize * 0.4f);

        // Physics.OverlapBox is much better for blocky Sokoban games!
        Collider[] hits = Physics.OverlapBox(checkPos, halfExtents, Quaternion.identity);

        // 2. Move the elevator visually
        transform.position += new Vector3(0, delta * cellSize, 0);

        // 3. Move the entities
        foreach (var hit in hits)
        {
            // Using GetComponentInParent handles cases where the collider is on a child object (like a mesh)
            CharacterMovement player = hit.GetComponentInParent<CharacterMovement>();
            if (player != null)
            {
                player.gridPosition.y += delta;
                player.transform.position += new Vector3(0, delta * cellSize, 0);
                continue; // Move to the next hit
            }

            PushableBox box = hit.GetComponentInParent<PushableBox>();
            if (box != null)
            {
                box.gridPosition.y += delta;
                box.transform.position += new Vector3(0, delta * cellSize, 0);
                continue;
            }

            CloneRecorderPlayer clone = hit.GetComponentInParent<CloneRecorderPlayer>();
            if (clone != null)
            {
                clone.transform.position += new Vector3(0, delta * cellSize, 0);
                continue;
            }
        }
    }

    private void OnDestroy()
    {
        FloorButton.OnButtonDown.RemoveListener(OnButtonDown);
        FloorButton.OnButtonUp.RemoveListener(OnButtonUp);
    }
}