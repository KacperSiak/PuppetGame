using UnityEngine;

public class Bridge : MonoBehaviour
{
    [SerializeField] private int IDNeed;
    [SerializeField] private GameObject bridgeCollider;

    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
        FloorButton.OnButtonDown.AddListener(BridgeUp);
        FloorButton.OnButtonUp.AddListener(BridgeDown);
    }

    private void BridgeUp(int ID)
    {
        if (ID == IDNeed)
        {
            _anim.SetTrigger("Open");
            bridgeCollider.SetActive(false);
        }
    }

    private void BridgeDown(int ID)
    {
        if (ID == IDNeed)
        {
            _anim.SetTrigger("Close");
            bridgeCollider.SetActive(true);
        }
    }
}
