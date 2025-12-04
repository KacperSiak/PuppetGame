using System;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct StepFrame
{
    public Vector2Int pos;
    public float time;

    public StepFrame(Vector2Int pos, float time)
    {
        this.pos = pos; 
        this.time = time;
    }
}


public class CloneRecorder : MonoBehaviour
{
    [SerializeField] private GameObject _marionette;
    [SerializeField] private CharacterMovement _player;

    private List<StepFrame> _recordedSteps = new List<StepFrame>();
    private float _firstStepTime = -1f;

    private void Start()
    {
        CharacterMovement.OnStep.AddListener(MadeStep);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnMarionette();
        }
    }

    private void MadeStep(Vector2Int step, float time)
    {
        if (_firstStepTime < 0f) _firstStepTime = time;

        _recordedSteps.Add(new StepFrame(step, time - _firstStepTime));
    }

    private void SpawnMarionette()
    {
        GameObject marionette = Instantiate(_marionette, _player.playerSpawnPoint.position , Quaternion.identity);

        marionette.TryGetComponent<CloneRecorderPlayer>(out CloneRecorderPlayer recordPlayer);

        recordPlayer.Begin(_recordedSteps);
    }
}
