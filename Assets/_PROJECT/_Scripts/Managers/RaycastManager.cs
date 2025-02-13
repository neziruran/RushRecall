using UnityEngine;
using System.Collections;

public class RaycastManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float interval = 0.1f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask layer;
    [SerializeField] private DollyPlayer dollyPlayer;

    private Coroutine _raycastRoutine;
    private Landmark _currentLandmark; 

    private int _nextExpectedLandmarkOrder = 0;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        EventManager.OnLevelStart += OnLevelStart;
        EventManager.OnLevelEnd += OnLevelEnd;
    }

    private void OnDisable()
    {
        EventManager.OnLevelStart -= OnLevelStart;
        EventManager.OnLevelEnd -= OnLevelEnd;
    }

    private void OnLevelStart()
    {
        _nextExpectedLandmarkOrder = 1;
        _raycastRoutine = StartCoroutine(RaycastLoop());
    }

    private void OnLevelEnd()
    {
        if (_raycastRoutine != null)
        {
            StopCoroutine(_raycastRoutine);
            _raycastRoutine = null;
        }

        if (_currentLandmark != null)
        {
            _currentLandmark.OnNotLookingAt();
            _currentLandmark = null;
        }
    }

    private IEnumerator RaycastLoop()
    {
        while (gameObject.activeInHierarchy)
        {
            PerformRaycast();
            yield return new WaitForSeconds(interval);
        }
    }

    private void PerformRaycast()
    {
        var t = mainCamera.transform;
        Ray ray = new Ray(t.position, t.forward);

        if (Physics.Raycast(ray, out var hit, maxDistance, layer))
        {
            if (hit.collider.TryGetComponent(out Landmark landmark))
            {
                // Case 1: The hit landmark is the next expected landmark.
                if (landmark.order == _nextExpectedLandmarkOrder)
                {
                    if (_currentLandmark != landmark)
                    {
                        if (_currentLandmark != null)
                        {
                            _currentLandmark.OnNotLookingAt();
                            dollyPlayer.SetCursor(false);
                        }
                        _currentLandmark = landmark;
                        landmark.OnLookingAt();
                        dollyPlayer.SetCursor(true);
                    }
                    if (!landmark.HasRecognized)
                    {
                        EventManager.TriggerLandmarkVisited(landmark);
                        _nextExpectedLandmarkOrder++;
                    }
                }
                // Case 2: The hit landmark is a previous one that has already been recognized.
                else if (landmark.order < _nextExpectedLandmarkOrder && landmark.HasRecognized)
                {
                    if (_currentLandmark != landmark)
                    {
                        if (_currentLandmark != null)
                        {
                            _currentLandmark.OnNotLookingAt();
                        }
                        _currentLandmark = landmark;
                        landmark.OnLookingAt();
                        dollyPlayer.SetCursor(true);
                    }
                }
                // Case 3: The hit landmark is in the future (order > expected).
                else if (landmark.order > _nextExpectedLandmarkOrder)
                {
                    if (_currentLandmark != null)
                    {
                        _currentLandmark.OnNotLookingAt();
                        _currentLandmark = null;
                        dollyPlayer.SetCursor(false);
                    }
                }
            }
        }
        else
        {
            if (_currentLandmark != null)
            {
                _currentLandmark.OnNotLookingAt();
                _currentLandmark = null;
                dollyPlayer.SetCursor(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null)
            return;

        Gizmos.color = Color.red;
        var t = mainCamera.transform;
        Gizmos.DrawRay(t.position, t.forward * maxDistance);
    }
}
