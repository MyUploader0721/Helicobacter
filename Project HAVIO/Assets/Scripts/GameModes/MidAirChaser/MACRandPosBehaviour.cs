using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: MACRandPosBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-18
 * DESCRIPTION: 쫒기는 타겟 오브젝트가 이동할 위치가 해야 할 일을 정의한 스크립트
 *     DEV LOG: 
 *  - 타겟 오브젝트는 이 랜덤 포지션으로 이동하게 됩니다. 
 *  - 랜덤 포지션의 콜라이더와 충돌할 경우 새로운 랜덤 포지션을 갖게 되지요
 */

public class MACRandPosBehaviour : MonoBehaviour
{
    [SerializeField] int nNum;

	void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && other.GetComponent<MACTargetBehaviour>().nTargetPos == nNum)
        {
            other.GetComponent<MACTargetBehaviour>().ChangeDestination();
        }
    }
}
