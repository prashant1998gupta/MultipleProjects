using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass = 1f;
    public Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We'll simulate gravity manually
    }
}
