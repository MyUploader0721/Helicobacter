using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 3. 28.
   Description : 헬리콥터의 전체적인 충돌을 관리하는 스크립트
   Edit Log    : 
    - 자식 오브젝트의 콜라이더로부터 충돌 정보를 가져와서 헬리콥터에
      적용하는 스크립트 입니다. 
   ================================================================= */

public class HelicopterCollisionController : MonoBehaviour
{
    HelicopterFlightController hfc;
    HelicopterInfoController hic;
    HelicopterSFXController hsc;

    float fCurrentVelocity = 0.0f;

    public float fMaxSafeVelocity = 3.5f;

    void Start ()
    {
        hfc = GetComponent<HelicopterFlightController>();
        hic = GetComponent<HelicopterInfoController>();
        hsc = GetComponent<HelicopterSFXController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            fCurrentVelocity = hfc.fVelocity;

            if (fCurrentVelocity > fMaxSafeVelocity)
            {
                float temp = fCurrentVelocity - fMaxSafeVelocity;
                hic.Damage(temp);
            }

            hsc.PlayCrashSound(fCurrentVelocity, fMaxSafeVelocity);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Terrain"))
        {
            hic.fDurability = 0.0f;

            hsc.PlayCrashSound(fCurrentVelocity, fMaxSafeVelocity);
        }
    }
}
