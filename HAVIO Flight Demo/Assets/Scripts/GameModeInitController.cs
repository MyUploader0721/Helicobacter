using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 3. 28.
   Description : 게임모드의 초기화를 담당하는 스크립트
   Edit Log    : 
    - 게임모드와 헬리콥터의 초기 정보를 설정하는 스크립트입니다. 
    - 사실 이 스크립트는 테스트용으로, 앞으로 제작될 많은 게임모드 씬
      에 이와 같은 형식으로 적용될 것입니다. 
   ================================================================= */

public class GameModeInitController : MonoBehaviour
{
    public GameObject objPlayerHelicopter;
    public float fMaxDurability;

    public bool bActiveGunPod = false;
    public bool bActiveRocketPod = false;
    public bool bActiveSearchLight = false;

    public bool bPlayWithJoystick = false;

    HelicopterInfoController hic;

    void Start ()
    {
        hic = objPlayerHelicopter.GetComponent<HelicopterInfoController>();

        hic.fMaxDurability = fMaxDurability;
        hic.fDurability = hic.fMaxDurability;

        hic.bActivateGunPod = bActiveGunPod;
        hic.bActivateRocketPod = bActiveRocketPod;
        hic.bActiveSearchLight = bActiveSearchLight;

        hic.bPlayWithJoystick = bPlayWithJoystick;

        hic.SetHelicopterStatus();
	}
	
	void FixedUpdate ()
    {
		
	}
}
