using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterFlightController : MonoBehaviour
{
    Rigidbody rigidBody;

    Vector3 v3UpForce;
    bool bEngineStatus = false;
    float fEngineCoefficient = 0.0f;

    float fCollective;
    float fAntiTorque;
    Vector3 v3CycleDir = Vector3.zero;

    const float PASSIVE_INPUT_STEP = 0.03125f;
    
	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        v3UpForce = -Physics.gravity * rigidBody.mass;
	}
	
	void FixedUpdate ()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            ToggleEngine();

        if (bEngineStatus == false)
        {

        }
        else
        {
            ControlCollective();
            ControlAntiTorque();
            ControlCycle();
        }

        rigidBody.AddRelativeForce((v3UpForce + v3CycleDir) * fEngineCoefficient);
    }

    void ToggleEngine()
    {
        if (bEngineStatus == false)
        {
            while (fEngineCoefficient < 1.0f)
            {
                fEngineCoefficient += 0.0015625f;
            }
        }
        else
        {
            while (fEngineCoefficient > 0.0f)
            {
                fEngineCoefficient -= 0.0015625f;
            }
        }

        bEngineStatus = !bEngineStatus;
    }

    void ControlCollective()
    {
        fCollective = Input.GetAxis("Vertical");

        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, rigidBody.transform.up * fCollective, 0.0625f);
    }

    void ControlAntiTorque()
    {
        fAntiTorque = Input.GetAxis("Horizontal");

        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, rigidBody.transform.up * fAntiTorque, 0.0625f);
    }

    void ControlCycle()
    {
        // U for Pitch Down
        if (Input.GetKey(KeyCode.U))
        {
            if (v3CycleDir.z < 1.0f) v3CycleDir.z += PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.z > 0.0f) v3CycleDir.z -= PASSIVE_INPUT_STEP;
        }

        // J for Pitch Up
        if (Input.GetKey(KeyCode.J))
        {
            if (v3CycleDir.z > -1.0f) v3CycleDir.z -= PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.z < 0.0f) v3CycleDir.z += PASSIVE_INPUT_STEP;
        }

        // K for Roll Right
        if (Input.GetKey(KeyCode.K))
        {
            if (v3CycleDir.x < 1.0f) v3CycleDir.x += PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.x > 0.0f) v3CycleDir.x -= PASSIVE_INPUT_STEP;
        }

        // H for Roll Left
        if (Input.GetKey(KeyCode.H))
        {
            if (v3CycleDir.x > -1.0f) v3CycleDir.x -= PASSIVE_INPUT_STEP;
        }
        else
        {
            if (v3CycleDir.x < 0.0f) v3CycleDir.x += PASSIVE_INPUT_STEP;
        }

        // Pitch
        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, rigidBody.transform.right * v3CycleDir.z, 0.0625f);

        rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, -rigidBody.transform.forward * v3CycleDir.x, 0.0625f);
    }
}
