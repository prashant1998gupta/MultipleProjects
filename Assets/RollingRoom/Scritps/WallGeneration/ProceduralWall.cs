using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralWall : MonoBehaviour
{
    [Min(0.01f)] public float length = 3f;
    [Min(0.01f)] public float height = 2.5f;
    [Min(0.01f)] public float thickness = 0.2f;
    public bool tileUVsByMeters = true; // 1 UV = 1 meter

    Mesh _mesh;
    MeshCollider _meshCollider;

    public void InitFromPrefab(GameObject prefab)
    {
        var renderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (renderer != null)
        {
            Vector3 size = renderer.bounds.size;
            height = size.y;
            thickness = Mathf.Min(size.x, size.z); // use the thin axis as wall thickness
        }
    }

    public void SetColliderEnabled1(bool enabled)
    {
        if (enabled)
        {
            if (_meshCollider == null) _meshCollider = gameObject.GetComponent<MeshCollider>() ?? gameObject.AddComponent<MeshCollider>();
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
            _meshCollider.enabled = true;
        }
        else
        {
            if (_meshCollider != null) _meshCollider.enabled = false;
        }
    }

    public void SetColliderEnabled(bool enabled)
    {
        if (_meshCollider == null)
        {
            _meshCollider = gameObject.GetComponent<MeshCollider>();
            if (_meshCollider == null)
                _meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        if (enabled)
        {
            _meshCollider.sharedMesh = null;   // reset before re-assign
            _meshCollider.sharedMesh = _mesh;
            _meshCollider.enabled = true;
        }
        else
        {
            _meshCollider.enabled = false;
        }
    }


    public void Generate()
    {
        if (_mesh == null)
        {
            _mesh = new Mesh { name = "ProceduralWall" };
            GetComponent<MeshFilter>().sharedMesh = _mesh;
        }
        _mesh.Clear();

        float L = Mathf.Max(length, 0.01f);
        float H = Mathf.Max(height, 0.01f);
        float T = Mathf.Max(thickness, 0.01f);

        // Pivot centered like many FBX walls: x ∈ [-L/2, +L/2], y ∈ [0, H], z ∈ [-T/2, +T/2]
        float x0 = -L * 0.5f, x1 = L * 0.5f;
        float y0 = 0f, y1 = H;
        float z0 = -T * 0.5f, z1 = T * 0.5f;

        // 6 quads * 4 verts each
        var v = new Vector3[24];
        var n = new Vector3[24];
        var uv = new Vector2[24];
        int vi = 0;

        // Add a quad with vertices listed COUNTER-CLOCKWISE as seen from the outside.
        void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal, float uMeters, float vMeters)
        {
            v[vi + 0] = a; v[vi + 1] = b; v[vi + 2] = c; v[vi + 3] = d;
            n[vi + 0] = n[vi + 1] = n[vi + 2] = n[vi + 3] = normal;

            if (tileUVsByMeters)
            {
                uv[vi + 0] = new Vector2(0, 0);
                uv[vi + 1] = new Vector2(uMeters, 0);
                uv[vi + 2] = new Vector2(uMeters, vMeters);
                uv[vi + 3] = new Vector2(0, vMeters);
            }
            else
            {
                uv[vi + 0] = new Vector2(0, 0);
                uv[vi + 1] = new Vector2(1, 0);
                uv[vi + 2] = new Vector2(1, 1);
                uv[vi + 3] = new Vector2(0, 1);
            }
            vi += 4;
        }

        // Faces (CCW from outside):
        // +Z (front), -Z (back), -X (left), +X (right), -Y (bottom), +Y (top)
        Quad(new Vector3(x0, y0, z1), new Vector3(x1, y0, z1), new Vector3(x1, y1, z1), new Vector3(x0, y1, z1), Vector3.forward, L, H); // front
        Quad(new Vector3(x1, y0, z0), new Vector3(x0, y0, z0), new Vector3(x0, y1, z0), new Vector3(x1, y1, z0), Vector3.back, L, H); // back
        Quad(new Vector3(x0, y0, z0), new Vector3(x0, y0, z1), new Vector3(x0, y1, z1), new Vector3(x0, y1, z0), Vector3.left, T, H); // left
        Quad(new Vector3(x1, y0, z1), new Vector3(x1, y0, z0), new Vector3(x1, y1, z0), new Vector3(x1, y1, z1), Vector3.right, T, H); // right
        Quad(new Vector3(x0, y0, z0), new Vector3(x1, y0, z0), new Vector3(x1, y0, z1), new Vector3(x0, y0, z1), Vector3.down, L, T); // bottom
        Quad(new Vector3(x0, y1, z1), new Vector3(x1, y1, z1), new Vector3(x1, y1, z0), new Vector3(x0, y1, z0), Vector3.up, L, T); // top

        // 2 tris per quad, CCW
        int[] tris = new int[36];
        for (int f = 0; f < 6; f++)
        {
            int baseVi = f * 4;
            int ti = f * 6;
            tris[ti + 0] = baseVi + 0;
            tris[ti + 1] = baseVi + 1;
            tris[ti + 2] = baseVi + 2;
            tris[ti + 3] = baseVi + 0;
            tris[ti + 4] = baseVi + 2;
            tris[ti + 5] = baseVi + 3;
        }

        _mesh.vertices = v;
        _mesh.triangles = tris;
        _mesh.normals = n;
        _mesh.uv = uv;

        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();   // keep lighting consistent
        _mesh.RecalculateTangents();  // match FBX shading with normal maps
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying) return;
        if (GetComponent<MeshFilter>().sharedMesh == null) GetComponent<MeshFilter>().sharedMesh = new Mesh();
        Generate();
    }
#endif
}
