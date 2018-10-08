using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: CarePackageStartingPointBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-13
 * DESCRIPTION: CPD 게임모드의 끝을 장식하는 시작 위치 행동 정의 스크립트
 *     DEV LOG: 
 *  - 주어진 개수의 배달을 완료하면 헬리콥터는 시작 위치에 착륙을 합니다. 
 */

public class CarePackageStartingPointBehaviour : MonoBehaviour
{
    [Header("Basic Setting")]
    [SerializeField] HelicopterInfo helicopterInfo;
    [SerializeField] CarePackageDeliveryController cpdController;
    [SerializeField] float fLandingTime = 2.5f;

    bool bisLanding = false;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && !bisLanding && cpdController.bAccomplished && helicopterInfo.bIsFlyable && !helicopterInfo.bIsEngineStart)
        {
            StartCoroutine(AccomplishLanding());
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && cpdController.bAccomplished || !helicopterInfo.bIsFlyable)
        {
            StopAllCoroutines();
            bisLanding = false;
        }
    }

    /// <summary>
    /// 임무 완수 후 베이스에 안전한 착륙을 하였을 때 비로소 임무가 종료됩니다. 
    /// </summary>
    /// <returns>코루틴 함수입니다. <code>fLandingTime</code>초 동안 대기합니다. </returns>
    IEnumerator AccomplishLanding()
    {
        bisLanding = true;

        yield return new WaitForSeconds(fLandingTime);

		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
    }
}
