using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SabotageController : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] Transform trsPlayer;
    [SerializeField] HelicopterInfo helicopterInfo;
    [Space]
    [SerializeField] Transform trsCamera;
    [Space]
    [Header("Mission End Panel")]
    [SerializeField] GameObject objAccomplishedPanel;
    [SerializeField] GameObject objFailedPanel;
    [SerializeField] Transform objMissionEndCameraPos;
    [Space]
    [Header("Fader")]
    [SerializeField] SceneFadingController sfc;
    [Space]
    [Header("SFX")]
    [SerializeField] AudioClip sfxAccomplished;
    [SerializeField] AudioClip sfxFailed;
    [Space]
    [SerializeField] AudioClip[] sfxMissionBrief;
    [Space]
    [Header("UI Settings")]
    [SerializeField] GameObject cvsInfo;
    [Space]
    [SerializeField] GameObject objPanelInGameMenu;
    [SerializeField] Button btnExit;
    [SerializeField] Button btnRestart;

    AudioSource audioSource;
    MotionInput motionInput;
    InputController inputController;

    [HideInInspector] public bool bMissionEnd = false;
    bool bMissionAccomplished = false;
    bool bMissionFailed = false;
    bool bRepairing = false;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        motionInput = trsPlayer.GetComponent<MotionInput>();
        inputController = trsPlayer.GetComponent<InputController>();

        inputController.bControllable = false;

        btnExit.onClick.AddListener(delegate { sfc.FadeOutForLoad("Lobby"); });
        btnRestart.onClick.AddListener(delegate { sfc.FadeOutForLoad(SceneManager.GetActiveScene().name); });

        sfc.FadeIn();

        StartCoroutine(MissionStartBrief());

        trsPlayer.GetComponentInChildren<CompetitiveAdvantageDirectionalArrowBehaviour>().trsTarget = GameObject.Find("Narr Block - Entering Mission Area").transform;
    }
	
	void Update ()
    {
		if (!helicopterInfo.bIsFlyable)
        {
            bMissionFailed = true;
        }

        if (!bRepairing && helicopterInfo.bIsFlyable && !helicopterInfo.bIsEngineStart)
        {
            bRepairing = true;
            StartCoroutine(Repair());
        }
        else
        {
            if (bRepairing)
                StopCoroutine(Repair());
        }
        
        if ((bMissionAccomplished || bMissionFailed) && !bMissionEnd)
        {
            bMissionEnd = true;
            cvsInfo.SetActive(false);

            if (bMissionAccomplished)
            {
                objAccomplishedPanel.SetActive(true);
                audioSource.PlayOneShot(sfxAccomplished);
            }
            else if (bMissionFailed)
            {
                objFailedPanel.SetActive(true);
                audioSource.PlayOneShot(sfxFailed);
            }

            sfc.FadeOutAndIn(delegate {
                trsCamera.parent = null;
                motionInput.UseAutoRotation = false;
                motionInput.SetInputValues();
                trsCamera.position = objMissionEndCameraPos.position;
                trsCamera.rotation = objMissionEndCameraPos.rotation;
            });
        }

        // 인게임 메뉴
        if (helicopterInfo.bIsFlyable && (Input.GetButtonDown("FaceButtonB") || Input.GetKeyDown(KeyCode.Escape)))
        {
            objPanelInGameMenu.SetActive(!objPanelInGameMenu.activeInHierarchy);
        }
    }

    IEnumerator MissionStartBrief()
    {
        yield return new WaitForSeconds(1.0f);

        audioSource.PlayOneShot(sfxMissionBrief[0]);
        yield return new WaitForSeconds(sfxMissionBrief[0].length + 0.5f);

        audioSource.PlayOneShot(sfxMissionBrief[1]);
        yield return new WaitForSeconds(sfxMissionBrief[1].length + 0.5f);

        audioSource.PlayOneShot(sfxMissionBrief[2]);
        yield return new WaitForSeconds(sfxMissionBrief[2].length);

        inputController.bControllable = true;
    }

    IEnumerator Repair()
    {
        while (true)
        {
            if (helicopterInfo.nCurrentDurability == helicopterInfo.nMaxDurability) break;
            if (helicopterInfo.bIsEngineStart == true) break;

            helicopterInfo.nCurrentDurability++;
            yield return new WaitForSeconds(1.0f);
        }

        bRepairing = false;
    }

    public void SetMissionVariable(bool bIsSuccess)
    {
        if (bIsSuccess)
            bMissionAccomplished = true;
        else
            bMissionFailed = true;
    }
}
