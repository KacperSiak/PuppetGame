using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public Vector2Int gridPostion;
    public float cellsize = 1f;

    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            gridPostion.y += 1;

        if (Input.GetKeyDown(KeyCode.S))
            gridPostion.y -= 1;

        if (Input.GetKeyDown(KeyCode.D))
            gridPostion.x += 1;

        if (Input.GetKeyDown(KeyCode.A))
            gridPostion.x -= 1;

        transform.position = new Vector3(gridPostion.x * cellsize, 0, gridPostion.y * cellsize);
    }
}
