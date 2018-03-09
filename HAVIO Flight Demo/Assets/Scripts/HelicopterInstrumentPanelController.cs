using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterInstrumentPanelController : MonoBehaviour
{
    HelicopterFlightController hfc;

    public GameObject objAttitude;

    public GameObject objCompass;

    public GameObject objAltitude100Hand;
    public GameObject objAltitude10Hand;

    public GameObject objVerticalSpeedHand;

    public GameObject objWindSpeedHand;

    float fAltitude = 0.0f;

	void Start ()
    {
        hfc = GetComponent<HelicopterFlightController>();
	}
	
	void FixedUpdate ()
    {
        objAttitude.transform.forward = Vector3.forward;

        objCompass.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, transform.rotation.eulerAngles.y);

        fAltitude = transform.position.y;
        while (fAltitude > 100.0f) fAltitude -= 100.0f;
        objAltitude100Hand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -fAltitude / 100.0f * 36.0f);
        objAltitude10Hand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -fAltitude / 10.0f * 36.0f);

        objVerticalSpeedHand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - 40.0f * hfc.rigidBody.velocity.y);

        objWindSpeedHand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - 50.0f * hfc.fVelocity);
	}
}
