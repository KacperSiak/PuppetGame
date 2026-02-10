using System;
using UnityEngine;
using UnityEngine.Events;

public class FinalDestination : MonoBehaviour
{
    public static UnityEvent OnLevelEnd = new UnityEvent();

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterMovement>(out var movement ))
        {
            Debug.Log("Zamykamy kurwa impreze");
            OnLevelEnd?.Invoke();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
}
