using UnityEngine;

public class CurtainCall : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        FinalDestination.OnLevelEnd.AddListener(CloseTheCurtains);
    }

    private void Update()
    {
        if (animator != null && Input.GetKeyDown(KeyCode.L)) animator.SetTrigger("Open");
        if (animator != null && Input.GetKeyDown(KeyCode.M)) animator.SetTrigger("Close");
    }

    private void CloseTheCurtains()
    {
        animator.SetTrigger("Close");
    }
}
