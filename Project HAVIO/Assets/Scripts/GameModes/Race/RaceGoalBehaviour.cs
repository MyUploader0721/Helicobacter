using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RaceGoalBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-05-07
 * DESCRIPTION: 마지막 통과지점의 행동을 정의하는 스크립트
 *     DEV LOG: 
 *  - 플레이어는 모든 중간 통과지점을 통과한 후 이 곳에 와야합니다. 
 */

public class RaceGoalBehaviour : MonoBehaviour
{
    [Header("Race Controller")]
    [SerializeField] RaceController raceController;

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 마지막 통과지점에 도달하였을 경우
        if (other.CompareTag("Player") && raceController.bCanGoal)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
