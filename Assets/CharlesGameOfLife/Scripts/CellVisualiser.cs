using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellVisualiser : MonoBehaviour
{
    [SerializeField]
    Material activeMaterial;
    [SerializeField]
    Material inactiveMaterial;

    [SerializeField]
    Image _renderer;

    [SerializeField]
    bool showNeighbourCount = true;
    [SerializeField]
    TextMeshProUGUI textMesh;
    [SerializeField]
    List<CellVisualiser> Neighbours = new List<CellVisualiser>();
    public bool isActive;

    private void Awake()
    {
        _renderer.material = inactiveMaterial;
    }

    public void SetCellActive(bool v)
    {
        isActive = v;

        if (v)
        {
            _renderer.material = activeMaterial;
        }
        else
        {
            _renderer.material = inactiveMaterial;
        }
    }

    public void DisplayNeighbours(int count)
    {
        if (textMesh && showNeighbourCount)
        {
            textMesh.text = count.ToString();
        }
    }

    public void SetNeighbours(List<CellVisualiser> newNeighbours)
    {
        Neighbours = newNeighbours;
    }
}
