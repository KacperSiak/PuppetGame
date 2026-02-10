using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct StepFrame
{
    public Vector3Int pos; // CHANGED: Now uses Vector3Int

    public StepFrame(Vector3Int pos)
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

    private int _initialMarionetteCounter;
    private int _initialAvailableSteps;

    // CHANGED: Now uses Vector3Int
    private Vector3Int _startLoopPosition;

    public static UnityEvent OnCloneSpawned = new UnityEvent();

    public static UnityEvent OnReplay = new UnityEvent();

    private void Start()
    {
        _initialMarionetteCounter = marionetteCounter;
        _initialAvailableSteps = availableSteps;

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

    // CHANGED: Parameter type to Vector3Int
    public void RecordStep(Vector3Int step)
    {
        _recordedSteps.Add(new StepFrame(step));
        availableSteps--;
    }

    private void SpawnMarionette()
    {
        OnReplay.Invoke();

        GameObject marionette = Instantiate(
            marionetteClone,
            player.playerPositions[_marionetteSpawnIndex].position,
            Quaternion.identity
        );

        _marionetteSpawnIndex++;

        OnCloneSpawned.Invoke();

        if (marionette.TryGetComponent(out CloneRecorderPlayer clonePlayer))
        {
            clonePlayer.Begin(new List<StepFrame>(_recordedSteps));
        }

        _recordedSteps.Clear();
        player.ChangePlace();

        _startLoopPosition = player.gridPosition;
    }

    // CHANGED: Parameter type to Vector3Int
    public bool UndoStep(out Vector3Int prevPos)
    {
        prevPos = Vector3Int.zero;

        if (_recordedSteps.Count == 0)
            return false;

        _recordedSteps.RemoveAt(_recordedSteps.Count - 1);
        availableSteps++;

        if (_recordedSteps.Count == 0)
        {
            prevPos = _startLoopPosition;
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
        _startLoopPosition = player.gridPosition;
    }
}