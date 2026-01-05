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
        if (Input.GetKeyDown(KeyCode.W))
        {
            gridPosition.y += 1;
            didMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            gridPosition.y -= 1;
            didMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            gridPosition.x += 1;
            didMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            gridPosition.x -= 1;
            didMove = true;
        }

        // UNDO
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (recorder.UndoStep(out Vector2Int undoPos))
            {
                gridPosition = undoPos;
                UpdateWorldPosition();
            }
            return;
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
}