using UnityEngine;

public class HeadBobbing : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.6f;  
    public Transform head;                 
    private CharacterController cc;

    [Header("Detection")]
    public float startThreshold = 0.10f;    
    public float stopThreshold = 0.04f;   
    public float smoothing = 10f;    
    public float minOnTime = 0.4f;      
    public float accel = 6f;             
    public float decel = 8f;          

    private bool active;
    private float onTimer;
    private float currentSpeed;
    private float smoothedVel;

    private Vector3 lastLocalPos;
    private bool hasLast;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (head == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) head = cam.transform;
        }
    }

    void Update()
    {
        if (head == null) return;

        Vector3 localVel = EstimateLocalVelocity();
        float v = Mathf.Max(0f, localVel.y); 

        smoothedVel = Mathf.Lerp(
            smoothedVel,
            v,
            1f - Mathf.Exp(-smoothing * Time.deltaTime)
        );

        if (!active)
        {
            if (smoothedVel >= startThreshold)
            {
                active = true;
                onTimer = minOnTime;
            }
        }
        else
        {
            onTimer -= Time.deltaTime;
            if (onTimer <= 0f && smoothedVel <= stopThreshold)
                active = false;
        }

        float targetSpeed = active ? moveSpeed : 0f;
        float rate = active ? accel : decel;
        currentSpeed = Mathf.MoveTowards(
            currentSpeed,
            targetSpeed,
            rate * Time.deltaTime
        );

        if (currentSpeed > 0.001f)
            MoveForward(currentSpeed * Time.deltaTime);
    }


    Vector3 EstimateLocalVelocity()
    {
        Vector3 p = head.localPosition;

        if (!hasLast)
        {
            hasLast = true;
            lastLocalPos = p;
            return Vector3.zero;
        }

        Vector3 v = (p - lastLocalPos) / Mathf.Max(Time.deltaTime, 0.0001f);
        lastLocalPos = p;
        return v;
    }

    void MoveForward(float dist)
    {
        Vector3 forward = head.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 delta = forward * dist;

        if (cc != null)
            cc.Move(delta);
        else
            transform.position += delta;
    }
}
