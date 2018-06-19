using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: AH6InstrumentPanelController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-11
 * DESCRIPTION: 헬리콥터의 계기판을 관리하는 스크립트
 *     DEV LOG: 
 *  - 헬리콥터마다 계기판이 다르므로 이를 관리하는 스크립트가 제각각이어야 할 것 같았습니다. 
 *  - 본 스크립트는 AH-6의 계기판용으로만 사용할 수 있습니다. 
 *  - 헬리콥터의 현재 정보를 읽어와서 조종사가 확인할 수 있도록 표시하는 역할을 합니다. 
 */

public class AH6InstrumentPanelController : MonoBehaviour
{
    [Header("Flight Info")]
    FlightController flightController;

    [Header("Altitude Gauge")]
    [SerializeField] GameObject objAltShortHand;
    [SerializeField] float fAltShortHandAmount = 100.0f;
    [SerializeField] GameObject objAltLongHand;
    [SerializeField] float fAltLongHandAmount = 1.0f;

    [Header("Attitude")]
    [SerializeField] GameObject objAttBall;
    [SerializeField] float fAdjustXRot = 0.0f;

    [Header("Compass")]
    [SerializeField] GameObject objCompassDir;

    [Header("Speed Gauge")]
    [SerializeField] GameObject objWindSpeedHand;
    [SerializeField] GameObject objVerticalSpeedHand;

    void Start ()
    {
        flightController = GetComponentInParent<FlightController>();
	}
	
	void Update ()
    {
        Rigidbody rigidbody = flightController.GetComponent<Rigidbody>();

        float fAltitude = rigidbody.position.y;
        float fVelocity = rigidbody.velocity.magnitude;
        float fVerticalVelocity = rigidbody.velocity.y;
        float fCompassDir = rigidbody.rotation.eulerAngles.y;

        // Altitude
        objAltShortHand.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAltitude / fAltShortHandAmount * 36.0f);
        objAltLongHand.transform.localEulerAngles = new Vector3(0.0f, 0.0f, -fAltitude / fAltLongHandAmount * 36.0f);

        // Attitude
        objAttBall.transform.eulerAngles = new Vector3(fAdjustXRot, 0.0f, 0.0f);

        // Compass
        objCompassDir.transform.localEulerAngles = new Vector3(0.0f, 0.0f, fCompassDir);

        // Speed
        objWindSpeedHand.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f - fVelocity * 51.4285714f);
        objVerticalSpeedHand.transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f - fVerticalVelocity * 40.0f);
    }
}
