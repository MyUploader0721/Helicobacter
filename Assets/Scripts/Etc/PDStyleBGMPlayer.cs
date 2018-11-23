using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDStyleBGMPlayer : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [Space]
    [SerializeField] AudioClip sfxStealth;
    [SerializeField] AudioClip sfxAssault;
    [Space]
    [SerializeField] HelicopterInfo helicopterInfo;

    bool bIsStealth = true;

	void Start ()
    {
        audioSource.loop = true;

        audioSource.clip = sfxStealth;
        audioSource.Play();
    }

    void Update()
    {
        if (!helicopterInfo.bIsFlyable && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void GoStealthBGM()
    {
        if (!bIsStealth)
        {
            bIsStealth = true;

            audioSource.Stop();
            audioSource.clip = sfxStealth;
            audioSource.Play();
        }
    }

    public void GoAssaultBGM()
    {
        if (bIsStealth)
        {
            bIsStealth = false;

            audioSource.Stop();
            audioSource.clip = sfxAssault;
            audioSource.Play();
        }
    }
}
