using UnityEngine;

public class GateController : MonoBehaviour
{
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
            Debug.Log("Otwieram brame");
        }
        else if (!shouldBeOpen && _isGateOpen)
        {
            _animator.SetTrigger("Down");
            _isGateOpen = false;
        }

    }

    private void AddSwitch(int ID)
    {
        if (ID == 2) { _left = true; Debug.Log("Lewy w³¹czony"); }
        else if (ID == 3) { _right = true; Debug.Log("Prawy w³¹czony"); }
    }

    private void RemoveSwitch(int ID)
    {
        if (ID == 2) { _left = false;  Debug.Log("Lewy wy³¹czony"); }
        else if (ID == 3) { _right = false; Debug.Log("Prawy wy³¹czony"); }
    }


}
