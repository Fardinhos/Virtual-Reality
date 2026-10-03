using UnityEngine;

public class Walking : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2.0f;

    [Header("Direction Source")]
    public Transform directionSource;

    [Header("Input")]
    public OVRInput.Button moveButton = OVRInput.Button.PrimaryIndexTrigger;

    void Reset()
    {
        var cam = GetComponentInChildren<Camera>();
        if (cam != null) directionSource = cam.transform;
    }

    void Update()
    {
        if (directionSource == null) return;

        if (OVRInput.Get(moveButton))
        {
            Vector3 forward = directionSource.forward;
            forward.y = 0f;
            forward.Normalize();

            transform.position += forward * speed * Time.deltaTime;
        }
    }
}
