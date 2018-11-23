using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAVController : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] bool bUsingGamePad;
    [Space]
    [SerializeField] float fBasicThrust;
    [SerializeField] float fThrusterSpeed;
    [SerializeField] float fRotationSpeed;
    [Space]
    [Header("Rotor")]
    [Space]
    [Header("SFX")]
    
    AudioSource audioSource;
    Rigidbody rigidBody;

    bool bEngineStatus = false;

    float fThrust = 0.0f;
    float fThrustInput = 0.0f;
    Vector3 v3Direction;

	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update ()
    {
        if (bUsingGamePad)
        {
            fThrustInput = Input.GetAxis("DPadY");
            v3Direction.x = Input.GetAxis("RightJoystickX");
            v3Direction.y = Input.GetAxis("RightJoystickY");
        }
        else
        {
            fThrustInput = Input.GetAxis("Vertical");
            v3Direction.x = Input.GetAxis("KeyboardCycleHorizontal");
            v3Direction.y = Input.GetAxis("KeyboardCycleVertical");
        }

        Debug.Log(v3Direction);

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetButtonDown("FaceButtonA"))
        {
            bEngineStatus = !bEngineStatus;
        }

        if (bEngineStatus)
        {
            if (rigidBody.useGravity)
                rigidBody.useGravity = false;

            if (fThrust < fBasicThrust)
                fThrust = Mathf.Lerp(fThrust, fBasicThrust, Time.deltaTime);

            rigidBody.velocity = transform.forward * (fThrust + fThrustInput * fThrusterSpeed);
            transform.localEulerAngles += new Vector3(v3Direction.y * fRotationSpeed, 0.0f, -v3Direction.x * fRotationSpeed);
        }
        else
        {
            if (!rigidBody.useGravity)
                rigidBody.useGravity = true;

            fThrust = Mathf.Lerp(fThrust, 0.0f, Time.deltaTime);
        }
    }
}
