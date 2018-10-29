using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrationPlayer : MonoBehaviour
{
    AudioSource audioSource;

    [SerializeField] AudioClip sfxNarration;
    [Space]
    [SerializeField] PDStyleBGMPlayer bgmplayer;
    [SerializeField] bool bInFrontOfBase = false;

    bool bEntered = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!bEntered && other.CompareTag("Player"))
        {
            bEntered = true;
            StartCoroutine(PlayNarration());

            if (bInFrontOfBase)
                bgmplayer.GoAssaultBGM();
        }
    }

    IEnumerator PlayNarration()
    {
        audioSource.PlayOneShot(sfxNarration);
        yield return new WaitForSeconds(sfxNarration.length);

        Destroy(gameObject);
    }
}
