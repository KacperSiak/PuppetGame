using System;
using UnityEngine;
using UnityEngine.Events;

public class FloorButton : MonoBehaviour
{
    [SerializeField] private int buttonID;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material pressMaterial;
    [SerializeField] private Material notPressMaterial;

    public static UnityEvent<int> OnButtonDown = new UnityEvent<int>();
    public static UnityEvent<int> OnButtonUp = new UnityEvent<int>();

    private float _detectionRange = 2f;
    private MeshRenderer _renderer;



    private bool _isSomethingOn;

    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ButtonPressed();
    }

    private void ButtonPressed()
    {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, Vector3.up, out hit, _detectionRange);

        bool isValid = hitSomething && hit.collider.GetComponentInParent<IPressable>() != null;

        if(isValid && !_isSomethingOn)
        {
            _isSomethingOn = true;
            OnButtonDown?.Invoke(buttonID);
            if (pressMaterial != null) _renderer.material = pressMaterial;
        }
        else if (!isValid && _isSomethingOn)
        {
            _isSomethingOn= false;
            OnButtonUp?.Invoke(buttonID);
            if (baseMaterial != null) _renderer.material = baseMaterial;
        }    
    }
}
