using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, Range(1f, 50f), Tooltip("Camera movement speed")] 
    private float panSpeed = 20f;
    
    [SerializeField, Range(1f, 100f), Tooltip("Screen edge threshold in pixels for mouse movement")] 
    private float edgeScrollThreshold = 10f;
    
    [SerializeField, Tooltip("Enable mouse edge scrolling")] 
    private bool useMouseMovement = true;

    [Header("Position Constraints")]
    [SerializeField, Tooltip("X-axis position constraints (min, max)")]
    private Vector2 xClamp = new(-60f, 60f);
    
    [SerializeField, Tooltip("Z-axis position constraints (min, max)")]
    private Vector2 zClamp = new(-30f, 0f);

    [SerializeField] private Vector3 originalRotation;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnEnable()
    {
        transform.rotation = Quaternion.Euler(originalRotation);
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 movementDirection = GetMovementDirection();
        Vector3 scaledMovement = movementDirection * (panSpeed * Time.deltaTime);
        Vector3 newPosition = _transform.position + scaledMovement;

        newPosition.x = Mathf.Clamp(newPosition.x, xClamp.x, xClamp.y);
        newPosition.z = Mathf.Clamp(newPosition.z, zClamp.x, zClamp.y);

        _transform.position = newPosition;
    }

    private Vector3 GetMovementDirection()
    {
        Vector2 inputAxes = GetInputAxes();
        Vector3 forward = GetFlattenedDirection(_transform.forward);
        Vector3 right = GetFlattenedDirection(_transform.right);

        return forward * inputAxes.y + right * inputAxes.x;
    }

    private Vector2 GetInputAxes()
    {
        Vector2 input = Vector2.zero;

        // Keyboard input
        input.y += Input.GetKey(KeyCode.W) ? 1 : 0;
        input.y -= Input.GetKey(KeyCode.S) ? 1 : 0;
        input.x += Input.GetKey(KeyCode.D) ? 1 : 0;
        input.x -= Input.GetKey(KeyCode.A) ? 1 : 0;

        // Edge scrolling
        if (useMouseMovement)
        {
            Vector2 mouseInput = GetMouseEdgeInput();
            input += mouseInput;
        }

        return Vector2.ClampMagnitude(input, 1f);
    }

    private Vector2 GetMouseEdgeInput()
    {
        Vector2 mouseInput = Vector2.zero;
        Vector2 mousePosition = Input.mousePosition;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Vertical movement
        if (mousePosition.y >= screenHeight - edgeScrollThreshold)
            mouseInput.y += 1;
        else if (mousePosition.y <= edgeScrollThreshold)
            mouseInput.y -= 1;

        // Horizontal movement
        if (mousePosition.x >= screenWidth - edgeScrollThreshold)
            mouseInput.x += 1;
        else if (mousePosition.x <= edgeScrollThreshold)
            mouseInput.x -= 1;

        return mouseInput;
    }

    private static Vector3 GetFlattenedDirection(Vector3 direction)
    {
        return new Vector3(direction.x, 0, direction.z).normalized;
    }
    
}