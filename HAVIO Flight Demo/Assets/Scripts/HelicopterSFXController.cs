using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayStartEngine()
    {
        StopAllCoroutines();
        StartCoroutine(EngineStart());
    }

    IEnumerator EngineStart()
    {
        while (audioSource[0].pitch < 1.0f)
        {
            audioSource[0].pitch += 0.03125f;
            yield return new WaitForSeconds(0.03125f * 3);
        }

        bEngineStart = true;
    }

    public void PlayKillEngine()
    {
        StopAllCoroutines();
        StartCoroutine(EngineKill());
    }

    IEnumerator EngineKill()
    {
        bEngineStart = false;

        while (audioSource[0].pitch > 0.0f)
        {
            audioSource[0].pitch -= 0.03125f;
            yield return new WaitForSeconds(0.03125f * 3);
        }
    }
}
