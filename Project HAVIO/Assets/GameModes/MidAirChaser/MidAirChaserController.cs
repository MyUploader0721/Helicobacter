using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

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
 *  - 06-08: Gaze 컨트롤을 이용하여 게임을 종료/재시작할 수 있도록 하였습니다. 
 *  - 06-20: UI 디자인을 좀 바꿨습니다. 임무 선택으로 이동하고 페이드효과를 추가합니다. 
 *  - 07-04: UI 디자인에 따라 임무 성공/실패시 UI를 따로 배치하였습니다. 
 */

public class MidAirChaserController : MonoBehaviour
{
    HelicopterInfo helicopterInfo;

    [Header("Background Music")]
    [SerializeField] AudioClip sfxNotFound;
    [SerializeField] AudioClip sfxChasing;
    [SerializeField] AudioClip sfxAccomplished;
    [SerializeField] AudioClip sfxFailed;
    AudioSource bgmPlayer;

    [Header("UI SFX")]
    [SerializeField] AudioClip sfxHover;
    [SerializeField] AudioClip sfxClick;
    [SerializeField] AudioClip sfxTargetFound;
    [SerializeField] AudioClip sfxMissingAlert;
    AudioSource sfxPlayer;

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
    [SerializeField] GameObject objMainCamera;
    [SerializeField] Transform tfMissionEndPosition;
    [SerializeField] GameObject objCanvas;
    [SerializeField] Text textTime;
    [SerializeField] Text textDistance;
    [SerializeField] Text textMissingAlert;
    bool bIsGameOver = false;

    [Header("UI for Mission End: Accomplished")]
    [SerializeField] GameObject objPanelAccomplished;
    [SerializeField] GameObject textAccomplishedMissionTime;
    [SerializeField] Button btnAccomplishedMissionRestart;
    [SerializeField] Button btnAccomplishedMissionExit;

    [Header("UI for Mission End: Failed")]
    [SerializeField] GameObject objPanelFailed;
    [SerializeField] GameObject textFailedMissionTime;
    [SerializeField] Button btnFailedMissionRestart;
    [SerializeField] Button btnFailedMissionExit;

    [Header("UI for Mission Summary")]
    [SerializeField] GameObject objPanelSummary;
    [SerializeField] GameObject textMissionTime;
    [SerializeField] Button btnSummaryMissionExit;
    [SerializeField] Button btnSummaryMissionRestart;

    [Header("Setting for Fading While Scene Transition")]
    [SerializeField] Image imgPanelFading;
    [SerializeField][Range(1.0f, 2.5f)] float fFadeInTime = 1.5f;
    [SerializeField][Range(1.0f, 2.5f)] float fFadeOutTime = 1.5f;
    [SerializeField] [Range(1.0f, 2.5f)] float fFadeOutAndInTime = 2.5f;
    bool bFadingDone = false;
    bool bQuitGame = false;
    bool bRestartGame = false;
    bool bFadeOutAndInDone = false;

    [Header("Scene when quit")]
    [SerializeField] Object sceneToGo;

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

        objCanvas.transform.SetParent(objMainCamera.transform);

        btnSummaryMissionExit.onClick.AddListener(OnButtonAnyQuitClicked);
        btnSummaryMissionRestart.onClick.AddListener(OnButtonFailedRestartClicked);

        btnAccomplishedMissionRestart.onClick.AddListener(OnButtonFailedRestartClicked);
        btnAccomplishedMissionExit.onClick.AddListener(OnButtonAnyQuitClicked);
        btnFailedMissionRestart.onClick.AddListener(OnButtonFailedRestartClicked);
        btnFailedMissionExit.onClick.AddListener(OnButtonAnyQuitClicked);

        nRemainedTime = nMissionTime;
        textTime.text = "Time: " + nRemainedTime;

        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.clip = sfxNotFound;
        bgmPlayer.volume = 0.0f;
        bgmPlayer.Play();

        sfxPlayer = gameObject.AddComponent<AudioSource>();

