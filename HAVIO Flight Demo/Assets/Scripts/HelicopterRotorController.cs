using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 01. 16. 
   Description : 헬리콥터 날개의 회전을 담당하는 스크립트
   Edit Log    : 
    - 사용법: 
     * StartSpinning()과 EndSpinning()는 HelicopterFlightController
       의 ToggleEngine()에서 사용됩니다. 이외에서는 사용하지 않습니다. 
     * GetThrottlePercent(float)은 HelicopterFlightController의 
       FixedUpdate()에서 사용됩니다. 이외에서는 사용하지 않습니다. 
      
   ================================================================= */

public class HelicopterRotorController : MonoBehaviour
{
    float fAngularSpeed = 0.0f;
    float fThrottleSpeed = 0.0f;

    public Vector3 v3RotationTo = Vector3.zero;

    public float fMaxAngularSpeed = 2.0f;
    public float fAccelarationInterval = 0.1f;
    public float fThrottleMultiplier = 10.0f;

    void FixedUpdate()
    {
        transform.Rotate(v3RotationTo, fAngularSpeed + (fThrottleSpeed * fThrottleMultiplier));
    }

    /// <summary>
    /// 엔진 시동을 걸 때 로터가 회전하도록 합니다. 
    /// HelicopterFlightController의 ToggleEngine()에서 사용됩니다. 
    /// </summary>
    public void StartSpinning()
    {
        StopAllCoroutines();
        StartCoroutine(StartsSpinningCoroutine());
    }
    /// <summary>
    /// 엔진 시동을 걸 때 로터가 서서히 회전하도록 코루틴을 구성합니다. 
    /// StartSpinning()에서 사용됩니다. 
    /// </summary>
    IEnumerator StartsSpinningCoroutine()
    {
        while (fAngularSpeed < fMaxAngularSpeed)
        {
            fAngularSpeed += 0.125f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }

    /// <summary>
    /// 엔진 시동을 끌 때 로터가 정지하도록 합니다. 
    /// HelicopterFlightController의 ToggleEngine()에서 사용됩니다. 
    /// </summary>
    public void EndSpinning()
    {
        StopAllCoroutines();
        StartCoroutine(EndsSpinningCoroutine());
    }
    /// <summary>
    /// 엔진 시동을 끌 때 로터가 서서히 정지하도록 코루틴을 구성합니다. 
    /// EndSpinning()에서 사용됩니다. 
    /// </summary>
    IEnumerator EndsSpinningCoroutine()
    {
        while (fAngularSpeed > 0.0f)
        {
            fAngularSpeed -= 0.125f;
            if (fAngularSpeed < 0.0f)
                fAngularSpeed = 0.0f;
            yield return new WaitForSeconds(fAccelarationInterval);
        }
    }

    /// <summary>
    /// 헬리콥터의 현재 스로틀을 참조합니다. 
    /// HelicopterFlightController의 FixedUpdate()에서 사용됩니다. 
    /// </summary>
    /// <param name="fThrottle"></param>
    public void GetThrottlePercent(float fThrottle)
    {
        fThrottleSpeed = fThrottle;
    }
}
