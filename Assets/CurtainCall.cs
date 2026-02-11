using UnityEngine;

public class CurtainCall : MonoBehaviour
{
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        FinalDestination.OnLevelEnd.AddListener(CloseTheCurtains);
    }
    private void Start()
    {
        animator.SetTrigger("Open");
    }

    private void CloseTheCurtains()
    {
        animator.SetTrigger("Close");
    }
}
