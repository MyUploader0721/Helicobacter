﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnoMotion;

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

    public enum Platform { Debug, Release }
    public Platform platform = Platform.Debug;

    Rigidbody rigidBody = null;

    Vector3 v3UpForce = Vector3.zero;
    public bool bEngineStatus = false;
    bool bUpCollective = false;

    float fThrottle = 0.0f;
    float fCollective = 0.0f;
    float fAntiTorque = 0.0f;
    Vector3 v3CycleDir = Vector3.zero;

    const float PASSIVE_INPUT_STEP = 0.03125f;
    const float VELOCITY_LERP_STEP = 0.0625f;

	MotionInput motionInput;
    HelicopterSFXController sfxController;
    HelicopterMotionController motionController;

	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
        v3UpForce = -Physics.gravity * rigidBody.mass;

        hrcMainRotor = objMainRotor.GetComponent<HelicopterRotorController>();
        hrcTailRotor = objTailRotor.GetComponent<HelicopterRotorController>();

		motionInput = GetComponent<MotionInput> ();
		motionInput.StartInput ();

        sfxController = GetComponent<HelicopterSFXController>();
        motionController = GetComponent<HelicopterMotionController>();
    }
	
	void FixedUpdate ()
    {
        if (platform == Platform.Debug)
            if (Input.GetKeyDown(KeyCode.LeftShift))
                ToggleEngine();
        else if (platform == Platform.Release)
            if (Input.GetButtonDown("joystick button 3"))
                ToggleEngine();

        if (bEngineStatus == true)
        {
            ControlCollective();
            ControlAntiTorque();
            ControlCycle();

            sfxController.PlayNormal();
            if (fCollective > 0.0f)
                sfxController.PlayKeepCollective();
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

            sfxController.PlayStartEngine();
            motionController.PlayStartEngine();

            StartCoroutine(SlightlyThrottleUp());
        }
        else
        {
            hrcMainRotor.EndSpinning();
            hrcTailRotor.EndSpinning();

            sfxController.PlayKillEngine();

            StartCoroutine(SlightlyThrottleDown());
        }
    }

    IEnumerator SlightlyThrottleUp()
    {
        while (fThrottle < 1.0f)
        {
            fThrottle += PASSIVE_INPUT_STEP;
            yield return new WaitForSeconds(0.1f);
        }

        fThrottle = 1.0f;
    }
    IEnumerator SlightlyThrottleDown()
    {
        while (fThrottle > 0.0f)
        {
            fThrottle -= PASSIVE_INPUT_STEP;
            yield return new WaitForSeconds(0.1f);
        }

        fThrottle = 0.0f;
    }

    void ControlCollective()
    {
        if (platform == Platform.Debug)
        {
            fCollective = Input.GetAxis("Vertical") * fCollectiveVelocity;

            if (Input.GetKeyDown(KeyCode.W))
            {
                sfxController.PlayUpCollective();
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                sfxController.PlayDownCollective();
            }
        }
        else if (platform == Platform.Release)
        {
            fCollective = Input.GetAxis("CollectiveVertical") * fCollectiveVelocity;

            if (Input.GetButtonDown("CollectiveVertical") && fCollective > 0.0f)
            {
                sfxController.PlayUpCollective();
            }
            else if (Input.GetKeyUp("CollectiveVertical") && fCollective > 0.0f)
            {
                sfxController.PlayDownCollective();
            }
        }

        rigidBody.velocity = Vector3.Lerp(
            rigidBody.velocity,
            rigidBody.transform.up * fCollective,
            VELOCITY_LERP_STEP
        );
    }

    void ControlAntiTorque()
    {
        if (platform == Platform.Debug)
            fAntiTorque = Input.GetAxis("Horizontal");
        else if (platform == Platform.Release)
            fAntiTorque = Input.GetAxis("AntiTorqueHorizontal");

        rigidBody.angularVelocity = Vector3.Lerp(
            rigidBody.angularVelocity, 
            rigidBody.transform.up * fAntiTorque, 
            VELOCITY_LERP_STEP
        );
    }

    void ControlCycle()
    {
        if (platform == Platform.Debug)
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
        }
        else if (platform == Platform.Release)
        {
            v3CycleDir.x = Input.GetAxis("CycleHorizontal");
            v3CycleDir.z = -Input.GetAxis("CycleVertical");
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
