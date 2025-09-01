using UnityEngine;

public class IconGenerator : MonoBehaviour
{
    public Camera previewCamera;
    public LayerMask previewLayer;
    private GameObject lastPreviewInstance;

    public Sprite GenerateIcon(GameObject prefab)
    {
        // Cleanup old preview object
        if (lastPreviewInstance != null)
        {
            DestroyImmediate(lastPreviewInstance);
            lastPreviewInstance = null;
        }

        previewCamera.gameObject.SetActive(true);

        // Instantiate new preview object
        lastPreviewInstance = Instantiate(prefab);
        SetLayerRecursively(lastPreviewInstance, LayerMask.NameToLayer("Preview"));

        // Calculate bounds for framing
        Bounds bounds = GetCombinedBounds(lastPreviewInstance);
        Vector3 center = bounds.center;
        float size = Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z) * 2f;

        // Camera setup
        previewCamera.transform.position = center + new Vector3(size, size, size);
        previewCamera.transform.LookAt(center);
        previewCamera.orthographic = true;
        previewCamera.orthographicSize = size * 0.75f;
        previewCamera.clearFlags = CameraClearFlags.SolidColor;
        previewCamera.backgroundColor = new Color(0, 0, 0, 0);
        previewCamera.cullingMask = 1 << LayerMask.NameToLayer("Preview");

        /*// Optional light
        GameObject lightGO = new GameObject("PreviewLight");
        Light light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1f;
        light.transform.rotation = Quaternion.Euler(45, 45, 0);
        lightGO.layer = LayerMask.NameToLayer("Preview");*/

        // Create a new RenderTexture each time to avoid ghosting
        RenderTexture tempRT = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
        tempRT.Create();

        RenderTexture.active = tempRT;
        GL.Clear(true, true, new Color(0, 0, 0, 0)); // fully clear before render
        previewCamera.targetTexture = tempRT;

        previewCamera.Render();

        // Read the result into Texture2D
        Texture2D tex = new Texture2D(tempRT.width, tempRT.height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        tex.Apply();

        // Cleanup
        RenderTexture.active = null;
        previewCamera.targetTexture = null;
        tempRT.Release();
        DestroyImmediate(tempRT);
        //DestroyImmediate(lightGO);
        DestroyImmediate(lastPreviewInstance);
        previewCamera.gameObject.SetActive(false);

        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private Bounds GetCombinedBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(obj.transform.position, Vector3.one);

        Bounds bounds = renderers[0].bounds;
        foreach (var r in renderers)
            bounds.Encapsulate(r.bounds);
        return bounds;
    }
}
