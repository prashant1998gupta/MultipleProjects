using Unity.VisualScripting;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public LayerMask placementLayer;
    public LayerMask placementLayerWall;
    public Material previewMaterial;


    public float rotationSpeed = 10f;
    public float scaleSpeed = 0.1f;
    public float minScale = 0.3f;
    public float maxScale = 3f;
    private bool isPreviewing => previewInstance != null;


    [Header("Auto-Rotate Settings")]
    public bool enableRotationAlignment = true;
    public float rotationSnapThreshold = 10f;


    private ItemData selectedItem;
    private int index;
    private GameObject previewInstance;
    private Renderer[] previewRenderers;

    public static ItemSpawner Instance { get; private set; }
    public static bool IsPreviewing => Instance?.previewInstance != null;

    void Awake()
    {
        Instance = this;
    }


    void Update()
    {
        if (selectedItem == null)
        {
            DestroyPreview();
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayer | placementLayerWall))
        {
          

            if (previewInstance == null)
                CreatePreview();



           
            if (((1 << hit.collider.gameObject.layer) & placementLayerWall) != 0)
            {
                // Wall detected
                Vector3 wallSnap = hit.point + hit.normal * 0.01f;
                Vector3 wallNormal = hit.normal;
                Quaternion wallFacingRotation = Quaternion.LookRotation(wallNormal * -1f, Vector3.up);

                previewInstance.transform.position = wallSnap;
                previewInstance.transform.rotation = wallFacingRotation;
            }
            else
            {
                // Floor/grid
                Vector3 gridSnap = GridSnapper.Snap(hit.point);
                previewInstance.transform.position = gridSnap;
            }

            TrySnapToNearbyFurniture();

            if (isPreviewing)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");

                if (Mathf.Abs(scroll) > 0.01f)
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        // Scale
                        float newScale = Mathf.Clamp(previewInstance.transform.localScale.x + scroll * scaleSpeed, minScale, maxScale);
                        previewInstance.transform.localScale = Vector3.one * newScale;
                    }
                    else
                    {
                        // Rotate Y
                        previewInstance.transform.Rotate(Vector3.up, scroll * rotationSpeed, Space.World);
                    }
                }
            }

            //bool isValid = IsValidPlacement(previewInstance);
            bool isValid = true;

            if (!isValid)
            {
              SetPreviewColor(Color.cyan);
            }
            else
            {
                SetPreviewDefault();

                if (Input.GetMouseButtonDown(0))
                {
                    //GameObject placed = Instantiate(selectedItem.prefabVariants[index], snapPos, Quaternion.identity);
                    GameObject placed = Instantiate(selectedItem.prefabVariants[index], previewInstance.transform.position, previewInstance.transform.rotation);
                    placed.transform.localScale = previewInstance.transform.localScale;
                    SetLayerRecursively(placed, LayerMask.NameToLayer("PlacedItem"));
                    //AddSelectableItemRuntime(placed); // Add SelectableItem component to the new object
                    //Instantiate(selectedItem.prefab, snapPos, Quaternion.identity);
                    DestroyPreview();         // Remove ghost
                    selectedItem = null;      // Reset selection ✅
                }
            }

           
        }
        else
        {
            DestroyPreview();
        }
    }

    void AddSelectableItemRuntime(GameObject obj)
    {
        obj.AddComponent<SelectableItem>(); // Add SelectableItem component to the object
        foreach (Transform child in obj.transform)
            AddSelectableItemRuntime(child.gameObject);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, newLayer);
    }

    bool IsValidPlacement(GameObject obj)
    {
        Bounds bounds = GetCombinedBounds(obj);
        Collider[] hits = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, LayerMask.GetMask("PlacedItem"));

        foreach (var hit in hits)
        {
            if (!hit.transform.IsChildOf(obj.transform))
                return false;
        }

        return true;
    }

    void SetPreviewColor(Color color)
    {
        foreach (var renderer in previewRenderers)
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                    mat.color = color;
            }
        }
    }

    void SetPreviewDefault()
    {
        foreach (var renderer in previewRenderers)
        {
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = new Material(previewMaterial); // Instance-safe
            }
            renderer.materials = mats;
        }
    }

    Bounds GetCombinedBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.one * 0.5f);

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    void CreatePreview()
    {
        previewInstance = Instantiate(selectedItem.prefabVariants[index]);
        previewRenderers = previewInstance.GetComponentsInChildren<Renderer>();

        foreach (var renderer in previewRenderers)
        {
            Material[] mats = renderer.materials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = new Material(previewMaterial); // Instance-safe
            }
            renderer.materials = mats;
        }

        // Optional: disable colliders
       // foreach (var col in previewInstance.GetComponentsInChildren<Collider>())
       //     col.enabled = false;
    }

    void DestroyPreview()
    {
        if (previewInstance)
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    public void SetSelectedItem(ItemData item)
    {
        selectedItem = item;
        DestroyPreview(); // Replace previous preview with new item
    }

    public void SetSelectedItemVariant(ItemData item , int index)
    {
        selectedItem = item;
        this.index = index;
        DestroyPreview(); // Replace previous preview with new item
    }


    void TrySnapToNearbyFurniture()
    {
        if (previewInstance == null) return;

        Bounds previewBounds = GetCombinedBounds(previewInstance);
        Collider[] nearby = Physics.OverlapBox(previewBounds.center, previewBounds.extents + Vector3.one * 0.05f, Quaternion.identity, LayerMask.GetMask("PlacedItem"));

        foreach (Collider col in nearby)
        {
            if (col.transform.IsChildOf(previewInstance.transform)) continue;

            Bounds otherBounds = col.bounds;

            if (previewBounds.Intersects(otherBounds))
            {

                Vector3 correction = Vector3.zero;

                float xOverlap = Mathf.Min(previewBounds.max.x, otherBounds.max.x) - Mathf.Max(previewBounds.min.x, otherBounds.min.x);
                float yOverlap = Mathf.Min(previewBounds.max.y, otherBounds.max.y) - Mathf.Max(previewBounds.min.y, otherBounds.min.y);
                float zOverlap = Mathf.Min(previewBounds.max.z, otherBounds.max.z) - Mathf.Max(previewBounds.min.z, otherBounds.min.z);

                // Push in the axis with smallest overlap
                if (xOverlap < yOverlap && xOverlap < zOverlap)
                {
                    correction = new Vector3(
                        (previewBounds.center.x > otherBounds.center.x) ? xOverlap : -xOverlap,
                        0f, 0f
                    );
                }
                else if (zOverlap < yOverlap && zOverlap < xOverlap)
                {
                    correction = new Vector3(
                        0f, 0f,
                        (previewBounds.center.z > otherBounds.center.z) ? zOverlap : -zOverlap
                    );
                }
                else
                {
                    correction = new Vector3(0f,
                        (previewBounds.center.y > otherBounds.center.y) ? yOverlap : -yOverlap,
                        0f
                    );
                }

                previewInstance.transform.position += correction;

                // 🔄 Align rotation Y if needed
                if (enableRotationAlignment)
                {
                    float currentY = previewInstance.transform.eulerAngles.y;
                    float targetY = col.transform.eulerAngles.y;
                    float angleDiff = Mathf.Abs(Mathf.DeltaAngle(currentY, targetY));

                    if (angleDiff > rotationSnapThreshold)
                    {
                        Vector3 newRotation = previewInstance.transform.eulerAngles;
                        newRotation.y = targetY;
                        previewInstance.transform.eulerAngles = newRotation;

#if UNITY_EDITOR
                        Debug.DrawRay(previewBounds.center, previewInstance.transform.forward * 0.5f, Color.blue, 0.1f);
#endif
                    }
                }


                previewBounds = GetCombinedBounds(previewInstance); // update for further iterations


#if UNITY_EDITOR
                Debug.DrawRay(previewBounds.center, correction.normalized * 0.2f, Color.cyan, 0.1f);
#endif
            }

            // If not intersecting, try magnetic edge snapping  
            float snapGap = 0.001f; // tiny buffer to prevent overlap

            if (!previewBounds.Intersects(otherBounds))
            {
                Vector3 offset = Vector3.zero;

                if (Mathf.Abs(previewBounds.min.x - otherBounds.max.x) < 0.05f)
                    offset = new Vector3(otherBounds.max.x - previewBounds.min.x + snapGap, 0f, 0f);

                else if (Mathf.Abs(previewBounds.max.x - otherBounds.min.x) < 0.05f)
                    offset = new Vector3(otherBounds.min.x - previewBounds.max.x - snapGap, 0f, 0f);

                else if (Mathf.Abs(previewBounds.min.z - otherBounds.max.z) < 0.05f)
                    offset = new Vector3(0f, 0f, otherBounds.max.z - previewBounds.min.z + snapGap);

                else if (Mathf.Abs(previewBounds.max.z - otherBounds.min.z) < 0.05f)
                    offset = new Vector3(0f, 0f, otherBounds.min.z - previewBounds.max.z - snapGap);

                if (offset != Vector3.zero)
                {
                    previewInstance.transform.position += offset;
                    previewBounds = GetCombinedBounds(previewInstance); // update bounds
#if UNITY_EDITOR
                    Debug.DrawRay(previewBounds.center, offset.normalized * 0.2f, Color.magenta, 0.1f);
#endif
                    continue; // Skip overlap logic since snapping was successful
                }
            }

        }
    }


    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (previewInstance == null) return;

        // Draw preview bounding box
        Bounds previewBounds = GetCombinedBounds(previewInstance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(previewBounds.center, previewBounds.size);

        // Draw nearby overlap boxes
        Collider[] nearby = Physics.OverlapBox(previewBounds.center, previewBounds.extents + Vector3.one * 0.05f, Quaternion.identity, LayerMask.GetMask("PlacedItem"));

        foreach (Collider col in nearby)
        {
            if (col.transform.IsChildOf(previewInstance.transform)) continue;

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
#endif
    }

}
