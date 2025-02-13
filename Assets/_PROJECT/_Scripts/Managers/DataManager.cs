using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Landmark;

public class DataManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private SessionData currentSession;

    
    private void OnEnable()
    {
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnLevelEnd += OnLevelEnd;
        EventManager.OnLandmarkVisited += OnLandmarkVisited;

    }


    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
        EventManager.OnLevelEnd -= OnLevelEnd;
        EventManager.OnLandmarkVisited -= OnLandmarkVisited;

    }

    private void OnLandmarkVisited(Landmark landmark)
    {
        currentSession.landmarkData.Add(landmark.landmarkData);
    }

    private void SaveCameraData()
    {
        var cam = Camera.main;
        if(cam == null)return;
        currentSession.cameraData = gameManager.DollyPlayer.GetCameraData();
    }

    private void OnLevelStart()
    {
        currentSession = new SessionData
        {
            userData = new UserData()
            {
                userName = gameManager.Username,
                simulationSpeed = gameManager.SimulationSpeed,
                timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss"),
            },
            cameraData =  new CameraData(),
            landmarkData = new List<LandmarkData>(),

        };
    }

    private void OnLevelEnd()
    {
        SaveCameraData();
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(currentSession, true);
        string directoryPath = Path.Combine(Application.dataPath, "_PROJECT", "Data");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, currentSession.userData.userName + "_" + currentSession.userData.timestamp + ".json");
        File.WriteAllText(filePath, json);
    }
}