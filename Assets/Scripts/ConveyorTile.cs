using UnityEngine;

public class ConveyorTile : MonoBehaviour
{
    [Tooltip("The direction this conveyor pushes (e.g., X=1 for Right, Z=1 for Forward)")]
    public Vector3Int pushDirection = Vector3Int.forward;
}