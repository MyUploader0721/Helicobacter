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
 *  - 04-30: 타겟의 AI를 조금 변형합니다. 일정 범위 밖에서는 랜덤으로 이동하다가
 *           플레이어가 일정 범위 안으로 이동하면 플레이어와 반대 방향으로 이동하게 됩니다. 
 *  - 05-02: 플레이어는 공중에 있으므로 타겟 바로 위에 위치하면 플레이어가 아주 조금씩 움직이거나
 *           움직이질 못하는 것 같습니다. destination의 y를 0으로 설정하여 해결했습니다. 
 */

public class MACTargetBehaviour : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    public int nTargetPos = 0;

    [SerializeField] MidAirChaserController macController;
    [SerializeField] GameObject objPlayer;

    [SerializeField] float fEscapeDistance = 15.0f;

    public float fDistance = 0.0f;
    public bool bIsEscapingMode = false;
    
	void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        nTargetPos = Random.Range(0, macController.objRandPos.Length);
    }
	
	void Update ()
    {
        fDistance = Vector3.Distance(transform.position, objPlayer.transform.position);

        if (fDistance >= fEscapeDistance)
        {
            bIsEscapingMode = false;
            navMeshAgent.destination = macController.objRandPos[nTargetPos].transform.position;
        }
        else
        {
            bIsEscapingMode = true;
            Vector3 delta = transform.position - objPlayer.transform.position;
            // EDITLOG: 05-02, 타겟의 종착지의 y값을 0으로 설정하여 지상에서 움직이도록 설정
            delta.y = 0.0f;
            navMeshAgent.destination = transform.position + delta * delta.magnitude;
        }
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
