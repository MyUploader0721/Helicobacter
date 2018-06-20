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
    [SerializeField][Range(1.0f, 2.5f)] float fFadeInTime = 1.5f;
    [SerializeField][Range(1.0f, 2.5f)] float fFadeOutTime = 1.5f;

    [Header("Setting for UI Information")]
    [SerializeField] GameObject objPressAnyKey;
    [SerializeField][Range(1.0f, 15.0f)] float fTimeToTransition = 5.0f;

    [Header("Lobby Scene")]
    [SerializeField] Object sceneLobby;

    float fStartTime = 0.0f;
    bool bFadingDone = false;
    bool bAvailableTransition = false;

    void Start ()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = bgmIntro;
        audioSource.volume = 0.0f;
        audioSource.Play();

        fStartTime = Time.time;
        StartCoroutine(FadeIn());
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
                StartCoroutine(FadeOut());
                bAvailableTransition = true;
            }
        }

        if (bFadingDone && bAvailableTransition)
        {
            SceneManager.LoadScene(sceneLobby.name);
        }
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
            audioSource.volume = 1.0f - imgPanelFading.color.a;
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
            audioSource.volume = 1.0f - imgPanelFading.color.a;
            yield return new WaitForSeconds(fFadeOutTime / 100.0f);
        }
        bFadingDone = true;
    }
}
