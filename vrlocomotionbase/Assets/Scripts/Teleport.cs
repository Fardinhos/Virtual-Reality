using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Teleport")]
    public Transform rayOrigin;
    public float maxDistance = 20f;
    public LayerMask groundMask = ~0;

    [Header("Input")]
    public OVRInput.Button teleportButton = OVRInput.Button.One;
    public OVRInput.Button aimButton = OVRInput.Button.PrimaryHandTrigger;

    [Header("Optional Visual")]
    public LineRenderer line;

    void Reset()
    {
        var cam = GetComponentInChildren<Camera>();
        if (cam != null) rayOrigin = cam.transform;
    }

    void Update()
    {
        if (rayOrigin == null) return;

        bool aiming = OVRInput.Get(aimButton) || OVRInput.Get(teleportButton);
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, maxDistance, groundMask, QueryTriggerInteraction.Ignore);

        if (line != null)
        {
            line.enabled = aiming;
            if (aiming)
            {
                line.positionCount = 2;
                line.SetPosition(0, ray.origin);
                line.SetPosition(1, hitSomething ? hit.point : (ray.origin + ray.direction * maxDistance));
            }
        }

        if (OVRInput.GetDown(teleportButton) && hitSomething)
        {
            Vector3 target = hit.point;
            target.y = transform.position.y;

            transform.position = target;
        }
    }
}
