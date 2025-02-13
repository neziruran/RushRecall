using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Material previewMaterial;
    [SerializeField] private LayerMask gridLayer;
    [SerializeField] private Camera targetCamera;

    private Landmark _selectedLandmark;
    private GameObject _currentPreview;
    // Variable to track when the user selects a landmark.
    private float _landmarkSelectionStartTime;

    private void Update()
    {
        if (_selectedLandmark == null) return;

        UpdateBuildingPreview();
        HandleBuildingPlacement();
    }

    public void SelectBuilding(Landmark targetLandmark)
    {
        if (_selectedLandmark == targetLandmark)
        {
            DeselectBuilding();
            return;
        }

        _selectedLandmark = targetLandmark;
        _landmarkSelectionStartTime = Time.time; // Start tracking time on selection.
        CreatePreview();
    }

    private void CreatePreview()
    {
        if (_currentPreview != null) 
            Destroy(_currentPreview);

        _currentPreview = Instantiate(_selectedLandmark.Prefab);
        foreach (Renderer r in _currentPreview.GetComponentsInChildren<Renderer>())
        {
            r.material = previewMaterial;
        }

        // Disable colliders in preview.
        foreach (Collider c in _currentPreview.GetComponentsInChildren<Collider>())
        {
            c.enabled = false;
        }
    }

    private void UpdateBuildingPreview()
    {
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gridLayer))
        {
            var hitPosition = hit.point;
            Vector3 gridPosition = new Vector3(hitPosition.x, 0, hitPosition.z);
            _currentPreview.transform.position = gridPosition;        
        }
    }
    
    private void HandleBuildingPlacement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, gridLayer))
            {
                var hitPosition = hit.point;
                float placementDuration = Time.time - _landmarkSelectionStartTime;
                Vector3 position = new Vector3(hitPosition.x, 0, hitPosition.z);
                PlaceBuilding(position,placementDuration);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            DeselectBuilding();
        }
    }
    
    private void PlaceBuilding(Vector3 position,float placementDuration)
    {
        _selectedLandmark.OnPlaced(position,placementDuration);
        Instantiate(_selectedLandmark.Prefab, position, Quaternion.identity);
        EventManager.TriggerOnLandmarkPlaced(_selectedLandmark);
        DeselectBuilding();
    }
    
    private void DeselectBuilding()
    {
        _selectedLandmark = null;
        _landmarkSelectionStartTime = 0f; // Reset the timer.
        Destroy(_currentPreview);
    }
}
