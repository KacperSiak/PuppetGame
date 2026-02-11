using System.Collections.Generic;
using UnityEngine;

public class CloneRecorderPlayer : MonoBehaviour, IPressable
{
    [SerializeField] private float cellsize = 1f;

    private List<StepFrame> _steps;
    private int _currentIndex = 0;

    private Vector3 _startPosition;
    private int _stepsFromBirth = 0;

    private Animator _animator;

    public void Begin(List<StepFrame> recordedSteps)
    {
        _steps = recordedSteps;
        _currentIndex = 0;
        _stepsFromBirth = 0;
        _startPosition = transform.position;

        CharacterMovement.OnStep.AddListener(AdvanceStep);
        CharacterMovement.OnReset.AddListener(ResetTheLevel);
        CharacterMovement.OnPreUndo.AddListener(TryUndo);
        CloneRecorder.OnReplay.AddListener(RestartLoop);

        _animator = GetComponent<Animator>();
    }

    private void AdvanceStep()
    {
        _stepsFromBirth++;

        if (_currentIndex >= _steps.Count)
            return;

        // CHANGED: Use Vector3Int
        Vector3Int nextPos = _steps[_currentIndex].pos;

        // CHANGED: Calculate current grid pos as Vector3Int
        Vector3Int currentGridPos = new Vector3Int(
            Mathf.RoundToInt(transform.position.x / cellsize),
            Mathf.RoundToInt(transform.position.y / cellsize),
            Mathf.RoundToInt(transform.position.z / cellsize)
        );

        // CHANGED: Direction is now Vector3Int
        Vector3Int direction = nextPos - currentGridPos;

        // Push logic - checking overlap in 3D
        Collider[] colliders = Physics.OverlapSphere(new Vector3(nextPos.x * cellsize, nextPos.y * cellsize, nextPos.z * cellsize), 0.3f, LayerMask.GetMask("Box"));
        foreach (var hit in colliders)
        {
            if (hit.TryGetComponent(out PushableBox box))
            {
                box.Push(direction);
            }
        }

        // Apply movement using Y axis
        transform.position = new Vector3(nextPos.x * cellsize, nextPos.y * cellsize, nextPos.z * cellsize);
        _currentIndex++;
        if (direction != Vector3Int.zero)
        {
            if (direction.x != 0 || direction.z != 0)
            {
                Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
                transform.rotation = Quaternion.LookRotation(lookDir);
                _animator.SetTrigger("Walk");
            }
        }    
    }

    public void TryUndo()
    {
        if (_stepsFromBirth <= 0)
            return;

        _stepsFromBirth--;

        if (_stepsFromBirth >= _steps.Count)
            return;

        _currentIndex--;

        if (_currentIndex <= 0)
        {
            transform.position = _startPosition;
            _currentIndex = 0;
        }
        else
        {
            // CHANGED: Using Vector3Int from steps
            Vector3Int pos = _steps[_currentIndex - 1].pos;
            transform.position = new Vector3(pos.x * cellsize, pos.y * cellsize, pos.z * cellsize);
        }
    }

    private void ResetTheLevel()
    {
        Destroy(gameObject);
    }

    private void RestartLoop()
    {
        // Reset counters
        _currentIndex = 0;
        _stepsFromBirth = 0;

        // Teleport back to start
        transform.position = _startPosition;
    }

    private void OnDestroy()
    {
        CharacterMovement.OnStep.RemoveListener(AdvanceStep);
        CharacterMovement.OnReset.RemoveListener(ResetTheLevel);
        CharacterMovement.OnPreUndo.RemoveListener(TryUndo);
        CloneRecorder.OnReplay.RemoveListener(RestartLoop);
    }
}