using UnityEngine;

public class WallScaler : MonoBehaviour
{
    public float scaleSpeed = 5f;
    public float holdTime = 0.2f; // stay hidden this long after obstruction ends

    private float baseHeight;
    private float targetFactor = 1f;
    private float obstructedUntil = 0f;

    void Awake()
    {
        baseHeight = transform.localScale.y;
    }

    void Update()
    {
        // Decide target based on timer
        if (Time.time < obstructedUntil)
            targetFactor = 0f;
        else
            targetFactor = 1f;

        Vector3 s = transform.localScale;
        float targetY = baseHeight * targetFactor;

        s.y = Mathf.MoveTowards(s.y, targetY, Time.deltaTime * scaleSpeed * baseHeight);
        transform.localScale = s;
    }

    public void SetObstructed(bool obstructed)
    {
        if (obstructed)
        {
            // extend hidden time
            obstructedUntil = Time.time + holdTime;
        }
    }

    public void SetUserHeight(float newHeight)
    {
        baseHeight = newHeight;
        Vector3 s = transform.localScale;
        s.y = newHeight;
        transform.localScale = s;
    }
}
