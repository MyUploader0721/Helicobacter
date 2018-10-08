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
 *           또, 메쉬 안에서 바깥 방향으로 추격할 때, 타겟이 경계에 있으면 다음 path를 찾을 수
 *           없어서 멈춰버리는 현상이 발생합니다. 이를 해결하기 위해 다음 path를 계산하지 못하면
 *           그냥 랜덤으로 이동하도록 하였습니다. 
 *  - 05-21: 여전히 문제가 발생하여 Brackeys의 예제를 참고하였습니다..
 *           ref: https://github.com/MyUploader0721/Helicobacter/blob/feature/myuploader0721/Weekly%20Report%20SandBox/Assets/NavMesh/CS_RunAway.cs
 *  - 05-25: 임무를 성공할 경우 타겟이 제자리에 멈추도록 하였습니다. 
 */

public class MACTargetBehaviour : MonoBehaviour
{
    NavMeshAgent navMeshAgent;

    public int nTargetPos = 0;

    [SerializeField] MidAirChaserController macController;
    [SerializeField] GameObject objPlayer;

    [SerializeField] float fEscapeDistance = 15.0f;

    public float fDistance = 999.0f;
    public bool bIsEscapingMode = false;
    
	void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        nTargetPos = Random.Range(0, macController.objRandPos.Length);
    }
	
	void Update ()
    {
        fDistance = Vector3.Distance(transform.position, objPlayer.transform.position);

        
        // DevLog: 05-25
        if (macController.bAccomplished)
        {
            navMeshAgent.SetDestination(transform.position);
        }
        else
        {
            if (fDistance >= fEscapeDistance)
            {
                bIsEscapingMode = false;
                navMeshAgent.destination = macController.objRandPos[nTargetPos].transform.position;
            }
            else
            {
                // DevLog: 05-21, 추적
                Vector3 mDirToPlayer = transform.position - objPlayer.transform.position;
                Vector3 mNewPos = transform.position + mDirToPlayer;
                navMeshAgent.SetDestination(mNewPos);
            }
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
