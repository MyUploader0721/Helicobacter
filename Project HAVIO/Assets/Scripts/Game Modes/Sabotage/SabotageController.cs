﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    AudioSource audioSource;
    MotionInput motionInput;
    InputController inputController;

    [HideInInspector] public bool bMissionEnd = false;
    bool bMissionAccomplished = false;
    bool bMissionFailed = false;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();
        motionInput = trsPlayer.GetComponent<MotionInput>();
        inputController = trsPlayer.GetComponent<InputController>();

        inputController.bControllable = false;

        sfc.FadeIn();

        StartCoroutine(MissionStartBrief());
    }
	
	void Update ()
    {
		if (!helicopterInfo.bIsFlyable)
        {
            bMissionFailed = true;
        }
        
        if ((bMissionAccomplished || bMissionFailed) && !bMissionEnd)
        {
            bMissionEnd = true;

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
}
