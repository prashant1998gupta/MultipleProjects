using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeCameraController : MonoBehaviour
{
    private enum CameraMode { Orbit, FreeFly }
    [SerializeField] private CameraMode currentMode = CameraMode.Orbit;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public float fastSpeedMultiplier = 3f;
    public float smoothTime = 0.1f;
    public float scrollSpeed = 10f;
    public float minZoomDistance = 2f;
    public float maxZoomDistance = 100f;    

    [Header("Rotation Settings")]
    public float mouseSensitivity = 3f;
    public float rotationSmoothTime = 0.05f;
    public float pitchMin = -80f;
    public float pitchMax = 80f;
    public float panSpeed = 0.5f;
    public bool invertY = false;

    [SerializeField] private Vector3 orbitPivot = Vector3.zero;
    [SerializeField] private float orbitDistance = 10f;

    private Vector3 currentVelocity;
    private Vector3 targetPosition;

    private float yaw;
    private float pitch;
    private Vector2 currentRotation;
    private Vector2 rotationSmoothVelocity;

    private bool isFast = false;

    void Start()
    {
        targetPosition = transform.position;
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    void Update()
    {
        HandleCursorLock();
        HandleSpeedToggle();

        if (Input.GetKeyDown(KeyCode.Tab)) //Press Tab to switch mode
        {
            currentMode = (currentMode == CameraMode.Orbit) ? CameraMode.FreeFly : CameraMode.Orbit;
        }

        if (currentMode == CameraMode.Orbit)
        {
            HandlePan();
            HandleRotation();
            HandleScrollZoom();
        }
        else
        {
            HandleFreeFlyRotation(); // new for free look
            HandleMovement(); // WASD movement
        }

        SmoothMove();
        SmoothRotate();
    }

    void HandleCursorLock()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HandleSpeedToggle()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFast = !isFast;
        }
    }

    void HandleMovement()
    {
        float speed = moveSpeed * (isFast || Input.GetKey(KeyCode.LeftShift) ? fastSpeedMultiplier : 1f);

        Vector3 direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.E)) direction += transform.up;
        if (Input.GetKey(KeyCode.Q)) direction -= transform.up;

        Vector3 move = direction.normalized * speed * Time.deltaTime;

        //Move the orbit pivot instead of the camera
        orbitPivot += move;
        targetPosition += move; // Optional: move camera with pivot directly if zooming is mid-transition
    }

    void HandleFreeFlyRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);

            yaw += mouseX;
            pitch += mouseY;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * (invertY ? 1 : -1);

            yaw += mouseX;
            pitch += mouseY;
            pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
        }
    }

    void HandleScrollZoom()
    {
        if (ItemSpawner.IsPreviewing) return;


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 direction = transform.forward * scroll * scrollSpeed;
            Vector3 newPosition = targetPosition + direction;

            float newDistance = Vector3.Distance(newPosition, orbitPivot); // Use orbitPivot instead of Vector3.zero
            if (newDistance >= minZoomDistance && newDistance <= maxZoomDistance)
            {
                orbitDistance = newDistance; // Dynamically update orbitDistance
                targetPosition = orbitPivot + (transform.rotation * new Vector3(0f, 0f, -orbitDistance));
            }
        }
    }

    void HandlePan()
    {
        if (Input.GetMouseButton(2)) // Middle mouse button
        {
            //float panSpeed = orbitDistance * 0.002f; // scale pan speed based on zoom level

            float moveX = -Input.GetAxis("Mouse X") * panSpeed;
            float moveY = -Input.GetAxis("Mouse Y") * panSpeed;

            // Get camera right and up directions
            Vector3 right = transform.right;
            Vector3 up = transform.up;

            // Calculate pan movement
            Vector3 panMovement = (right * moveX) + (up * moveY);

            // Update pivot and camera position together
            orbitPivot += panMovement;
            targetPosition += panMovement;
        }
    }

    void SmoothMove()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }

    void SmoothRotate()
    {
        Vector2 targetRotation = new Vector2(pitch, yaw);
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        // Apply smoothed rotation
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);
        transform.rotation = rotation;

        // Update position using smoothed rotation
        Vector3 offset = rotation * new Vector3(0f, 0f, -orbitDistance);
        targetPosition = orbitPivot + offset;
    }
}
