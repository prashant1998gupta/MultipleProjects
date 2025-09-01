using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    public GameObject planetPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject newPlanet = Instantiate(planetPrefab, hit.point, Quaternion.identity);
                Rigidbody rb = newPlanet.GetComponent<Rigidbody>();
                rb.linearVelocity = Random.onUnitSphere * 2f;
            }
        }
    }
}
