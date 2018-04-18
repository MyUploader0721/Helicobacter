using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: MidAirChaserController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-18
 * DESCRIPTION: Mid-Air Chaser 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - 공중에서 목표를 추격하는 임무를 수행하는 게임모드를 설정합니다. 
 *  - 정해진 매개변수들을 조절하여 난이도를 변경할 수 있습니다. 
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
    [SerializeField] float fMaxDistance = 20.0f;
    public int nMaxMissingAlert = 50;
    public int nRemainedMissingAlert = 0;
    bool bMissingAlert = false;

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
            StopCoroutine("MissionTimer");
	}

    IEnumerator MissionTimer()
    {
        while (nRemainedTime > 0)
        {
            nRemainedTime--;
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Mission Accomplished!");
        bAccomplished = true;

        UnityEditor.EditorApplication.isPlaying = false;
    }

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

        UnityEditor.EditorApplication.isPlaying = false;
    }
}
