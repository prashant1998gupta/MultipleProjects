using UnityEngine;
using System.Collections.Generic;

public class GravityManager : MonoBehaviour
{
    public float gravitationalConstant = 0.1f; // Tune this for stable orbits
    private Planet[] planets;

    void FixedUpdate()
    {
        planets = FindObjectsOfType<Planet>();

        foreach (Planet a in planets)
        {
            foreach (Planet b in planets)
            {
                if (a == b) continue;

                Vector3 direction = b.transform.position - a.transform.position;
                float distance = direction.magnitude;

                if (distance == 0) continue;

                float forceMagnitude = gravitationalConstant * (a.mass * b.mass) / (distance * distance);
                Vector3 force = direction.normalized * forceMagnitude;

                a.rb.AddForce(force);
            }
        }
    }
}
