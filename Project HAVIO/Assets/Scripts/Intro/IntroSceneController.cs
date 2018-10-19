using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/**
 *       TITLE: IntroSceneController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-06-19
 * DESCRIPTION: 게임의 인트로 씬을 컨트롤하는 스크립트입니다. 
 *     DEV LOG: 씬의 전환에 페이드인/아웃을 넣어 부드러운 화면 전환을 지원합니다. 
 *              일정 시간이 지나면 다음 씬으로 넘어갈 수 있도록 합니다. 
 */

public class IntroSceneController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] AudioClip bgmIntro;
    AudioSource audioSource;

    [Header("Setting for Fading While Scene Transition")]
    [SerializeField] Image imgPanelFading;
    [SerializeField] SceneFadingController sceneFadingController;

    [Header("Setting for UI Information")]
    [SerializeField] GameObject objPressAnyKey;
    [SerializeField][Range(1.0f, 15.0f)] float fTimeToTransition = 5.0f;

    float fStartTime = 0.0f;

    void Start ()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgmIntro;
        audioSource.volume = 0.0f;
        audioSource.Play();

        fStartTime = Time.time;

        sceneFadingController.FadeIn();
    }
	
	void Update ()
    {
		if (Time.time > fStartTime + fTimeToTransition && !objPressAnyKey.activeInHierarchy)
        {
            objPressAnyKey.SetActive(true);
        }

        if (objPressAnyKey.activeInHierarchy)
        {
            if (Input.anyKeyDown || Input.GetButtonDown("FaceButtonA"))
            {
                sceneFadingController.FadeOut(false);
            }
        }

        if (!audioSource.isPlaying)
            audioSource.Play();
	}
}
