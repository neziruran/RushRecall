using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Image = UnityEngine.UI.Image;

public class DollyPlayer : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private FpsCamera fpsCamera;
    [SerializeField] private Canvas fakeCursor;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float camSaveInterval = 0.1f;

    // List to hold all camera samples.
    private readonly List<CameraSample> _cameraSamples = new List<CameraSample>();

    private CinemachineDollyCart _cart;
    private Coroutine _cameraRoutine;
    private float _startTime;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        StopCart();
    }

    public void StartCart(float speed = 10)
    {
        _cart = GetComponent<CinemachineDollyCart>();
        _cart.m_Path = FindObjectOfType<CinemachineSmoothPath>();
        _startTime = Time.time;
        fpsCamera.isActive = true;
        dollyCart.m_Speed = speed;
        fakeCursor.gameObject.SetActive(true);
        _cameraRoutine = StartCoroutine(CameraRoutine());
    }

    public void SetCursor(bool isLooking)
    {
        fakeCursor.transform.GetChild(1).GetComponent<Image>().color = isLooking ? Color.green : Color.white;
    }

    private void SaveCameraData()
    {
        var t = mainCamera.transform;
        var sample = new CameraSample
        {
            position = t.position,
            // Store Euler angles (in degrees) instead of a quaternion.
            rotation = t.rotation.eulerAngles,
            timestamp = Time.time - _startTime
        };
        _cameraSamples.Add(sample);
    }

    public CameraData GetCameraData()
    {
        return new CameraData
        {
            samples = _cameraSamples.ToArray()
        };
    }

    private IEnumerator CameraRoutine()
    {
        while (gameObject.activeInHierarchy)
        {
            SaveCameraData();
            yield return new WaitForSeconds(camSaveInterval);
        }
    }

    private void StopCart()
    {
        dollyCart.m_Speed = 0f;
        fpsCamera.isActive = false;
        fakeCursor.gameObject.SetActive(false);
        if (_cameraRoutine != null)
        {
            StopCoroutine(_cameraRoutine);
            _cameraRoutine = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Landmark landmark))
        {
            if (!landmark.HasRecognized)
                EventManager.TriggerLandmarkVisited(landmark);
        }
        
        if (other.gameObject.CompareTag("LevelEnd"))
        {
            EventManager.TriggerLevelEnd();
            fpsCamera.isActive = false;
            fakeCursor.gameObject.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
