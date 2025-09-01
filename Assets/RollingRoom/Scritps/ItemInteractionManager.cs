using UnityEngine;

public class ItemInteractionManager : MonoBehaviour
{

    public LayerMask placementLayer;

    public Material previewMaterial;

    private Material[][] originalMaterials;
    private Renderer[] renderers;
    private bool lastValidPlacement = true;


    void Update()
    {
        var selected = SelectableItem.Current;
        if (selected == null) return;

        if (SelectableItem.Current == null)
        {
            RestoreOriginalMaterials(); // In case user deleted or deselected
            return;
        }

        if (selected == null)
        {
            RestoreOriginalMaterials(); // Safety
            return;
        }

       

        if (Input.GetKeyDown(KeyCode.R))
        {
            selected.transform.Rotate(Vector3.up * 45f); // rotate 45° Y
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            RestoreOriginalMaterials(); // Cleanup preview state
            Destroy(selected.gameObject);
        }

        // In Update()
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit) || hit.collider.GetComponent<SelectableItem>() == null)
            {
                if (SelectableItem.Current != null)
                    SelectableItem.Current.Deselect();
            }
        }


       /* // Move existing object (if being moved)
        if (selected.IsBeingMoved)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayer))
            {
                Vector3 snapPos = GridSnapper.Snap(hit.point);
                selected.transform.position = snapPos;

                if (renderers == null)
                {
                    renderers = selected.GetComponentsInChildren<Renderer>();
                    originalMaterials = new Material[renderers.Length][];

                    for (int i = 0; i < renderers.Length; i++)
                    {
                        originalMaterials[i] = renderers[i].materials;

                        Material[] newMats = new Material[originalMaterials[i].Length];
                        for (int j = 0; j < newMats.Length; j++)
                            newMats[j] = new Material(previewMaterial); // Clone per slot

                        renderers[i].materials = newMats;
                    }
                }

                bool isValid = IsValidPlacement(selected.gameObject);

                if (isValid != lastValidPlacement)
                {
                    lastValidPlacement = isValid;
                    SetPreviewColor(isValid ? Color.green : Color.red);
                }

                if (isValid && Input.GetMouseButtonDown(0))
                {
                    selected.ConfirmPlacement();
                    RestoreOriginalMaterials();
                }
            }
        }*/

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

    Bounds GetCombinedBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(obj.transform.position, Vector3.one * 0.5f);

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    void SetPreviewColor(Color color)
    {
        if (renderers == null) return;

        foreach (var renderer in renderers)
        {
            if (renderer == null) continue;

            var mats = renderer.materials;
            foreach (var mat in mats)
            {
                if (mat != null && mat.HasProperty("_Color"))
                    mat.color = color;
            }
        }
    }

    void RestoreOriginalMaterials()
    {
        if (renderers == null || originalMaterials == null) return;

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null || originalMaterials[i] == null)
                continue;

            try
            {
                renderers[i].materials = originalMaterials[i];
            }
            catch
            {
                // In case materials[] is invalid, silently skip
            }
        }

        renderers = null;
        originalMaterials = null;
    }
}
