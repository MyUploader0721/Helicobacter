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
 *  - 08-23: UI를 개선합니다. 
 *  - 09-15: BGM
 */

public class RaceController : MonoBehaviour
{
    HelicopterInfo helicopterInfo;
    MotionInput motionInput;

    [Header("Player Helicopter")]
    [SerializeField] GameObject objPlayer;
    [SerializeField] GameObject objCamera;
    [Space]
    [SerializeField] Transform trsEndMissionCamPos;
    [Space]
    [Header("Helicopter Setting")]
    [SerializeField] bool bIsPlayWithGamePad = false;
    [Space]
    [Header("Helicopter Armament Setting(Recommends default)")]
    [SerializeField] bool bUseSearchLight = true;
    [Space]
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
    [Space]
    [Header("AI Setting")]
    [SerializeField] RaceAIHelicopterController[] raceAIHeliController;
    [Space]
    [Header("UI Setting")]
    [SerializeField] GameObject panelGameOver;
    [SerializeField] GameObject panelAccomplished;
    [Space]
    [SerializeField] GameObject objCanvasScreenSpaced;
    [SerializeField] Text textTime;
    [SerializeField] Text textGoalLeft;
    [SerializeField] Text textHP;
    [Space]
    [SerializeField] Text textCountdown;
    [Space] 
    [SerializeField] SceneFadingController sceneFadingController;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioClip sfxBGM;
    [Space]
    [SerializeField] AudioClip sfxAccomplished;
    [SerializeField] AudioClip sfxFailed;
    [Space]
    [SerializeField] AudioClip sfxCountdown;
    [SerializeField] AudioClip sfxGo;

    AudioSource audioSourceBGM = new AudioSource();
    AudioSource audioSourceSFX = new AudioSource();

    bool bIsOnCountdown = true;

    void Start()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        audioSourceBGM = gameObject.AddComponent<AudioSource>();
        audioSourceBGM.clip = sfxBGM;
        audioSourceBGM.loop = true;
        audioSourceBGM.Play();

        audioSourceSFX = gameObject.AddComponent<AudioSource>();
        audioSourceSFX.volume = 0.75f;

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseSearchLight = bUseSearchLight;
        helicopterInfo.bIsFlyable = false;

        motionInput = objPlayer.GetComponent<MotionInput>();

        for (int i = 0; i < rpbPassages.Length; i++)
        {
            rpbPassages[i].SetNumber(i);
        }

        nNumPassages = rpbPassages.Length;

        sceneFadingController.FadeIn();

        StartCoroutine("StartCountdown");
    }

    void Update()
    {
        textGoalLeft.text = "Goal Left: " + (nNumPassages - nPassedPassages) + "/" + nNumPassages;
        textHP.text = "HP: " + helicopterInfo.nCurrentDurability + " / " + helicopterInfo.nMaxDurability;

        // 다음 목표 표시(Target Navigation)
        if (nPassedPassages < rpbPassages.Length)
        {
            Vector3 v3PosNav = rpbPassages[nPassedPassages].transform.position;
            objTargetNav.transform.position = v3PosNav;
        }
        else
        {
            Vector3 v3PosNav = objGoal.transform.position;
            objTargetNav.transform.position = v3PosNav;
        }

        if (!bIsOnCountdown && !objPlayer.GetComponent<HelicopterInfo>().bIsFlyable && !bAccomplished)
        {
            bGameOver = true;
            if (!panelGameOver.activeInHierarchy)
                panelGameOver.SetActive(true);
        }

        if (bGameOver || bAccomplished)
        {
            audioSourceBGM.Stop();

            if (objCamera.transform.parent != null)
            {
                objCamera.transform.SetParent(null);

                objCanvasScreenSpaced.SetActive(false);

                sceneFadingController.FadeOutAndIn(delegate {
                    objCamera.transform.position = trsEndMissionCamPos.position;
                    objCamera.transform.rotation = trsEndMissionCamPos.rotation;
                });
            }

            if (motionInput.UseAutoRotation)
            {
                motionInput.UseAutoRotation = false;
                motionInput.SetInputValues(0.0f);
            }

            // 임무 실패
            if (bGameOver)
            {
                if (!audioSourceSFX.isPlaying)
                    audioSourceSFX.PlayOneShot(sfxFailed);

                if (!panelGameOver.activeInHierarchy)
                    panelGameOver.SetActive(true);

                if (bTimerTicking)
                    StopCoroutine("StartTimer");

                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonB"))
                {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
				    Application.Quit();
                #endif
                }
                else if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("JoystickButtonA"))
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
            // 임무 성공
            if (bAccomplished)
            {
                if (!audioSourceSFX.isPlaying)
                    audioSourceSFX.PlayOneShot(sfxAccomplished);

                if (!panelAccomplished.activeInHierarchy)
                    panelAccomplished.SetActive(true);

                if (bTimerTicking)
                    StopCoroutine("StartTimer");

                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("JoystickButtonA"))
                {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
                }
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

    IEnumerator StartCountdown()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        textCountdown.gameObject.SetActive(true);

        for (int i = 0; i < 3; i++)
        {
            audioSourceSFX.PlayOneShot(sfxCountdown);
            textCountdown.text = (3 - i).ToString();
            switch (i)
            {
                case 0: textCountdown.color = Color.green; break;
                case 1: textCountdown.color = Color.yellow; break;
                case 2: textCountdown.color = Color.red; break;
            }
            yield return new WaitForSecondsRealtime(1.0f);
        }

        audioSourceSFX.PlayOneShot(sfxGo);
        foreach (RaceAIHelicopterController c in raceAIHeliController)
        {
            c.bActiveAI = true;
            helicopterInfo.bIsFlyable = true;
            bIsOnCountdown = false;
        }

        textCountdown.text = "GO!";
        StartCoroutine("StartTimer");
        yield return new WaitForSecondsRealtime(1.0f);
        textCountdown.gameObject.SetActive(false);
    }
}
