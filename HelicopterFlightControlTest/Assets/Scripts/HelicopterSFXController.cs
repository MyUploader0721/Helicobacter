using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterSFXController : MonoBehaviour
{
    AudioSource[] audioSource;
    HelicopterFlightController heliController;

    public AudioClip sfxStartUp;
    public AudioClip sfxNormal;
    public AudioClip sfxRiseCollective;
    public AudioClip sfxKeepCollectiveUp;
    public AudioClip sfxReleaseCollective;
    public AudioClip sfxStopEngine;

    void Start ()
    {
        audioSource = new AudioSource[2];
        heliController = GetComponent<HelicopterFlightController>();

        for (int i = 0; i < 2; i++)
            audioSource[i] = gameObject.AddComponent<AudioSource>();

        audioSource[1].volume = 0.35f;
	}

    public void PlayStartEngine()
    {
        audioSource[0].Stop();
        audioSource[0].clip = sfxStartUp;
        audioSource[0].Play();
    }

    public void PlayNormal()
    {
        if (audioSource[0].isPlaying == false)
        {
            audioSource[0].clip = sfxNormal;
            audioSource[0].Play();
        }
    }

    public void PlayKillEngine()
    {
        audioSource[0].Stop();
        audioSource[0].clip = sfxStopEngine;
        audioSource[0].Play();
    }

    public void PlayUpCollective()
    {
        audioSource[1].Stop();
        audioSource[1].clip = sfxRiseCollective;
        audioSource[1].Play();
    }

    public void PlayKeepCollective()
    {
        if (audioSource[1].isPlaying == false)
        {
            audioSource[1].clip = sfxKeepCollectiveUp;
            audioSource[1].Play();
        }
    }

    public void PlayDownCollective()
    {
        audioSource[1].Stop();
        audioSource[1].clip = sfxReleaseCollective;
        audioSource[1].Play();
    }
}
