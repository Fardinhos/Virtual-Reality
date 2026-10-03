using UnityEngine;

public class ControllerShake : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.8f;
    public Transform directionSource;
    private CharacterController cc;

    [Header("Shake Detection")]
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;
    public float shakeThreshold = 0.8f;
    public float activityHoldTime = 0.20f;
    public float intensitySmoothing = 12f;

    private float activityTimer = 0f;
    private float smoothedIntensity = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (directionSource == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                directionSource = cam.transform;
        }
    }

    void Update()
    {
        if (directionSource == null)
            return;

        float intensity = OVRInput.GetLocalControllerVelocity(controller).magnitude;

        smoothedIntensity = Mathf.Lerp(
            smoothedIntensity,
            intensity,
            1f - Mathf.Exp(-intensitySmoothing * Time.deltaTime)
        );

        if (smoothedIntensity > shakeThreshold)
            activityTimer = activityHoldTime;
        else
            activityTimer = Mathf.Max(0f, activityTimer - Time.deltaTime);

        if (activityTimer > 0f)
            MoveForward(moveSpeed * Time.deltaTime);
    }

    void MoveForward(float distance)
    {
        Vector3 forward = directionSource.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 delta = forward * distance;

        if (cc != null)
            cc.Move(delta);
        else
            transform.position += delta;
    }
}
