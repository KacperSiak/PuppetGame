using UnityEngine;

public class CoinStand : MonoBehaviour
{
    private Animator _animator;

    private bool _upIsActive;
    private bool _middleIsActive;
    private bool _downIsActive;

    private bool _isStandDown = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        FloorButton.OnButtonDown.AddListener(AddSwitch);
        FloorButton.OnButtonUp.AddListener(RemoveSwitch);
    }

    private void Update()
    {
        bool shouldBeDown = _upIsActive && _middleIsActive && _downIsActive;

        if(shouldBeDown && !_isStandDown)
        {
            _animator.SetTrigger("Up");
            _isStandDown = true;
        }
        else if (!shouldBeDown && _isStandDown)
        {
            _animator.SetTrigger("Down");
            _isStandDown = false;
        }
    }

    private void AddSwitch(int ID)
    {
        switch(ID)
        {
            case 1: 
                _upIsActive = true; 
                break;
            case 2:
                _middleIsActive = true;
                break;
            case 3:
                _downIsActive = true;
                break;
        }
    }

    private void RemoveSwitch(int ID)
    {
        switch (ID)
        {
            case 1:
                _upIsActive = false;
                break;
            case 2:
                _middleIsActive = false;
                break;
            case 3:
                _downIsActive = false;
                break;
        }
    }
}
