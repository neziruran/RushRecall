[System.Serializable]
public class LandmarkData
{
    public int order;
    public SerializableVector3 actualPosition,guessedPosition;
    public float timeSpentOnGuessing;
    public float impressionTime;
}