        StartCoroutine(FadeIn());
    }
	
	void Update ()
    {
        if (objPanelSummary.activeInHierarchy)
        {
            textMissionTime.GetComponent<TextMeshProUGUI>().text = ((int)(Time.time / 60.0f)).ToString("00") + ":" + ((int)(Time.time % 60)).ToString("00");
        }

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

            sfxPlayer.PlayOneShot(sfxTargetFound);

            bgmPlayer.Stop();
            bgmPlayer.clip = sfxChasing;
            bgmPlayer.Play();

            StartCoroutine("MissionTimer");
        }

        textDistance.text = "Dist.: " + targetBehaviour.fDistance.ToString("0.00") + "m";

        // 목표와의 거리가 멀어질 경우
        if (bChaseStart && targetBehaviour.fDistance > fMaxDistance)
        {
            if (!bMissingAlert && !bAccomplished && !bIsGameOver)
            {
                StartCoroutine("MissingCountdown");
                StopCoroutine("MissionTimer");
            }

            textDistance.color = new Color(0.8046875f, 0.0f, 0.0f);
        }
        // 목표와의 거리가 가까울 경우
        else if (bChaseStart && targetBehaviour.fDistance <= fMaxDistance)
        {
            if (bMissingAlert)
            {
                StopCoroutine("MissingCountdown");
                StartCoroutine("MissionTimer");
                textMissingAlert.gameObject.SetActive(false);
                bMissingAlert = false;
            }

            textDistance.color = new Color(0.1953125f, 0.1953125f, 0.1953125f);
        }

        // (파괴되는 등)시동이 꺼져서 기동이 불가능할 경우: 게임오버
        if (!helicopterInfo.bIsFlyable && !bAccomplished)
        {
            StopCoroutine("MissionTimer");
            if (bMissingAlert)
            {
                StopCoroutine("MissingCountdown");
                textMissingAlert.gameObject.SetActive(false);
            }
            GameOver();
        }

        // 임무 요약창
        if (bFadingDone && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("FaceButtonB")) && !bAccomplished && !bIsGameOver)
        {
            sfxPlayer.PlayOneShot(sfxClick);

            if (!objPanelSummary.activeInHierarchy)
            {
                objPanelSummary.SetActive(true);
                bgmPlayer.Pause();
            }
            else
            {
                objPanelSummary.SetActive(false);
                bgmPlayer.UnPause();
            }
        }

        // 페이드 아웃이 끝나면 끝
        if (bFadingDone && bQuitGame)
			SceneManager.LoadScene("Lobby");
        if (bFadingDone && bRestartGame)
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (!bgmPlayer.isPlaying)
            bgmPlayer.Play();
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
        bgmPlayer.Stop();
        bgmPlayer.clip = sfxAccomplished;
        bgmPlayer.Play();

        bAccomplished = true;

        StartCoroutine("FadeOutAndIn");
        objCanvas.SetActive(false);
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
            sfxPlayer.PlayOneShot(sfxMissingAlert);

            textMissingAlert.text = "" + nMissingCountdown;
            nMissingCountdown--;
            yield return new WaitForSeconds(2.0f);
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
            bgmPlayer.Stop();
            bgmPlayer.clip = sfxFailed;
            bgmPlayer.Play();

            StopCoroutine("MissionTimer");
            bIsGameOver = true;

            StartCoroutine("FadeOutAndIn");
            objCanvas.SetActive(false);
        }
    }

    /// <summary>
    /// 종료 버튼을 클릭했을 때 메인 화면으로 넘어가도록 합니다. 
    /// </summary>
    void OnButtonAnyQuitClicked()
    {
        sfxPlayer.PlayOneShot(sfxClick);
        StartCoroutine(FadeOut());
        bQuitGame = true;
    }

    /// <summary>
    /// 재시작 버튼을 클릭했을 때 해당 씬을 다시 불러오도록 합니다. 
    /// </summary>
    void OnButtonFailedRestartClicked()
    {
        sfxPlayer.PlayOneShot(sfxClick);
        StartCoroutine(FadeOut());
        bRestartGame = true;
    }

    /// <summary>
    /// 페이드인, 화면을 서서히 밝혀줍니다. 
    /// </summary>
    IEnumerator FadeIn()
    {
        bFadingDone = false;
        while (imgPanelFading.color.a > 0.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a - (fFadeInTime / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeInTime / 100.0f);
        }
        bFadingDone = true;
    }

    /// <summary>
    /// 페이드아웃, 화면을 서서히 어둡게 합니다. 
    /// </summary>
    IEnumerator FadeOut()
    {
        bFadingDone = false;
        while (imgPanelFading.color.a < 1.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a + (fFadeOutTime / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeOutTime / 100.0f);
        }
        bFadingDone = true;
    }

    /// <summary>
    /// 페이드아웃과 인을 진행합니다. 
    /// </summary>
    IEnumerator FadeOutAndIn()
    {
        bFadingDone = false;
        while (imgPanelFading.color.a < 1.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a + (fFadeOutAndInTime * 2.0f / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeOutAndInTime * 2.0f / 100.0f);
        }

        objMainCamera.transform.SetParent(tfMissionEndPosition);
        objMainCamera.transform.localPosition = Vector3.zero;
        objMainCamera.transform.localRotation = Quaternion.identity;

        if (bAccomplished)
        {
            objPanelAccomplished.SetActive(true);
            textAccomplishedMissionTime.GetComponent<TextMeshProUGUI>().text = ((int)(Time.time / 60.0f)).ToString("00") + ":" + ((int)(Time.time % 60)).ToString("00");
        }
        else if (bIsGameOver)
        {
            objPanelFailed.SetActive(true);
            textFailedMissionTime.GetComponent<TextMeshProUGUI>().text = ((int)(Time.time / 60.0f)).ToString("00") + ":" + ((int)(Time.time % 60)).ToString("00");
        }
        while (imgPanelFading.color.a > 0.0f)
        {
            imgPanelFading.color = new Color(0.0f, 0.0f, 0.0f, imgPanelFading.color.a - (fFadeOutAndInTime * 2.0f / 100.0f));
            bgmPlayer.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeOutAndInTime * 2.0f / 100.0f);
        }
        bFadingDone = true;
    }
}
