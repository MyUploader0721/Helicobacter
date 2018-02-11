using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterFlightController : MonoBehaviour
{
    [Header("Helicopter Rotor Controls")]
    public GameObject objMainRotor;
    HelicopterRotorController hrcMainRotor;
    public GameObject objTailRotor;
    HelicopterRotorController hrcTailRotor;

    [Header("Helicopter Values")]
    public float fCollectiveVelocity = 1.0f;
    public float fCycleVelocity = 1.0f;

    Rigidbody rigidBody = null;

    Vector3 v3UpForce = Vector3.zero;
    bool bEngineStatus = false;

    float fThrottle = 0.0f;
    float fCollective = 0.0f;
    float fAntiTorque = 0.0f;
    Vector3 v3CycleDir = Vector3.zero;

    const float PASSIVE_INPUT_STEP = 0.03125f;
    const float VELOCITY_LERP_STEP = 0.0625f;
    
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        v3UpForce = -Physics.gravity * rigidBody.mass;

        hrcMainRotor = objMainRotor.GetComponent<HelicopterRotorController>();
        hrcTailRotor = objTailRotor.GetComponent<HelicopterRotorController>();
    }
	
	void FixedUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ToggleEngine();

        if (bEngineStatus == true)
        {
            ControlThrottle();
            ControlCollective();
            ControlAntiTorque();
            ControlCycle();
        }

        hrcMainRotor.GetThrottlePercent(fThrottle);
        hrcTailRotor.GetThrottlePercent(fAntiTorque);

        rigidBody.AddRelativeForce((v3UpForce + v3CycleDir) * fThrottle);
    }

    void ToggleEngine()
    {
        bEngineStatus = !bEngineStatus;
        Debug.Log("Engine Status: " + bEngineStatus);

        if (bEngineStatus == true)
        {
            hrcMainRotor.StartSpinning();
            hrcTailRotor.StartSpinning();
        }
        else
        {
            hrcMainRotor.EndSpinning();
            hrcTailRotor.EndSpinning();

            while (fThrottle > 0.0f)
                fThrottle -= PASSIVE_INPUT_STEP;
        }
    }

    void ControlThrottle()
    {
        // Space for Throttle Up
        if (Input.GetKey(KeyCode.Space))
        {
            if (fThrottle < 1.0f)
            {
                fThrottle += PASSIVE_INPUT_STEP;
                Debug.Log("Throttle: " + (fThrottle * 100.0f).ToString("0.00") + "%");
            }
        }

        // LCtrl for Throttle Down
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (fThrottle > 0.0f)
            {
                fThrottle -= PASSIVE_INPUT_STEP;
                Debug.Log("Throttle: " + (fThrottle * 100.0f).ToString("0.00") + "%");
            }
        }
    }

    void ControlCollective()
    {
        fCollective = Input.GetAxis("Vertical") * fCollectiveVelocity;

        rigidBody.velocity = Vector3.Lerp(
            rigidBody.velocity, 
            rigidBody.transform.up * fCollective, 
            VELOCITY_LERP_STEP
        );
    }

    void ControlAntiTorque()
    {
        fAntiTorque = Input.GetAxis("Horizontal");

        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            rigidBody.transform.up * fAntiTorque, 
            VELOCITY_LERP_STEP
        );
    }

    void ControlCycle()
    {
        // U for Pitch Down
        if (Input.GetKey(KeyCode.U))
        {
            if (v3CycleDir.z < 1.0f)
                v3CycleDir.z += PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.z > 0.0f)
                v3CycleDir.z -= PASSIVE_INPUT_STEP;
        }

        // J for Pitch Up
        if (Input.GetKey(KeyCode.J))
        {
            if (v3CycleDir.z > -1.0f)
                v3CycleDir.z -= PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.z < 0.0f)
                v3CycleDir.z += PASSIVE_INPUT_STEP;
        }

        // K for Roll Right
        if (Input.GetKey(KeyCode.K))
        {
            if (v3CycleDir.x < 1.0f)
                v3CycleDir.x += PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.x > 0.0f)
                v3CycleDir.x -= PASSIVE_INPUT_STEP;
        }

        // H for Roll Left
        if (Input.GetKey(KeyCode.H))
        {
            if (v3CycleDir.x > -1.0f)
                v3CycleDir.x -= PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.x < 0.0f)
                v3CycleDir.x += PASSIVE_INPUT_STEP;
        }

        v3CycleDir *= fCycleVelocity;

        // Pitch
        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            rigidBody.transform.right * v3CycleDir.z, 
            VELOCITY_LERP_STEP
        );
        // Roll
        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            -rigidBody.transform.forward * v3CycleDir.x, 
            VELOCITY_LERP_STEP
        );
    }

    public Vector3 GetCycleDirection() { return v3CycleDir; }
    public float  GetCollectiveValue() { return fCollective; }
}
