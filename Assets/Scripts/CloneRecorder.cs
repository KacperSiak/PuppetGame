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
    [SerializeField] private GameObject marionetteClone;
    [SerializeField] private CharacterMovement player;
    [SerializeField] private int marionetteCounter;
    public int availableSteps;
    // [SerializeField] private TextMeshProUGUI _marionetteText;

    private List<StepFrame> _recordedSteps = new List<StepFrame>();
    private float _firstStepTime = -1f;
    private int _marionetteSpawnIndex = 0;

    // container for reset
    private int _marionetteCounterForReset;
    private int _availableStepsForReset;

    private void Start()
    {
        _marionetteSpawnIndex = 0;
        CharacterMovement.OnStep.AddListener(MadeStep);
        CharacterMovement.OnReset.AddListener(ResetCloneRecorder);

        _marionetteCounterForReset = marionetteCounter;
        _availableStepsForReset = availableSteps;   
        //_marionetteText.text = $"Marionette Left: {_marionetteCounter}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && marionetteCounter > 0)
        {
            SpawnMarionette();
            marionetteCounter--;
           // _marionetteText.text = $"Marionette Left: {_marionetteCounter}";
        }
    }

    private void MadeStep(Vector2Int step, float time)
    {
        if (_firstStepTime < 0f) _firstStepTime = time;

        _recordedSteps.Add(new StepFrame(step, time - _firstStepTime));
        availableSteps--;
        // Info in the interface about steps left
    }

    private void SpawnMarionette()
    {
        GameObject marionette = Instantiate(marionetteClone, player.playerPositions[_marionetteSpawnIndex].position , Quaternion.identity);
        _marionetteSpawnIndex++;

        marionette.TryGetComponent<CloneRecorderPlayer>(out CloneRecorderPlayer recordPlayer);

        List<StepFrame> _copiedList = new List<StepFrame>(_recordedSteps);

        recordPlayer.Begin(_copiedList);

        _recordedSteps.Clear();
        _firstStepTime = -1f;
        player.ChangePlace();
    }

    public bool UndoStep(out Vector2Int prevPos)
    {
        prevPos = Vector2Int.zero;

        if(_recordedSteps.Count == 0)
            return false;

        _recordedSteps.RemoveAt(_recordedSteps.Count - 1);
        availableSteps++;
        // change UI 

        if (_recordedSteps.Count == 0)
        {
            _firstStepTime = -1f;
            prevPos = player.gridPosition;
        }
        else prevPos = _recordedSteps[_recordedSteps.Count - 1].pos;

        return true;
    }

    private void ResetCloneRecorder()
    {
        _marionetteSpawnIndex = 0;
        marionetteCounter = _marionetteCounterForReset;
        availableSteps = _availableStepsForReset;
        // available steps and marionette UI;
        _recordedSteps.Clear();
        _firstStepTime = -1f;
    }
}
