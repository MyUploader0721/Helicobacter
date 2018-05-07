using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RaceController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-05-07
 * DESCRIPTION: Race 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - 정해진 지점을 통과하며 시간 내에 목표까지 도착해야 하는 레이싱 모드입니다. 
 *  - 이 모드를 컨트롤하는 스크립트입니다.  
 */

public class RaceController : MonoBehaviour
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
    [SerializeField] GameObject objTargetNav;
    public bool bAccomplished = false;
    [SerializeField] RacePassageBehaviour[] rpbPassages;
    [SerializeField] GameObject objGoal;
    public int nPassedPassages = 0;
    public int nTimeRemained = 60;
    public int nAmountOfTimeAddition = 5;
    public bool bCanGoal = false;
    public bool bHasGoalIn = false;

    void Start()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseInnerPod = bUseInnerPod;
        helicopterInfo.bUseOuterPod = bUseOuterPod;

        for (int i = 0; i < rpbPassages.Length; i++)
        {
            rpbPassages[i].SetNumber(i);
        }

        StartCoroutine("StartTimer");
    }

    void Update()
    {
        if (bIsStartInMidAir)
        {
            bIsStartInMidAir = false;
            objPlayer.GetComponent<InputController>().ToggleEngine();
        }

        if (nPassedPassages < rpbPassages.Length)
        {
            Vector3 v3PosNav = rpbPassages[nPassedPassages].transform.position;
            v3PosNav.y += 30.0f;
            objTargetNav.transform.position = v3PosNav;
        }
        else
        {
            Vector3 v3PosNav = objGoal.transform.position;
            v3PosNav.y += 30.0f;
            objTargetNav.transform.position = v3PosNav;
        }
    }

    public void EnterPassage(int nNumber)
    {
        rpbPassages[nNumber] = null;

        nPassedPassages++;
        nTimeRemained += nAmountOfTimeAddition;

        if (nNumber == rpbPassages.Length - 1)
        {
            bCanGoal = true;
        }
            
    }

    IEnumerator StartTimer()
    {
        while (nTimeRemained > 0)
        {
            nTimeRemained--;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
