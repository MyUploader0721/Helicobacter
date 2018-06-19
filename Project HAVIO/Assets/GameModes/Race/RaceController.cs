using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/**
 *       TITLE: RaceController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-05-07
 * DESCRIPTION: Race 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - 정해진 지점을 통과하며 시간 내에 목표까지 도착해야 하는 레이싱 모드입니다. 
 *  - 이 모드를 컨트롤하는 스크립트입니다.  
 *  - 05-17: 임무성공/실패에 해당하는 UI를 출력하도록 합니다. 
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
    public bool bGameOver = false;
    bool bTimerTicking = false;
    int nNumPassages;

    [Header("UI Setting")]
    [SerializeField] GameObject panelGameOver;
    public GameObject panelAccomplished;
    [SerializeField] Text textTime;
    [SerializeField] Text textGoalLeft;

    void Start()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseInnerPod = bUseInnerPod;
        helicopterInfo.bUseOuterPod = bUseOuterPod;
        helicopterInfo.bUseSearchLight = bUseSearchLight;

        for (int i = 0; i < rpbPassages.Length; i++)
        {
            rpbPassages[i].SetNumber(i);
        }

        nNumPassages = rpbPassages.Length;

        StartCoroutine("StartTimer");
    }

    void Update()
    {
        textGoalLeft.text = "Goal Left: " + (nNumPassages - nPassedPassages) + "/" + nNumPassages;

        // 공중에서 시작할 경우
        if (bIsStartInMidAir)
        {
            bIsStartInMidAir = false;
            objPlayer.GetComponent<InputController>().ToggleEngine();
        }

        // 다음 목표 표시(Target Navigation)
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

        if (!objPlayer.GetComponent<HelicopterInfo>().bIsFlyable && !bAccomplished)
        {
            bGameOver = true;
            if (!panelGameOver.activeInHierarchy)
                panelGameOver.SetActive(true);
        }

        if (bGameOver)
        {
            if (bTimerTicking)
                StopCoroutine("StartTimer");

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonB"))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            else if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("JoystickButtonA"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        if (bAccomplished)
        {
            if (bTimerTicking)
                StopCoroutine("StartTimer");

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonA"))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
    }

    /// <summary>
    /// <code>nNumber</code>번째 중간 통과지점을 통과할 때 호출되는 함수입니다. 
    /// 통과 지점 개수를 늘리고 시간초를 더합니다. 
    /// </summary>
    /// <param name="nNumber">중간 통과지점의 번호입니다. </param>
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

    /// <summary>
    /// 타이머를 작동합니다. 
    /// </summary>
    IEnumerator StartTimer()
    {
        bTimerTicking = true;

        while (nTimeRemained > 0)
        {
            nTimeRemained--;
            textTime.text = "Time: " + nTimeRemained;
            yield return new WaitForSeconds(1.0f);
        }

        if (!bAccomplished)
        {
            bGameOver = true;
            panelGameOver.SetActive(true);
        }
    }
}
