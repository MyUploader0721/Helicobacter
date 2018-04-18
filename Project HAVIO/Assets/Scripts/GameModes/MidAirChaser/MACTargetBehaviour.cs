using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/**
 *       TITLE: MACTargetBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-18
 * DESCRIPTION: Mid-Air Chaser 게임모드에서 쫒기는 오브젝트의 행위를 정의한 스크립트
 *     DEV LOG: 
 *  - 플레이어 헬리콥터에 쫒기는 오브젝트의 행위를 정의합니다. 
 *  - NavMesh를 사용하여 주어진 위치로 이동합니다. 
 */

public class MACTargetBehaviour : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    public int nTargetPos = 0;

    [SerializeField] MidAirChaserController macController;
    [SerializeField] GameObject objPlayer;

    public float fDistance = 0.0f;
    
	void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        nTargetPos = Random.Range(0, macController.objRandPos.Length);
    }
	
	void Update ()
    {
        navMeshAgent.destination = macController.objRandPos[nTargetPos].transform.position;

        fDistance = Vector3.Distance(transform.position, objPlayer.transform.position);
	}

    /// <summary>
    /// 랜덤 포지션에 닿았을 경우 랜덤 포지션 오브젝트에서 이것을 호출하게 됩니다. 
    /// </summary>
    public void ChangeDestination()
    {
        int temp = nTargetPos;

        while (temp == nTargetPos)
            temp = Random.Range(0, macController.objRandPos.Length);
        nTargetPos = temp;
    }
}
