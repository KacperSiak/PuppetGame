using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


[System.Serializable]
public struct StepFrame
{
    public Vector2Int pos;

    public StepFrame(Vector2Int pos)
    {
        this.pos = pos;
    }
}


public class CloneRecorder : MonoBehaviour
{
    [SerializeField] private GameObject marionetteClone;
    [SerializeField] private CharacterMovement player;

    [SerializeField] private int marionetteCounter = 3;
    public int availableSteps = 20;
    // [SerializeField] private TextMeshProUGUI _marionetteText;

    private List<StepFrame> _recordedSteps = new List<StepFrame>();
    private int _marionetteSpawnIndex = 0;

    // container for reset
    private int _initialMarionetteCounter;
    private int _initialAvailableSteps;

    private void Start()
    {
        _initialMarionetteCounter = marionetteCounter;
        _initialAvailableSteps = availableSteps;

        CharacterMovement.OnReset.AddListener(ResetCloneRecorder);

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

    // previously called MadeStep, after reworking logic RecordStep fits better (change back if lead programmer directs it so)
    // function called in CharacterMovement
    public void RecordStep(Vector2Int step)
    {
        _recordedSteps.Add(new StepFrame(step));
        availableSteps--;
        // Info in the interface about steps left
    }

    private void SpawnMarionette()
    {
        GameObject marionette = Instantiate(
            marionetteClone,
            player.playerPositions[_marionetteSpawnIndex].position,
            Quaternion.identity
        );

        _marionetteSpawnIndex++;

        if (marionette.TryGetComponent(out CloneRecorderPlayer clonePlayer))
        {
            clonePlayer.Begin(new List<StepFrame>(_recordedSteps));
        }

        _recordedSteps.Clear();
        player.ChangePlace();
    }

    public bool UndoStep(out Vector2Int prevPos)
    {
        prevPos = Vector2Int.zero;

        if (_recordedSteps.Count == 0)
            return false;

        _recordedSteps.RemoveAt(_recordedSteps.Count - 1);
        availableSteps++;
        // change UI 

        if (_recordedSteps.Count == 0)
        {
            prevPos = player.gridPosition;
        }
        else
        {
            prevPos = _recordedSteps[_recordedSteps.Count - 1].pos;
        }

        return true;
    }

    private void ResetCloneRecorder()
    {
        _marionetteSpawnIndex = 0;
        marionetteCounter = _initialMarionetteCounter;
        availableSteps = _initialAvailableSteps;
        // available steps and marionette UI;

        _recordedSteps.Clear();
    }
}