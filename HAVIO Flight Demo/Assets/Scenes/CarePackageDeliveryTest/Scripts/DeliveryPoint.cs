using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 4.
   Description : 배달지점으로써 마땅히 해야 할 일을 적은 스크립트
   Edit Log    : 
    - 헬리콥터에서 떨어진 케어패키지가 잘 있으면 bDelivered가 true
      로 변하게 됩니다. 
    - CPDTC에서는 true의 개수를 세서 전부 잘 배달되었는지를 검사합
      니다. 
   ================================================================= */

public class DeliveryPoint : MonoBehaviour
{
    bool bDelivered = false;
    CarePackageDeliveryTestController cpdtc;

    void Start()
    {
        cpdtc = GameObject.FindGameObjectWithTag("GameController").GetComponent<CarePackageDeliveryTestController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Package"))
        {
            if (!bDelivered)
            {
                bDelivered = true;
                cpdtc.nDeliveredPoint++;
            }
        }
    }
}
