using System;
using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    public static UnityEvent OnButtnTrigger = new UnityEvent();
    private float _detectionRange = 2f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ButtonPressed();
    }

    private void ButtonPressed()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, _detectionRange))
        {
            if (hit.collider.TryGetComponent<CharacterMovement>(out CharacterMovement player))
            {
                Debug.Log("Gracz trafiony");
                OnButtnTrigger?.Invoke();
            }
        }
    }

}
