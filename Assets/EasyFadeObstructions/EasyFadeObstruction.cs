using UnityEngine;
using System.Collections.Generic;

public class EasyFadeObstruction : MonoBehaviour
{
    public Transform target;
    public LayerMask fadeMask;
    public float fadeRadius = 0.3f;
    public float fadeOffset = 0f;
    public float fadeOutSpeed = 6f;
    public float fadeInSpeed = 6f;
    [Range(0f, 1f)]
    public float obstructAlpha = 0.2f;

    HashSet<Renderer> obstructed = new HashSet<Renderer>();
    HashSet<Renderer> stillObstructed = new HashSet<Renderer>();

    void LateUpdate()
    {
        if (!target) return;

        // 1. Cast to target
        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude + fadeOffset;

        RaycastHit[] hits = Physics.SphereCastAll(
            new Ray(transform.position, dir),
            fadeRadius, dist, fadeMask,
            QueryTriggerInteraction.Ignore);

        stillObstructed.Clear();
        foreach (var h in hits)
        {
            var r = h.collider.GetComponent<Renderer>();
            if (r == null) continue;

            stillObstructed.Add(r);

            if (!obstructed.Contains(r))
                obstructed.Add(r);
        }

        // 2. Fade obstructed
        foreach (var r in obstructed)
        {
            if (!r) continue;
            float targetA = stillObstructed.Contains(r) ? obstructAlpha : 1f;
            FadeTo(r, targetA, stillObstructed.Contains(r) ? fadeOutSpeed : fadeInSpeed);
        }

        // 3. Remove cleaned-up renderers
        obstructed.RemoveWhere(r => !r);
    }

    void FadeTo(Renderer r, float targetAlpha, float speed)
    {
        foreach (var m in r.materials) // important: per-instance material
        {
            Color c = m.color;
            c.a = Mathf.MoveTowards(c.a, targetAlpha, speed * Time.deltaTime);
            m.color = c;
        }
    }
}
