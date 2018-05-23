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
        helicopterInfo.bUseInnerPod       = bUseInnerPod;
        helicopterInfo.bUseOuterPod       = bUseOuterPod;
        helicopterInfo.bUseSearchLight    = bUseSearchLight;

        targetBehaviour = objTarget.GetComponent<MACTargetBehaviour>();

        nRemainedTime = nMissionTime;
        textTime.text = "Time: " + nRemainedTime;
    }
	
	void Update ()
    {
        // 플레이어가 공중에서 시작할 경우
        if (bIsStartInMidAir)
        {
            bIsStartInMidAir = false;
            objPlayer.GetComponent<InputController>().ToggleEngine();
        }

        // 05-21: 추격을 시작합니다. 
        if (!bChaseStart && Vector3.Distance(targetBehaviour.transform.position, objPlayer.transform.position) <= fMaxDistance)
        {
            bChaseStart = true;
            StartCoroutine("MissionTimer");
        }

        textDistance.text = "Dist.: " + targetBehaviour.fDistance.ToString("0.00") + "m";

        // 목표와의 거리가 멀어질 경우
        if (bChaseStart && targetBehaviour.fDistance > fMaxDistance)
        {
            if (!bMissingAlert && !bAccomplished && !bIsGameOver)
                StartCoroutine("MissingCountdown");

            textDistance.color = new Color(0.8046875f, 0.0f, 0.0f);
            Debug.Log("Missing your target!: " + targetBehaviour.fDistance + "m");
        }
        // 목표와의 거리가 가까울 경우
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

        // (파괴되는 등)시동이 꺼져서 기동이 불가능할 경우: 게임오버
        if (!helicopterInfo.bIsFlyable)
        {
            StopCoroutine("MissionTimer");
            GameOver();
        }

        // DEVLOG 05-09: 게임오버 UI
        // 추격이 성공하였을 경우: 
        if (bAccomplished)
        {
            // 종료
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonA"))
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }
        // 추격을 실패하였을 경우: 
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

    /// <summary>
    /// 플레이어가 타겟과 <code>fMaxDistance</code>만큼 떨어져 있을 경우 
    /// 3초 카운트를 셀 때까지 돌아와야 하는데 그 카운트다운을 해주는 코루틴 함수입니다. 
    /// </summary>
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
    /// 미션을 실패하여 게임 오버가 되었을 경우
    /// 게임 오버를 화면에 출력하고 다시 시작 또는 종료를 유도합니다. 
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
