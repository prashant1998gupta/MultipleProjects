using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float magnetRadius = 5f;
    public float magnetForce = 10f;
    public LayerMask attractionLayer; // Set this to only include layers with objects you want to attract
    public float snapDistance = 0.5f; // Distance to snap and stick
    public Collider[] colliders;

    void FixedUpdate()
    {
         colliders = Physics.OverlapSphere(transform.position, magnetRadius, attractionLayer);

        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 direction = (transform.position + rb.position).normalized;
                rb.AddForce(direction * magnetForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            }

            /*  float distance = Vector3.Distance(transform.position, rb.position);

              if (distance > snapDistance)
              {
                  // Attract the object
                  Vector3 direction = (transform.position - rb.position).normalized;
                  rb.AddForce(direction * magnetForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
              }
              else
              {
                  // Snap and stick
                  rb.linearVelocity = Vector3.zero;
                  rb.angularVelocity = Vector3.zero;
                  rb.isKinematic = true; // Makes it stop being affected by physics
                  rb.transform.position = transform.position; // Snap to magnet position
              }*/
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, snapDistance);
    }
}
    