using System.Collections.Generic;
using UnityEngine;

public class CloneRecorderPlayer : MonoBehaviour //REWORKED FULLY DUE TO REMOVING Time.time COMPONENT AND MAKING IT STEP BASED!
{
    [SerializeField] private float cellsize = 1f;

    private List<StepFrame> _steps;
    private int _currentIndex = 0;

    public void Begin(List<StepFrame> recordedSteps)
    {
        _steps = recordedSteps;
        _currentIndex = 0;

        CharacterMovement.OnStep.AddListener(AdvanceStep);
        CharacterMovement.OnReset.AddListener(ResetTheLevel);
    }

    private void AdvanceStep()
    {
        if (_currentIndex >= _steps.Count)
            return;

        Vector2Int pos = _steps[_currentIndex].pos;

        transform.position = new Vector3(
            pos.x * cellsize,
            0f,
            pos.y * cellsize
        );

        _currentIndex++;
    }

    private void ResetTheLevel()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        CharacterMovement.OnStep.RemoveListener(AdvanceStep);
        CharacterMovement.OnReset.RemoveListener(ResetTheLevel);
    }
}