using System.Collections;
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

    AudioSource audioSource;

    [HideInInspector] public bool bMissionEnd = false;
    bool bMissionAccomplished = false;
    bool bMissionFailed = false;

	void Start ()
    {
        audioSource = GetComponent<AudioSource>();

        sfc.FadeIn();
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
                trsCamera.position = objMissionEndCameraPos.position;
                trsCamera.rotation = objMissionEndCameraPos.rotation;
            });
        }
	}
}
