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

    private List<StepFrame> _recordedSteps = new List<StepFrame>();
    private int _marionetteSpawnIndex = 0;

    // RESET STORAGE
    private int _initialMarionetteCounter;
    private int _initialAvailableSteps;

    private void Start()
    {
        _initialMarionetteCounter = marionetteCounter;
        _initialAvailableSteps = availableSteps;

        CharacterMovement.OnReset.AddListener(ResetCloneRecorder);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && marionetteCounter > 0)
        {
            SpawnMarionette();
            marionetteCounter--;
        }
    }

    // CALLED BY CharacterMovement
    public void RecordStep(Vector2Int step)
    {
        _recordedSteps.Add(new StepFrame(step));
        availableSteps--;
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

        _recordedSteps.Clear();
    }
}