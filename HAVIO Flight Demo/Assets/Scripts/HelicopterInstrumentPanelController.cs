using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 03. 02. 
   Description : 헬리콥터의 계기판 동작을 관여하는 스크립트
   Edit Log    : 
    - 각 멤버변수에 헤더를 달아서 밖에서 봤을 때 조금 더 깔끔하게 보
      이도록 하였음
    - 단위눈금당 각도를 직접 입력하여 수정할 수 있도록 함
   ================================================================= */

public class HelicopterInstrumentPanelController : MonoBehaviour
{
    HelicopterFlightController hfc;

    [Header("Attitude Gauge")]
    public GameObject objAttitude;
    [Header("Plane Compass")]
    public GameObject objCompass;
    [Header("Altitude 100m Directing Hand, [x ˚/ 100m]")]
    public GameObject objAltitude100Hand;
    public float fDegsPer100m;
    [Header("Altitude 10m Directing Hand, [y ˚/ 10m]")]
    public GameObject objAltitude10Hand;
    public float fDegsPer10m;
    [Header("Vertical Speed Gauge, [z ˚/ m/s]")]
    public GameObject objVerticalSpeedHand;
    public float fDegsPer1VerticalSpeed;
    [Header("Wind Speed(Plane Speed) Gauge, [w ˚/ 1m/s]")]
    public GameObject objWindSpeedHand;
    public float fDegsPer1WindSpeed;

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
        objAltitude100Hand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -fAltitude / 100.0f * fDegsPer100m);
        objAltitude10Hand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -fAltitude / 10.0f * fDegsPer10m);

        objVerticalSpeedHand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - fDegsPer1VerticalSpeed * hfc.rigidBody.velocity.y);

        objWindSpeedHand.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f - fDegsPer1WindSpeed * hfc.fVelocity);
	}
}
