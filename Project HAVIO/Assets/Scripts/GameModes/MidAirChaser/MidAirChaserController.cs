﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 *       TITLE: MidAirChaserController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-18
 * DESCRIPTION: Mid-Air Chaser 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - 공중에서 목표를 추격하는 임무를 수행하는 게임모드를 설정합니다. 
 *  - 정해진 매개변수들을 조절하여 난이도를 변경할 수 있습니다. 
 *  - 05-09: 임무 성공 시 게임 종료를,
 *           게임 실패 시 재시작을 할 수 있도록 하였습니다. 
 */

public class MidAirChaserController : MonoBehaviour
{
    HelicopterInfo helicopterInfo;

    [Header("Player Helicopter")]
    [SerializeField] GameObject objPlayer;

    [Header("Helicopter Setting")]
    [SerializeField] bool bIsPlayWithGamePad = false;
    [SerializeField] bool bIsStartInMidAir = false;

    [Header("Helicopter Armament Setting(Recommends default)")]
    [SerializeField] bool bUseSearchLight = true;
    [SerializeField] bool bUseInnerPod = false;
    [SerializeField] bool bUseOuterPod = false;

    [Header("Game Mode Setting")]
    [SerializeField] GameObject objTarget;
    MACTargetBehaviour targetBehaviour;
    public GameObject[] objRandPos;
    public bool bAccomplished = false;
    public int nMissionTime = 300;
    public int nRemainedTime = 0;
    public float fMaxDistance = 20.0f;
    public int nMaxMissingAlert = 50;
    public int nRemainedMissingAlert = 0;
    bool bMissingAlert = false;

    [Header("Game Over Setting")]
    [SerializeField] GameObject objAccomplishedPanel;
    [SerializeField] GameObject objGameOverPanel;
    bool bIsGameOver = false;

    void Start ()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseInnerPod = bUseInnerPod;
        helicopterInfo.bUseOuterPod = bUseOuterPod;

        targetBehaviour = objTarget.GetComponent<MACTargetBehaviour>();

        nRemainedTime = nMissionTime;
        nRemainedMissingAlert = nMaxMissingAlert;

        StartCoroutine("MissionTimer");
    }
	
	void Update ()
    {
        if (bIsStartInMidAir)
        {
            bIsStartInMidAir = false;
            objPlayer.GetComponent<InputController>().ToggleEngine();
        }

        if (targetBehaviour.fDistance > fMaxDistance)
        {
            if (!bMissingAlert)
                StartCoroutine("DecreaseMissingAlert");

            Debug.Log("Missing your target!: " + targetBehaviour.fDistance + "m");
        }
        else
        {
            if (bMissingAlert)
            {
                StopCoroutine("DecreaseMissingAlert");
                bMissingAlert = false;
            }

            Debug.Log("Distance is Safe now: " + targetBehaviour.fDistance + "m");
        }

        if (!helicopterInfo.bIsFlyable)
        {
            StopCoroutine("MissionTimer");
            GameOver();
        }

        // DEVLOG 05-09:
        if (bAccomplished)
        {
            // 종료
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
        if (bIsGameOver)
        {
            // 재시작
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            // 종료
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
	}

    /// <summary>
    /// 타이머를 코루틴으로 구현합니다. 
    /// 플레이어는 해당 시간동안 Target을 좇아야 합니다. 
    /// </summary>
    IEnumerator MissionTimer()
    {
        while (nRemainedTime > 0)
        {
            nRemainedTime--;
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Mission Accomplished!");
        bAccomplished = true;
        objAccomplishedPanel.SetActive(true);
    }

    /// <summary>
    /// 플레이어와 Target 사이의 거리가 멀어질 때 MA값을 감소시킵니다. 
    /// </summary>
    IEnumerator DecreaseMissingAlert()
    {
        bMissingAlert = true;

        while (nRemainedMissingAlert > 0)
        {
            nRemainedMissingAlert--;
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Mission Failed!");
        bAccomplished = false;

        GameOver();
    }

    /// <summary>
    /// 미션을 실패하여 게임 오버가 되었을 경우 게임 오버를 화면에 출력하고 다시 시작 또는 종료를 유도합니다. 
    /// </summary>
    void GameOver()
    {
        if (!bIsGameOver)
        {
            objGameOverPanel.SetActive(true);

            bIsGameOver = true;

        }
    }
}
