using System;
using UnityEngine;
using UnityEngine.UI;
using static Landmark;

public class LandmarkButton : MonoBehaviour
{
    [Header("Grid Placement")] 
    [SerializeField] private PlacementManager placementManager;
    [SerializeField] private Landmark landmark;

    public void ActivateButton(Landmark targetLandmark)
    {
        GetComponent<Image>().enabled = true;
        landmark = targetLandmark;
    }
    public void SelectBuilding()
    {
        placementManager.SelectBuilding(landmark);
    }
}