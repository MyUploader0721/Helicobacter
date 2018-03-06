using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterControlsBehaviour : MonoBehaviour
{
    HelicopterFlightController hfc;

    [Header("Helicopter Control Sticks")]
    public GameObject []objCycles;
    public GameObject objCollective;

    [Header("Stick Control Values")]
    public float fCycleRotationAngle = 10.0f;
    public float fCollectiveRotationAngle = 10.0f;

    Vector3 v3CycleDir = Vector3.zero;
    float fCollective = 0.0f;

	void Start ()
    {
        hfc = GetComponent<HelicopterFlightController>();
	}
	
	void FixedUpdate ()
    {
        v3CycleDir = hfc.GetCycleDirection() * fCycleRotationAngle;
        fCollective = hfc.GetCollectiveValue() * fCollectiveRotationAngle;

        SetCycleStickRotation();
        SetCollectiveStickRotation();
    }

    void SetCycleStickRotation()
    {
        foreach(GameObject obj in objCycles)
        {
            obj.transform.localRotation = Quaternion.Euler(-v3CycleDir.z, 0.0f, v3CycleDir.x);
        }
    }

    void SetCollectiveStickRotation()
    {
        objCollective.transform.localRotation = Quaternion.AngleAxis(fCollective, Vector3.right);
    }
}
