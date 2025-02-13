[System.Serializable]
public class CameraSample
{
    public SerializableVector3 position;
    public SerializableVector3 rotation;
    public float timestamp;
}

[System.Serializable]
public class CameraData
{
    public CameraSample[] samples;
}