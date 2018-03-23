using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 2. 11.
   Description : 헬리콥터의 전체적인 효과음을 담당하는 스크립트
   Edit Log    : 
    - 사용법 : 
     * PlayStartEngine()과 PlayKillEngine()는
       HelicopterFlightController의 ToggleEngine()에서밖에 사용되지
       않습니다. 
   ================================================================= */

public class HelicopterSFXController : MonoBehaviour
{
    AudioSource[] audioSource;
    HelicopterFlightController hfc;

    public AudioClip sfxNormalFlight;
    public AudioClip sfxTurbulance;

    bool bEngineStart = false;

    void Start ()
    {
        audioSource = new AudioSource[2];
        hfc = GetComponent<HelicopterFlightController>();

        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i] = gameObject.AddComponent<AudioSource>();
        }

        audioSource[0].clip = sfxNormalFlight;
        audioSource[0].loop = true;
        audioSource[0].pitch = 0.0f;
        audioSource[0].Play();

        audioSource[1].clip = sfxTurbulance;
        audioSource[1].loop = true;
        audioSource[1].volume = 0.0f;
        audioSource[1].Play();
    }

    void FixedUpdate()
    {
        if (bEngineStart)
            audioSource[0].pitch = 1.0f + (hfc.fCollective / 10.0f);

        if (hfc.fVelocity > 2.75f)
            audioSource[1].volume = (hfc.fVelocity - 2.75f) / 5.0f;
        else
            audioSource[1].volume = 0.0f;
    }

    /// <summary>
    /// 엔진 시동을 켤 때 엔진과 날개 회전 소리가 점점 빨라집니다. 
    /// HelicopterFlightController의 ToggleEngine()에서 사용됩니다. 
    /// </summary>
    public void PlayStartEngine()
    {
        StopAllCoroutines();
        StartCoroutine(EngineStartCoroutine());
    }
    /// <summary>
    /// 엔진 시동을 켤 때 해당 사운드의 pitch를 점점 증가시킵니다. 
    /// PlayStartEngine()에서 사용됩니다. 
    /// </summary>
    IEnumerator EngineStartCoroutine()
    {
        while (audioSource[0].pitch < 1.0f)
        {
            audioSource[0].pitch += 0.03125f;
            yield return new WaitForSeconds(0.03125f * 3);
        }

        bEngineStart = true;
    }

    /// <summary>
    /// 엔진 시동을 끌 때 엔진과 날개 회전 소리가 점점 느려집니다. 
    /// HelicopterFlightController의 ToggleEngine()에서 사용됩니다. 
    /// </summary>
    public void PlayKillEngine()
    {
        StopAllCoroutines();
        StartCoroutine(EngineKillCoroutine());
    }
    /// <summary>
    /// 엔진 시동을 끌 때 해당 사운드의 pitch를 점점 감소시킵니다. 
    /// PlayKillEngine()에서 사용됩니다. 
    /// </summary>
    IEnumerator EngineKillCoroutine()
    {
        bEngineStart = false;

        while (audioSource[0].pitch > 0.0f)
        {
            audioSource[0].pitch -= 0.03125f;
            yield return new WaitForSeconds(0.03125f * 3);
        }
    }
}
