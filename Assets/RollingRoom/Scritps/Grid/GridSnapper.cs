using UnityEngine;

public static class GridSnapper
{
    public static Vector3 Snap(Vector3 rawPos, float gridSize = 0.01f)
    {
        return new Vector3(
            Mathf.Round(rawPos.x / gridSize) * gridSize,
            Mathf.Round(rawPos.y / gridSize) * gridSize,
            Mathf.Round(rawPos.z / gridSize) * gridSize
        );
    }
}
