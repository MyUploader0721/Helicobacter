using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompetitiveAdvantageHitSFXPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] sfxHit;

    AudioSource audioSource;

	void Start ()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(sfxHit[Random.Range(0, sfxHit.Length)]);
        }
    }
}
