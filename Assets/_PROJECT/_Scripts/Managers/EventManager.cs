using System;
using UnityEngine;

public static class EventManager 
{
    // Parameter-less event declarations
    
    public static event Action OnLevelStart;
    public static event Action OnLevelEnd;
    public static event Action<Landmark> OnLandmarkVisited;
    public static event Action<Landmark> OnLandmarkPlaced;
    public static event Action OnSecondPhaseStarted;

    public static void TriggerLevelStart()
    {
        OnLevelStart?.Invoke();
        Debug.Log("Level started");
    }

    public static void TriggerLevelEnd()
    {
        OnLevelEnd?.Invoke();
        Debug.Log("Level ended");
    }

    public static void TriggerLandmarkVisited(Landmark landmark)
    {
        OnLandmarkVisited?.Invoke(landmark);
        Debug.Log("Landmark recognized");
    }
    public static void TriggerSecondPhaseStart()
    {
        OnSecondPhaseStarted?.Invoke();
        Debug.Log("Second Phase Started");
    }
    
    public static void TriggerOnLandmarkPlaced(Landmark landmark)
    {
        OnLandmarkPlaced?.Invoke(landmark);
        Debug.Log("Landmark Placed");
    }
    public static void ClearAllSubscriptions()
    {
        OnLevelStart = null;
        OnLevelEnd = null;
        OnSecondPhaseStarted = null;
        OnLandmarkVisited = null;
    }



}