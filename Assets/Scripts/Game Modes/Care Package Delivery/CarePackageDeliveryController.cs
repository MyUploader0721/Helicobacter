using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: CarePackageDeliveryController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: CarePackageDelivery 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - Care Package를 배달하는 임무를 수행하는 게임모드를 설정합니다. 
 *  - 정해진 매개변수들을 조절하여 난이도를 변경할 수 있습니다. 
 */

public class CarePackageDeliveryController : MonoBehaviour
{
    [Header("Player Helicopter")]
    [SerializeField] Transform trsPlayer;
    [Space]
    [Header("Game Mode Setting")]
    [SerializeField] int nNumReceiver;
    [SerializeField] DropZoneBehaviour[] dzBehaviour;
    [Space]
    [SerializeField] float fMaxHeight;
    [Space]
    [Header("UI Setting")]
    [SerializeField] Text txtAltitude;
    [SerializeField] Text txtCountdown;
    [SerializeField] Text txtObjectRemained;
    [SerializeField] Text txtPackageLoaded;
    [SerializeField] GameObject objCanvasInfo;
    [SerializeField] GameObject objSuccessPanel;
    [SerializeField] GameObject objFailedPanel;
    [SerializeField] GameObject objPanelInGameMenu;
    [Space]
    [SerializeField] Button btnExit;
    [SerializeField] Button btnRestart;
    [Space]
    [SerializeField] Transform trsMissionEndPosition;
    [SerializeField] Transform trsMainCamera;
    [Space]
    [SerializeField] SceneFadingController sfc;
    [Space]
    [Header("SFX Setting")]
    [SerializeField] AudioClip sfxAltWarning;
    [SerializeField] AudioClip sfxMissionSuccess;
    [SerializeField] AudioClip sfxMissionFailed;
    [Space]
    [SerializeField] AudioClip sfxFailedByDestroyed;
    [SerializeField] AudioClip sfxFailedByTooHigh;
    [Space]
    [SerializeField] AudioClip[] sfxMissionNarr;
    [Space]
    [SerializeField] GameObject objBGMPlayer;

    public int nCurrentReceiver = 0;

    [HideInInspector] public bool bMissionEnd = false;
    [HideInInspector] public bool bMissionAccomplished = false;
    [HideInInspector] public bool bMissionFailed = false;

    bool bFadeOutAndInCalled;

    HelicopterInfo helicopterInfo;
    InputController inputController;
    MotionInput motionInput;
    AudioSource audioSource;

    bool bIsTooHigh;

    void Start ()
    {
        helicopterInfo = trsPlayer.GetComponent<HelicopterInfo>();
        inputController = trsPlayer.GetComponent<InputController>();
        motionInput = trsPlayer.GetComponent<MotionInput>();
        audioSource = GetComponent<AudioSource>();

        btnExit.onClick.AddListener(OnButtonExitClicked);
        btnRestart.onClick.AddListener(OnButtonRestartClicked);

        sfc.FadeIn();

        StartCoroutine("MissionStart");
    }
	
	void Update ()
    {
        // 고도 제한
        if (trsPlayer.position.y > fMaxHeight)
        {
            if (!bIsTooHigh)
            {
                txtAltitude.color = Color.red;
                StartCoroutine("AltitudeWarning");
            }
        }
        else
        {
            if (bIsTooHigh)
            {
                txtAltitude.color = new Color(0.9296875f, 0.9296875f, 0.9296875f);
                if (txtCountdown.gameObject.activeInHierarchy)
                    txtCountdown.gameObject.SetActive(false);
                StopCoroutine("AltitudeWarning");
                bIsTooHigh = false;
            }
        }

        // 추락사
        if (!helicopterInfo.bIsFlyable && !bMissionEnd)
        {
            AudioSource a = gameObject.AddComponent<AudioSource>();
            a.PlayOneShot(sfxFailedByDestroyed);

            bMissionEnd = true;
            bMissionFailed = true;
        }

        // 승리조건
        if (dzBehaviour.Length == nCurrentReceiver)
        {
            bMissionEnd = true;
        }

        // 임무가 끝나버림
        if (bMissionEnd && (bMissionAccomplished || bMissionFailed))
        {
            inputController.bControllable = false;
            objCanvasInfo.SetActive(false);

            if (objBGMPlayer != null)
                objBGMPlayer.SetActive(false);

            if (!bFadeOutAndInCalled)
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                if (bMissionFailed)
                    audioSource.PlayOneShot(sfxMissionFailed);
                else if (bMissionAccomplished)
                    audioSource.PlayOneShot(sfxMissionSuccess);

                if (motionInput.UseAutoRotation)
                {
                    motionInput.UseAutoRotation = false;
                    motionInput.SetInputValues(0.0f);
                }

                bFadeOutAndInCalled = true;

                sfc.FadeOutAndIn(delegate {
                    trsMainCamera.SetParent(trsMissionEndPosition);
                    trsMainCamera.localPosition = Vector3.zero;
                    trsMainCamera.localRotation = Quaternion.identity;

                    if (bMissionFailed)
                        objFailedPanel.SetActive(true);
                    else if (bMissionAccomplished)
                        objSuccessPanel.SetActive(true);
                });
            }
        }

        // UI
        if (dzBehaviour.Length - nCurrentReceiver > 0)
            txtObjectRemained.text = "OBJ: " + (dzBehaviour.Length - nCurrentReceiver) + " LEFT";
        else
            if (txtObjectRemained.enabled) txtObjectRemained.enabled = false;
        txtPackageLoaded.enabled = (helicopterInfo.objCargo != null);
        // 인게임 메뉴
        if (!bMissionEnd && helicopterInfo.bIsFlyable && (Input.GetButtonDown("FaceButtonB") || Input.GetKeyDown(KeyCode.Escape)))
        {
            objPanelInGameMenu.SetActive(!objPanelInGameMenu.activeInHierarchy);
        }
    }

    IEnumerator MissionStart()
    {
        inputController.bControllable = false;

        for (int i = 0; i < sfxMissionNarr.Length; i++)
        {
            audioSource.PlayOneShot(sfxMissionNarr[i]);
            yield return new WaitForSecondsRealtime(sfxMissionNarr[i].length);
        }

        inputController.bControllable = true;
    }

    IEnumerator AltitudeWarning()
    {
        bIsTooHigh = true;
        txtCountdown.gameObject.SetActive(true);

        audioSource.PlayOneShot(sfxAltWarning);
        txtCountdown.color = Color.yellow;
        txtCountdown.text = "3";
        yield return new WaitForSeconds(1.5f);

        audioSource.PlayOneShot(sfxAltWarning);
        txtCountdown.color = Color.red;
        txtCountdown.text = "2";
        yield return new WaitForSeconds(1.5f);

        audioSource.PlayOneShot(sfxAltWarning);
        txtCountdown.text = "1";
        yield return new WaitForSeconds(1.5f);

        txtCountdown.gameObject.SetActive(false);

        AudioSource a = gameObject.AddComponent<AudioSource>();
        a.PlayOneShot(sfxFailedByTooHigh);

        bMissionEnd = true;
        bMissionFailed = true;
    }

    void OnButtonExitClicked()
    {
        sfc.FadeOutForLoad(0);
    }

    void OnButtonRestartClicked()
    {
        sfc.FadeOutForLoad(SceneManager.GetActiveScene().buildIndex);
    }
}
