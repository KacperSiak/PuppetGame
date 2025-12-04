using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


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
    [SerializeField] private int _marionetteCounter;
    [SerializeField] private TextMeshProUGUI _marionetteText;

    private List<StepFrame> _recordedSteps = new List<StepFrame>();
    private float _firstStepTime = -1f;
    private int _spawnIndex = 0;

    

    private void Start()
    {
        _spawnIndex = 0;
        CharacterMovement.OnStep.AddListener(MadeStep);
        _marionetteText.text = $"Marionette Left: {_marionetteCounter}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _marionetteCounter > 0)
        {
            SpawnMarionette();
            _marionetteCounter--;
            _marionetteText.text = $"Marionette Left: {_marionetteCounter}";
        }
    }

    private void MadeStep(Vector2Int step, float time)
    {
        if (_firstStepTime < 0f) _firstStepTime = time;

        _recordedSteps.Add(new StepFrame(step, time - _firstStepTime));
    }

    private void SpawnMarionette()
    {
        GameObject marionette = Instantiate(_marionette, _player.playerPositions[_spawnIndex].position , Quaternion.identity);
        _spawnIndex++;

        marionette.TryGetComponent<CloneRecorderPlayer>(out CloneRecorderPlayer recordPlayer);

        List<StepFrame> _copiedList = new List<StepFrame>(_recordedSteps);

        recordPlayer.Begin(_copiedList);

        _recordedSteps.Clear();
        _firstStepTime = -1f;
        _player.ChangePlace();
    }
}
