using System.Collections.Generic;
using static Landmark;

[System.Serializable]
public class SessionData
{
    public UserData userData;
    public List<LandmarkData> landmarkData;
    public CameraData cameraData;

}