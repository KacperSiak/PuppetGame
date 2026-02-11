using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FinalDestination : MonoBehaviour
{
    public static UnityEvent OnLevelEnd = new UnityEvent();
    [SerializeField] private GameObject endPanel;

    private void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterMovement>(out var movement ))
        {
            OnLevelEnd?.Invoke();
            StartCoroutine(WaitForCurtain());
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }

    IEnumerator WaitForCurtain()
    {
        yield return new WaitForSeconds(7f);
        endPanel.SetActive(true);
    }
}
