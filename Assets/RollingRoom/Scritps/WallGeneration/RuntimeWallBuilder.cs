using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RuntimeWallBuilder : MonoBehaviour
{
    [Header("Scene")]
    public Camera cam;
    public LayerMask groundMask;       // set your ground layer(s)

    [Header("Wall Settings")]
    public Material finalMaterial;
    public Material previewMaterial;   // transparent/ghost
    public float height = 2.5f;
    public float thickness = 0.2f;

    [Header("Placement")]
    public float minLength = 0.1f;
    public float gridSize = 0.5f;      // 0 = no snapping
    public KeyCode buildMouseButton = KeyCode.Mouse0;
    public KeyCode undoKey = KeyCode.Z;   // Ctrl+Z or just Z (see below)
    public bool requireCtrlForUndo = true;

    Vector3 _startPos;
    bool _dragging;
    GameObject _previewGO;
    ProceduralWall _previewWall;
    readonly Stack<GameObject> _placed = new Stack<GameObject>();

    void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // ignore over UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        // start drag
        if (!_dragging && GetMouseDown(buildMouseButton) && RayToGround(out var hitStart))
        {
            _dragging = true;
            _startPos = Snap(hitStart);

            _previewGO = new GameObject("Wall_Preview");
            _previewGO.transform.position = _startPos;

            _previewWall = _previewGO.AddComponent<ProceduralWall>();
            _previewWall.length = minLength;
            _previewWall.height = height;
            _previewWall.thickness = thickness;

            var mf = _previewGO.AddComponent<MeshFilter>();
            var mr = _previewGO.AddComponent<MeshRenderer>();
            mr.sharedMaterial = previewMaterial;

            _previewWall.Generate();
            _previewWall.SetColliderEnabled(false);
        }

        // update drag
        // update drag
        if (_dragging && _previewGO != null && _previewWall != null &&
            Input.GetKey(buildMouseButton) && RayToGround(out var hitDrag))
        {
            Vector3 end = Snap(hitDrag);
            Vector3 dir = end - _startPos;
            dir.y = 0;

            float len = Mathf.Max(dir.magnitude, minLength);
            if (dir.sqrMagnitude > 1e-6f)
                _previewGO.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

            _previewGO.transform.position = _startPos;
            _previewWall.length = len;
            _previewWall.height = height;
            _previewWall.thickness = thickness;
            _previewWall.Generate();
        }


        // finish drag
        if (_dragging && GetMouseUp(buildMouseButton))
        {
            _dragging = false;

            // too short? cancel
            if (_previewWall.length < minLength + 1e-3f)
            {
                Destroy(_previewGO);
                _previewGO = null;
                _previewWall = null;
                return;
            }

            // finalize
            var mr = _previewGO.GetComponent<MeshRenderer>();
            mr.sharedMaterial = finalMaterial;

            _previewWall.SetColliderEnabled(true);

            _previewGO.name = "Wall_Segment";
            _placed.Push(_previewGO);

            _previewGO = null;
            _previewWall = null;
        }

        // undo
        if (PressedUndo())
        {
            if (_placed.Count > 0)
            {
                var go = _placed.Pop();
                if (go) Destroy(go);
            }
        }
    }

    bool RayToGround(out Vector3 pos)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 500f, groundMask))
        {
            pos = hit.point;
            return true;
        }
        pos = Vector3.zero;
        return false;
    }

    Vector3 Snap(Vector3 p)
    {
        if (gridSize <= 0f) return p;
        float x = Mathf.Round(p.x / gridSize) * gridSize;
        float y = Mathf.Round(p.y / gridSize) * gridSize;
        float z = Mathf.Round(p.z / gridSize) * gridSize;
        return new Vector3(x, y, z);
    }

    bool GetMouseDown(KeyCode btn) => btn == KeyCode.Mouse0 ? Input.GetMouseButtonDown(0) : Input.GetKeyDown(btn);
    bool GetMouseUp(KeyCode btn) => btn == KeyCode.Mouse0 ? Input.GetMouseButtonUp(0) : Input.GetKeyUp(btn);

    bool PressedUndo()
    {
        if (requireCtrlForUndo)
            return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(undoKey);
        else
            return Input.GetKeyDown(undoKey);
    }
}
