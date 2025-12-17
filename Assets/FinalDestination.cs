using System;
using UnityEngine;

public class FinalDestination : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private Transform _placeToAchive;

    private void Start()
    {
        _winPanel.SetActive(false);
        transform.position = _placeToAchive.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CharacterMovement>(out var movement ))
        {
            _winPanel.SetActive(true);
            PauseGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
}
