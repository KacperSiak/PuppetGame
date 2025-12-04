using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    public Vector2Int gridPosition;
    public float cellsize = 1f;
    public List<Transform> playerPositions = new List<Transform>();

    public static UnityEvent<Vector2Int, float> OnStep = new UnityEvent<Vector2Int, float>();

    private int _spawnIndex = 1;

    private void Start()
    {
        _spawnIndex = 1;  
    }

    private void Update()
    {
        Vector2Int oldPosition = gridPosition;

        if (Input.GetKeyDown(KeyCode.W))
            gridPosition.y += 1;

        if (Input.GetKeyDown(KeyCode.S))
            gridPosition.y -= 1;

        if (Input.GetKeyDown(KeyCode.D))
            gridPosition.x += 1;

        if (Input.GetKeyDown(KeyCode.A))
            gridPosition.x -= 1;

        if (gridPosition != oldPosition)
        {
            OnStep?.Invoke(gridPosition, Time.time);
        }

        transform.position = new Vector3(gridPosition.x * cellsize, 0, gridPosition.y * cellsize);
    }

    public void ChangePlace()
    {
        Transform destination = playerPositions[_spawnIndex];
        transform.position = destination.position;

        gridPosition = new Vector2Int(
            Mathf.RoundToInt(destination.position.x / cellsize),
            Mathf.RoundToInt(destination.position.z / cellsize)
            );
        _spawnIndex++;
    }
}
