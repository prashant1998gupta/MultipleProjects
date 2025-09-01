using UnityEngine;

public class BalloonController3D : MonoBehaviour
{
    public float buoyantForce = 10f;
    public float heatBoostForce = 20f;
    public float maxUpwardSpeed = 5f;
    public float horizontalSpeed = 3f;
    public float drag = 1f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = drag;
    }

    void FixedUpdate()
    {
        // Constant buoyancy
        rb.AddForce(Vector3.up * buoyantForce);

        // Boost when holding spacebar or mouse button
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            if (rb.linearVelocity.y < maxUpwardSpeed)
                rb.AddForce(Vector3.up * heatBoostForce);
        }

        // Optional horizontal movement with arrow keys / touch
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 horizontalMovement = new Vector3(h, 0, v) * horizontalSpeed;
        rb.AddForce(horizontalMovement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Hazard"))
        {
            Debug.Log("Balloon popped!");
            // TODO: Add restart or fail logic here
        }
    }
}
