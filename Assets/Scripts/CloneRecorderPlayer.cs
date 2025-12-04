using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneRecorderPlayer : MonoBehaviour
{
    [SerializeField] private float cellsize = 1f;

    private List<StepFrame> _steps = new List<StepFrame>();
    private int _currentIndex;
    private float _startTime;

    public void Begin(List<StepFrame> recordedSteps)
    {
        _steps = recordedSteps;
        _currentIndex = 0;
        _startTime = Time.time;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        while (_currentIndex < _steps.Count)
        {
            StepFrame frame = _steps[_currentIndex];

            float targetTime = _startTime + frame.time;

            while (Time.time < targetTime)
            {
                yield return null;
            }

            Vector3 worldPos = new Vector3 (frame.pos.x * cellsize, 0f, frame.pos.y * cellsize);

            transform.position = worldPos;

            _currentIndex++;
        }
    }
}
