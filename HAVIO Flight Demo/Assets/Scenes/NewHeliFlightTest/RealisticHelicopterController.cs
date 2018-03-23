using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealisticHelicopterController : MonoBehaviour
{
    [Header("Helicopter Control Variables")]
    [Range(0.0f, 1.0f)]
    public float fThrottle = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float fCyclePitch = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float fCycleRoll = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float fCollective = 0.0f;

    new Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
	}

    void FixedUpdate()
    {
        fCollective = Input.GetAxis("Vertical");

        rigidbody.AddForce(rigidbody.transform.up * Physics.gravity.magnitude * fThrottle);
        rigidbody.AddForce(rigidbody.transform.up * fCollective * 10);

        rigidbody.angularVelocity = new Vector3(fCyclePitch, 0.0f, -fCycleRoll);

        float fUpReverseCosine = 1.0f - Vector3.Dot(Vector3.up, rigidbody.transform.up);
        Debug.Log(fUpReverseCosine);

        rigidbody.velocity = Vector3.Lerp(
            rigidbody.velocity,
            rigidbody.velocity * (fUpReverseCosine > 1.0f ? 1.0f : fUpReverseCosine),
            0.125f
        );
    }
}
