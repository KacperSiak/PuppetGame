using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField] private GameObject gateBlocker;

    private Animator _animator;
    private bool _left;
    private bool _right;
    private bool _isGateOpen = false;


    private void Start()
    {
        _animator = GetComponent<Animator>();
        FloorButton.OnButtonDown.AddListener(AddSwitch);
        FloorButton.OnButtonUp.AddListener(RemoveSwitch);
    }

    private void Update()
    {
        bool shouldBeOpen = _left && _right;

        if (shouldBeOpen && !_isGateOpen)
        {
            _animator.SetTrigger("Up");
            _isGateOpen = true;
            gateBlocker.SetActive(false);
        }
        else if (!shouldBeOpen && _isGateOpen)
        {
            _animator.SetTrigger("Down");
            _isGateOpen = false;
            gateBlocker.SetActive(true);
        }

    }

    private void AddSwitch(int ID)
    {
        if (ID == 2) _left = true;
        else if (ID == 3) _right = true; 
    }

    private void RemoveSwitch(int ID)
    {
        if (ID == 2) _left = false;
        else if (ID == 3) _right = false;
    }


}
