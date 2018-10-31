using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAVIOCarController : MonoBehaviour
{
    [Header("Car Setting")]
    [SerializeField] Transform trsCar;
    [SerializeField] Transform trsCamera;
    [Space]
    [SerializeField] Transform trsSteeringWheel;
    [SerializeField] Transform[] trsFrontWheel;
    [SerializeField] Transform[] trsRearWheel;
    [SerializeField] float fRotation; // [-PI/2, PI/2]
    [Space]
    [SerializeField] float fMoveSpeed;
    [SerializeField] float fForwardSpeed;
    [SerializeField] float fBackwardSpeed;
    [Space]
    [SerializeField]

    Rigidbody rigidBody;
    bool bMovedForward = true;

    Vector3 v3MoveDirection;
    Vector3 v3FrontWheelRotation;

    void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
	
	void Update ()
    {
        CarInput();
    }

    void CarInput()
    {
        float fInput = Input.GetAxis("Vertical");

        v3MoveDirection = trsCar.forward;

        // acceleration
        if (fInput > 0)
        {
            fMoveSpeed = fForwardSpeed * fInput;
            if (!bMovedForward) bMovedForward = true;
        }
        else if (fInput < 0)
        {
            fMoveSpeed = fBackwardSpeed * fInput;
            if (bMovedForward) bMovedForward = false;
        }
        else
        {
            fMoveSpeed = fInput;
        }

        // steering
        fRotation = Input.GetAxis("Horizontal") * Mathf.PI / 2.0f;

        trsSteeringWheel.localRotation = Quaternion.Euler(0.0f, fRotation * Mathf.Rad2Deg - 90.0f, 0.0f);

        v3FrontWheelRotation += new Vector3(rigidBody.velocity.magnitude * (bMovedForward ? -1.0f : 1.0f), 0.0f, 0.0f);
        v3FrontWheelRotation.y = (fRotation * Mathf.Rad2Deg) / 2.0f + 90.0f;

        trsFrontWheel[0].localEulerAngles = v3FrontWheelRotation;
        trsFrontWheel[1].localEulerAngles = v3FrontWheelRotation;
        trsRearWheel[0].localRotation *= Quaternion.Euler(rigidBody.velocity.magnitude * (bMovedForward ? 1.0f : -1.0f), 0.0f, 0.0f);
        trsRearWheel[1].localRotation *= Quaternion.Euler(rigidBody.velocity.magnitude * (bMovedForward ? 1.0f : -1.0f), 0.0f, 0.0f);

        transform.rotation *= Quaternion.Euler(0.0f, (fRotation * Mathf.Rad2Deg) / 2.0f * Time.deltaTime * fInput, 0.0f);

        // moving
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, v3MoveDirection * fMoveSpeed, Time.deltaTime / 2.0f);
    }
}
