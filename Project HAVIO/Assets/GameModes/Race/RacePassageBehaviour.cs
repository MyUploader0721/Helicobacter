﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RacePassageBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-05-07
 * DESCRIPTION: 중간 통과지점의 행동을 정의하는 스크립트
 *     DEV LOG: 
 *  - 플레이어는 이 중간 통과지점을 순서대로 통과해야 합니다. 
 */

public class RacePassageBehaviour : MonoBehaviour
{
    [Header("Race Controller")]
    [SerializeField] RaceController raceController;
    [Header("Number of this Passage")]
    [SerializeField] int nNumber = -1;

    void OnTriggerEnter(Collider other)
    {
        // 플레이어가 중간 통과지점에 도달하였을 경우
        if (other.CompareTag("Player") && raceController.nPassedPassages == nNumber)
        {
            raceController.EnterPassage(nNumber);

            Destroy(transform.gameObject);
        }
    }

    /// <summary>
    /// 해당 중간 통과지점의 번호를 설정합니다. (초기화)
    /// </summary>
    /// <param name="n"></param>
    public void SetNumber(int n)
    {
        if (nNumber == -1)
        {
            nNumber = n;
        }
    }
}