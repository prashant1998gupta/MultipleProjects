using UnityEngine;
using System.Collections.Generic;

public class CameraObstruction : MonoBehaviour
{
    public Transform target;
    public LayerMask wallMask;
    public float sampleRadius = 0.5f;
    public int sampleResolution = 1;

    private List<WallScaler> checkedWalls = new List<WallScaler>();
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    List<Vector3> GetSamplePoints(Transform target, float radius, int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        points.Add(target.position);

        for (int x = -resolution; x <= resolution; x++)
        {
            for (int y = -resolution; y <= resolution; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector3 offset = new Vector3(x, y, 0).normalized * radius;
                points.Add(target.position + offset);
            }
        }
        return points;
    }

    void LateUpdate()
    {
        // First, mark all previously checked walls as not obstructed
        foreach (var f in checkedWalls) if (f != null) f.SetObstructed(false);
        checkedWalls.Clear();

        var points = GetSamplePoints(target, sampleRadius, sampleResolution);

        foreach (var p in points)
        {
            Vector3 dir = p - cam.transform.position;
            float dist = dir.magnitude;

            RaycastHit[] hits = Physics.RaycastAll(cam.transform.position, dir.normalized, dist, wallMask);

            foreach (var hit in hits)
            {
                WallScaler f = hit.collider.GetComponent<WallScaler>();
                if (f != null && !checkedWalls.Contains(f))
                {
                    f.SetObstructed(true);
                    checkedWalls.Add(f);
                }
            }
        }
    }
}
