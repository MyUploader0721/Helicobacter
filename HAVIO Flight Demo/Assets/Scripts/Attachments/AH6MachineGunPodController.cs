using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 3.
   Description : AH-6의 기관총 포드를 관리하는 스크립트
   Edit Log    : 
    - 기관총 빵야빵야, 사실 기관포입니다. 아직 작동하지는 않습니다. 
   ================================================================= */

public class AH6MachineGunPodController : MonoBehaviour
{
    public new Camera camera;

    public GameObject[] objMachineGuns;

    HelicopterFlightController hfc;

    void Start ()
    {
        hfc = GameObject.FindGameObjectWithTag("Player").GetComponent<HelicopterFlightController>();
    }
	
	void Update ()
    {
        if (hfc.bPlayWithJoystick)
        {
            // 게임패드 키 할당
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                Debug.Log("MachineGunPod Activated");
            }
        }
    }
}
