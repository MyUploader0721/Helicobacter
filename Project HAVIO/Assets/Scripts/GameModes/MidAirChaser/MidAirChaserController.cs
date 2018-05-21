using System.Collections;
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
    bool bMissingAlert = false;
    bool bChaseStart = false;
    int nMissingCountdown = 3;

    [Header("UI Setting")]
    [SerializeField] GameObject objAccomplishedPanel;
    [SerializeField] GameObject objGameOverPanel;
    [SerializeField] Text textTime;
    [SerializeField] Text textDistance;
    [SerializeField] Text textMissingAlert;
    bool bIsGameOver = false;

    void Start ()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseInnerPod = bUseInnerPod;
        helicopterInfo.bUseOuterPod = bUseOuterPod;
        helicopterInfo.bUseSearchLight = bUseSearchLight;

        targetBehaviour = objTarget.GetComponent<MACTargetBehaviour>();

        nRemainedTime = nMissionTime;

        StartCoroutine("MissionTimer");
    }
	
	void Update ()
    {
        textDistance.text = "Dist.: " + targetBehaviour.fDistance.ToString("0.00") + "m";
        if (bIsStartInMidAir)
        {
            bIsStartInMidAir = false;
            objPlayer.GetComponent<InputController>().ToggleEngine();
        }

        if (bChaseStart && targetBehaviour.fDistance > fMaxDistance)
        {
            if (!bMissingAlert)
                StartCoroutine("MissingCountdown");

            textDistance.color = new Color(0.8046875f, 0.0f, 0.0f);
            Debug.Log("Missing your target!: " + targetBehaviour.fDistance + "m");
        }
        else if (bChaseStart && targetBehaviour.fDistance <= fMaxDistance)
        {
            if (bMissingAlert)
            {
                StopCoroutine("MissingCountdown");
                textMissingAlert.gameObject.SetActive(false);
                bMissingAlert = false;
            }

            textDistance.color = new Color(0.1953125f, 0.1953125f, 0.1953125f);
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
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonA"))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
        if (bIsGameOver)
        {
            // 재시작
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("JoystickButtonB"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            // 종료
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonA"))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        // 05-21: 추격을 시작합니다. 
        if (!bChaseStart && targetBehaviour.fDistance <= fMaxDistance)
        {
            bChaseStart = true;
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
            textTime.text = "Time: " + nRemainedTime;
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("Mission Accomplished!");
        bAccomplished = true;
        objAccomplishedPanel.SetActive(true);
    }

    IEnumerator MissingCountdown()
    {
        textMissingAlert.gameObject.SetActive(true);

        bMissingAlert = true;
        nMissingCountdown = 3;

        while (nMissingCountdown > 0)
        {
            textMissingAlert.text = "" + nMissingCountdown;
            nMissingCountdown--;
            yield return new WaitForSeconds(1.0f);
        }

        textMissingAlert.gameObject.SetActive(false);

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
