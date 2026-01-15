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

    // the spaghetti gods demand another sacrifice - record the player start position because for some reason the undo system can't undo the first move (similar issue to the CloneRecorderPlayer)
    private Vector2Int _startLoopPosition;

    private void Start()
    {
        _initialMarionetteCounter = marionetteCounter;
        _initialAvailableSteps = availableSteps;

        // spaghetti initialize start pos
        _startLoopPosition = player.gridPosition;

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

        // spaghetti after teleporting save new position as start position
        _startLoopPosition = player.gridPosition;
    }

    public bool UndoStep(out Vector2Int prevPos)
    {
        prevPos = Vector2Int.zero;

        if (_recordedSteps.Count == 0)
            return false;

        _recordedSteps.RemoveAt(_recordedSteps.Count - 1);
        availableSteps++;
        // change UI 

        // spaghetti fixed undo logic (i hope)
        if (_recordedSteps.Count == 0)
        {
            // if no more undo, return to saved start pos (fixing the first step undo bug)
            prevPos = _startLoopPosition;
        }
        else
        {
            // else do normal logic and go to the last recorded step
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

        // reset the start anchor to the level's true start
        // (assuming player script resets position before this is called, 
        // otherwise might need to grab the position from playerPositions[0])
        _startLoopPosition = player.gridPosition;
    }
}