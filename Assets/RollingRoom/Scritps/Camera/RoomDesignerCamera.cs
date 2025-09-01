using UnityEngine;

public class OrbitCameraAppleMouse : MonoBehaviour
{
    public Transform target;
    public float distance = 10f;
    public float zoomSpeed = 2f;
    public float minDistance = 5f;
    public float maxDistance = 25f;

    public float xSpeed = 120f;
    public float ySpeed = 80f;

    public float yMinLimit = 10f;
    public float yMaxLimit = 80f;

    public float panSpeed = 0.5f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        HandleZoom();
        HandleRotationOrPan();

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }

    void HandleRotationOrPan()
    {
        bool isCmdPressed = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);

        if (Input.GetMouseButton(0)) // Left click
        {
            float moveX = Input.GetAxis("Mouse X");
            float moveY = Input.GetAxis("Mouse Y");

            if (isCmdPressed)
            {
                // Pan mode
                Vector3 right = transform.right;
                Vector3 up = transform.up;

                Vector3 pan = -right * moveX * panSpeed - up * moveY * panSpeed;
                target.position += pan;
            }
            else
            {
                // Rotate mode
                x += moveX * xSpeed * Time.deltaTime;
                y -= moveY * ySpeed * Time.deltaTime;
                y = Mathf.Clamp(y, yMinLimit, yMaxLimit);
            }
        }
    }
}
