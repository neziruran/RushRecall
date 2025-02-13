using UnityEngine;
using UnityEngine.UI;

public class Landmark : MonoBehaviour
{
    public int order;
    public bool HasRecognized{ get; private set; }
    public GameObject Prefab { get; set; }
    public Button LandmarkButton { get; set; }

    public LandmarkData landmarkData;
    
    private bool _isBeingLookedAt;
    private float _lookStartTime;
    private float _lookEndTime;
    
    
    private void Awake()
    {
        EventManager.OnLandmarkVisited += OnVisited;
    }

    private void Start()
    {
        EventManager.OnLandmarkVisited += OnVisited;
    }


    private void OnEnable()
    {
        EventManager.OnLandmarkVisited += OnVisited;
    }

    private void OnDisable()
    {
        EventManager.OnLandmarkVisited -= OnVisited;
    }


    private void OnDestroy()
    {
        EventManager.ClearAllSubscriptions();
    }
    public void Initialize()
    {
        landmarkData.actualPosition = transform.position;
        
        HasRecognized = false;

        var childCollider = transform.GetChild(0).GetComponent<BoxCollider>();
        var selfCollider = GetComponent<BoxCollider>();

        if (childCollider != null && selfCollider != null)
        {
            selfCollider.center = childCollider.center;
            selfCollider.size = childCollider.size;
        }
        else
        {
            Debug.LogError("One or both BoxCollider components are missing.");
        }
    }


    public void AutoRecognize()
    {
        HasRecognized = true;
    }
    
    public void OnLookingAt()
    {
        if (!_isBeingLookedAt)
        {
            _isBeingLookedAt = true;
            _lookStartTime = Time.time;
        }
    }

    public void OnNotLookingAt()
    {
        if (_isBeingLookedAt)
        {
            _isBeingLookedAt = false;
            _lookEndTime = Time.time;
            CalculateImpressionTime();
        }
    }

    private void CalculateImpressionTime()
    {
        landmarkData.impressionTime += (_lookEndTime - _lookStartTime);
    }
    
    public void OnPlaced(SerializableVector3 position,float timeSpentOnGuessing)
    {
        LandmarkButton.gameObject.SetActive(false);
        landmarkData.guessedPosition = position;
        landmarkData.order = order;
        landmarkData.timeSpentOnGuessing = timeSpentOnGuessing;
    }
    
    private void OnVisited(Landmark landmark)
    {
        if (landmark == this)
        {
            LandmarkButton.gameObject.SetActive(true);
            LandmarkButton.GetComponent<LandmarkButton>().ActivateButton(this);
            HasRecognized = true;
        }
    }
}