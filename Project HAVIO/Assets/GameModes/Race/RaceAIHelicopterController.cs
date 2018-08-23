using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RaceAIHelicopterController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-07-26
 * DESCRIPTION: Race 모드에서 경쟁 헬리콥터의 AI를 관여하는 컨트롤러
 *     DEV LOG: 
 *  
 */

public class RaceAIHelicopterController : MonoBehaviour
{
    [Header("Helicopter Speed")]
    [SerializeField] float fHelicopterForwardSpeed = 5.0f;
    [SerializeField] float fHelicopterAscendSpeed = 2.5f;
    [SerializeField] float fHelicopterDescendSpeed = -3.0f;
    [SerializeField] float fHelicopterRollingAngle = 30.0f;
    [SerializeField] float fHelicopterPitchingAngle = 30.0f;

    [Header("Helicopter Rotor Blade Options")]
    [SerializeField] GameObject objMainRotorBlade;
    [SerializeField] GameObject objTailRotorBlade;
    [SerializeField] float fMainRotorSpeed = 10.0f;
    [SerializeField] float fTailRotorSpeed = 10.0f;


    [Header("Waypoint Tracker")]
    [SerializeField] Transform tfWaypointTracker;

    Rigidbody rigidBody;
    float fVerticalSpeed = 0.0f;
    float fRollingAngle = 0.0f;
    float fPitchingAngle = 0.0f;

	void Start ()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 헬리콥터의 상승과 하강, Pitching
        Vector3 v3Velocity = transform.forward * fHelicopterForwardSpeed;
        if (tfWaypointTracker.position.y > transform.position.y + 2.0f)
        {
            fVerticalSpeed = Mathf.Lerp(fVerticalSpeed, fHelicopterAscendSpeed, Time.deltaTime);
            fPitchingAngle = Mathf.Lerp(fPitchingAngle, 0.0f, Time.deltaTime);
        }
        else if (tfWaypointTracker.position.y < transform.position.y - 2.0f)
        {
            fVerticalSpeed = Mathf.Lerp(fVerticalSpeed, fHelicopterDescendSpeed, Time.deltaTime);
            fPitchingAngle = Mathf.Lerp(fPitchingAngle, fHelicopterPitchingAngle, Time.deltaTime);
        }
        else
        {
            fVerticalSpeed = Mathf.Lerp(fVerticalSpeed, 0.0f, Time.deltaTime);
            fPitchingAngle = Mathf.Lerp(fPitchingAngle, 0.0f, Time.deltaTime);
        }
        v3Velocity.y = fVerticalSpeed;

        // 헬리콥터의 기울기
        Vector3 v3Heli = transform.forward;
        v3Heli.y = 0.0f;
        Vector3 v3Targey = tfWaypointTracker.forward;
        v3Targey.y = 0.0f;

        float fRoll = Vector3.Angle(v3Heli, v3Targey) * (Vector3.Cross(v3Heli, v3Targey).y > 0.0f ? -1.0f : 1.0f);

        // 헬리콥터의 Rolling
        if (fRoll > 5.0f)
            fRollingAngle = Mathf.Lerp(fRollingAngle, fHelicopterRollingAngle, Time.deltaTime);
        else if (fRoll < -5.0f)
            fRollingAngle = Mathf.Lerp(fRollingAngle, -fHelicopterRollingAngle, Time.deltaTime);
        else
            fRollingAngle = Mathf.Lerp(fRollingAngle, 0.0f, Time.deltaTime);

        rigidBody.velocity = v3Velocity;

        transform.eulerAngles = new Vector3(fPitchingAngle, Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(tfWaypointTracker.position - transform.position), Time.deltaTime).eulerAngles.y, fRollingAngle);

        // 메인 로터 회전익 회전
        objMainRotorBlade.transform.localEulerAngles += new Vector3(0.0f, fMainRotorSpeed, 0.0f);

        // 테일 로터 회전익 회전
        objTailRotorBlade.transform.localRotation *= Quaternion.Euler(fTailRotorSpeed, 0.0f, 0.0f);
    }
}
