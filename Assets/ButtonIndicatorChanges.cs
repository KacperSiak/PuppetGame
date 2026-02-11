using System;
using UnityEngine;

public class ButtonIndicatorChanges : MonoBehaviour
{
    [SerializeField] private Material green;
    [SerializeField] private Material red;
    [SerializeField] private int IDNeeded;

    private MeshRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        FloorButton.OnButtonDown.AddListener(GreenLight);
        FloorButton.OnButtonUp.AddListener(RedLight);
    }

    private void RedLight(int ID)
    {
        if(ID == IDNeeded)
        {
            ChangeMaterial(red);
        }
    }

    private void GreenLight(int ID)
    {
        if (ID == IDNeeded)
        {
            ChangeMaterial(green);
        }
    }

    private void ChangeMaterial(Material neededMaterial)
    {
        if (_renderer == null || neededMaterial == null) return;

        Material[] currentMats = _renderer.sharedMaterials;

        for(int i=0; i<currentMats.Length; i++)
        {
            currentMats[i] = neededMaterial;
        }

        _renderer.sharedMaterials = currentMats;
    }
}
