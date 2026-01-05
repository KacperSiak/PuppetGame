using System.Collections.Generic;
using UnityEngine;

public class CloneRecorderPlayer : MonoBehaviour //REWORKED FULLY DUE TO REMOVING Time.time COMPONENT AND MAKING IT STEP BASED!
{
    [SerializeField] private float cellsize = 1f;

    private List<StepFrame> _steps;
    private int _currentIndex = 0;

    // undo bugfix spaghetti needs me to record the starting position of the clone
    private Vector3 _startPosition;

    public void Begin(List<StepFrame> recordedSteps)
    {
        _steps = recordedSteps;
        _currentIndex = 0;

        //undo bugfix spaghetti pt2 (capture the starting pos):
        _startPosition = transform.position;

        CharacterMovement.OnStep.AddListener(AdvanceStep);
        CharacterMovement.OnReset.AddListener(ResetTheLevel);

    }
    public void UndoStep()
    {
        if (_currentIndex <= 0)
            return; // nothing to undo

        _currentIndex--; // rewind index state

        // 3. LOGIC FIX: Check where to move
        if (_currentIndex == 0)
        {
            // If we rewound to 0, we are back at the start line
            transform.position = _startPosition;
        }
        else
        {
            // Otherwise, we move to the PREVIOUS step (index - 1)
            // Example: If index is 1, we are physically at step 0. 
            // To undo, we need to move to step "-1" (which is start) 
            // or if index is 2, move to step 1.

            Vector2Int pos = _steps[_currentIndex - 1].pos;
            transform.position = new Vector3(pos.x * cellsize, 0f, pos.y * cellsize);
        }
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