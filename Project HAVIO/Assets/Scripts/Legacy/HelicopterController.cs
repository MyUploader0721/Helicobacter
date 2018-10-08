using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterController : MonoBehaviour
{
    [Header("Helicopter Setting")]
    [SerializeField] float fEngineStartDuration = 2.0f;
    [SerializeField] float fEngineEndDuration = 1.0f;
    [Space]
    [SerializeField] float fCollectiveSpeed = 3.5f;
    [SerializeField] float fAntiTorqueSpeed = 1.0f;
    [SerializeField] float fCycleSpeed = 5.0f;

    [Header("Rotor Setting")]
    [SerializeField] GameObject objMainRotorAxis;
    [SerializeField] float fMainRotorSpeed = 30.0f;
    [SerializeField] GameObject objTailRotorAxis;
    [SerializeField] float fTailRotorSpeed = 30.0f;

    [Header("Player Setting")]
    [SerializeField] bool bPlayWithJoystick = false;
    public bool PlayWithJoystick { get { return bPlayWithJoystick; } set { bPlayWithJoystick = value; } }

    Rigidbody rigidBody;

    float fThrottle = 0.0f;
    float fCollective = 0.0f;
    float fAntiTorque = 0.0f;
    Vector3 v3Cycle = Vector3.zero;

    bool bEngineStatus = false;
    bool bEngineWorking = false;
    bool bIsOnTheGround = false;
    public bool IsOnTheGround { get { return bIsOnTheGround; } set { bIsOnTheGround = value; } }
    bool bEngineAvailable = true;
    public bool IsEngineAvailable { get { return bEngineAvailable; } set { bEngineAvailable = value; } }

    readonly float fGravity = Physics.gravity.magnitude;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Helicopter has been crashed
        if (!bEngineAvailable)
        {
            if (bEngineStatus)
                StartCoroutine(EngineEnd());
            bEngineStatus = false;
        }

        if (bPlayWithJoystick)
        {
            
        }
        else
        {
            // Start & Kill Engine
            if (Input.GetKeyDown(KeyCode.LeftShift) && !bEngineWorking && bEngineAvailable)
            {
                if (bEngineStatus)
                    StartCoroutine(EngineEnd());
                else
                    StartCoroutine(EngineStart());

                bEngineStatus = !bEngineStatus;
            }

            fCollective = Input.GetAxis("Vertical");
            fAntiTorque = Input.GetAxis("Horizontal");

            v3Cycle.x = Input.GetAxis("KeyboardCycleHorizontal");
            v3Cycle.z = Input.GetAxis("KeyboardCycleVertical");
        }

    }

    void FixedUpdate()
    {
        // Flight
        if (bEngineStatus && !bEngineWorking)
        {
            float fStallParameter = 1.0f - Mathf.Abs(Vector3.Dot(Vector3.up, rigidBody.transform.up));
            //float fStallAngle = Mathf.Acos(Vector3.Dot(Vector3.up, rigidBody.transform.up));

            //float fVelXOrZ = Mathf.Clamp(Mathf.Sin(3.0f * fStallAngle), 0.0f, 1.0f);
            //if (float.IsNaN(fVelXOrZ)) fVelXOrZ = 0.0f;
            //float fVelY = Mathf.Clamp(Mathf.Cos(3.0f * fStallAngle), 0.0f, 1.0f);
            //if (float.IsNaN(fVelY)) fVelY = 0.0f;
            //Vector2 v2Tilted = new Vector2(rigidBody.transform.up.x, rigidBody.transform.up.z).normalized;

            // Throttle
            rigidBody.AddForce(rigidBody.transform.up * rigidBody.mass * fGravity * fThrottle);

            // Collective
            rigidBody.velocity =
                Vector3.Lerp(rigidBody.velocity, rigidBody.velocity * fStallParameter + (rigidBody.transform.up * fCollectiveSpeed) * fCollective, 1.0f / 16.0f);

            //Vector3 v3Stall = rigidBody.velocity;
            //rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, v3Stall + new Vector3(v2Tilted.x * fVelXOrZ, fVelY, v2Tilted.y * fVelXOrZ) * fCollectiveSpeed * fCollective, 1.0f / 16.0f);

            // Anti-Torque
            rigidBody.angularVelocity =
                Vector3.Lerp(rigidBody.angularVelocity, (rigidBody.transform.up * fAntiTorqueSpeed) * fAntiTorque, 1.0f / 16.0f);

            // Cycle
            rigidBody.rotation *=
                Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(v3Cycle.z * fCycleSpeed, 0.0f, -v3Cycle.x * fCycleSpeed), 1.0f / 16.0f);
        }

        // Main Rotor Rotation
        objMainRotorAxis.transform.Rotate(0.0f, fThrottle * fMainRotorSpeed, 0.0f);

        // Tail Rotor Rotation
        objTailRotorAxis.transform.Rotate(0.0f, fThrottle * fTailRotorSpeed, 0.0f);
    }

    /// <summary>
    /// Starts the engine
    /// </summary>
    IEnumerator EngineStart()
    {
        bEngineWorking = true;

        while (fThrottle < 1.0f)
        {
            fThrottle += Time.deltaTime / fEngineStartDuration;
            yield return new WaitForEndOfFrame();
        }
        fThrottle = 1.0f;

        bEngineWorking = false;
    }

    /// <summary>
    /// Kills the engine
    /// </summary>
    IEnumerator EngineEnd()
    {
        bEngineWorking = true;

        while (fThrottle > 0.0f)
        {
            fThrottle -= Time.deltaTime / fEngineEndDuration;
            yield return new WaitForEndOfFrame();
        }
        fThrottle = 0.0f;

        bEngineWorking = false;
    }
}
