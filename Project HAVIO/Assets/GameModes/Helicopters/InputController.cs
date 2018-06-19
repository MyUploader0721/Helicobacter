using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: InputController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-06
 * DESCRIPTION: 헬리콥터의 입력을 담당하는 스크립트
 *     DEV LOG: 
 *  - 예전에 작성하던 스크립트가 완전히 재사용 불가능한 수준이었습니다. 
 *    (특정 헬리콥터에 종속적임) 따라서 재사용 가능한 스크립트 작성을 목표로 
 *    재작성하도록 하였습니다. 
 *  - 각각의 비행 매개변수들은 다른 스크립트에서 접근 가능하도록 하여 입력과
 *    입력에 따른 동작을 따로 분리하였습니다. 
 */

public class InputController : MonoBehaviour
{
    [HideInInspector] public float fCollective = 0.0f;
    [HideInInspector] public float fAntiTorque = 0.0f;
    [HideInInspector] public float fCycleRoll  = 0.0f;
    [HideInInspector] public float fCyclePitch = 0.0f;
    [HideInInspector] public float fThrottle   = 0.0f;

    [HideInInspector] public bool bActivateInnerPod;
    [HideInInspector] public bool bActivateOuterPod;

    HelicopterInfo helicopterInfo;

    bool bIsPlayingEngineCoroutine = false;

    const float fThrottleIncreaseInterval = 32.0f;
    const float fThrottleIncreaseStep = 1.0f / fThrottleIncreaseInterval;
    const float fThrottleDecreaseInterval = 16.0f;
    const float fThrottleDecreaseStep = 1.0f / fThrottleDecreaseInterval;

    void Start ()
    {
        helicopterInfo = GetComponent<HelicopterInfo>();
	}
	
	void Update ()
    {
		if (helicopterInfo.bIsPlayWithGamePad)
        {
            if (Input.GetButtonDown("EngineToggle") && helicopterInfo.bIsFlyable) ToggleEngine();

            // Control Flight
            fCollective = Input.GetAxis("CollectiveVertical");
            fAntiTorque = Input.GetAxis("AntiTorqueHorizontal");
            fCycleRoll  = Input.GetAxis("CycleHorizontal");
            fCyclePitch = Input.GetAxis("CycleVertical");

            bActivateOuterPod = Input.GetButton("Secondary Trigger");
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && helicopterInfo.bIsFlyable) ToggleEngine();

            // Control Flight
            fCollective = Input.GetAxis("Vertical");
            fAntiTorque = Input.GetAxis("Horizontal");
            fCycleRoll  = Input.GetAxis("KeyboardCycleHorizontal");
            fCyclePitch = Input.GetAxis("KeyboardCycleVertical");

            bActivateInnerPod = Input.GetKey(KeyCode.Space);
            bActivateOuterPod = Input.GetKey(KeyCode.LeftControl);
        }
	}

    /// <summary>
    /// 엔진의 상태를 Toggle합니다. 
    /// </summary>
    public void ToggleEngine()
    {
        helicopterInfo.bIsEngineStart = !helicopterInfo.bIsEngineStart;

        if (helicopterInfo.bIsEngineStart)
        {
            if (bIsPlayingEngineCoroutine) StopCoroutine(KillEngine());
            if (!bIsPlayingEngineCoroutine) StartCoroutine(StartEngine());
        }
        else
        {
            if (bIsPlayingEngineCoroutine) StopCoroutine(StartEngine());
            if (!bIsPlayingEngineCoroutine) StartCoroutine(KillEngine());
        }
    }
    IEnumerator StartEngine()
    {
        bIsPlayingEngineCoroutine = true;

        while (fThrottle < 1.0f)
        {
            fThrottle += fThrottleIncreaseStep;
            yield return new WaitForSeconds(fThrottleIncreaseStep);
        }

        bIsPlayingEngineCoroutine = false;
    }
    IEnumerator KillEngine()
    {
        bIsPlayingEngineCoroutine = true;

        while (fThrottle > 0.0f)
        {
            fThrottle -= fThrottleDecreaseStep;
            yield return new WaitForSeconds(fThrottleDecreaseStep);
        }

        bIsPlayingEngineCoroutine = false;
    }
}
