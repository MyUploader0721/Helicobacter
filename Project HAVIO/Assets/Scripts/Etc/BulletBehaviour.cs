using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("Rocket Behaviour")]
    [SerializeField] float fDuration = 7.5f;
    [SerializeField] float fSpeed = 0.1f;
    [SerializeField] int nDamage;
    [Space]
    AudioSource audioSource;
    [Header("SFX: Explosion")]
    [SerializeField] AudioClip[] sfxHit;
    [SerializeField] AudioClip[] sfxExplosion;
    [Space]
    [SerializeField] GameObject objExplosion;

    float fTimeToFired;
    bool bHit = false;

    void Start ()
    {
        fTimeToFired = Time.time;

        audioSource = GetComponent<AudioSource>();
    }
	
	void Update ()
    {
        if (!bHit)
            transform.Translate(Vector3.forward * fSpeed);

        if (Time.time - fTimeToFired > fDuration)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!bHit)
        {
            bHit = true;
            transform.GetChild(0).gameObject.SetActive(false);

            Destroy(Instantiate(objExplosion, transform.position, Quaternion.identity, null), 2.5f);

            if (collision.CompareTag("Player"))
            {
                audioSource.PlayOneShot(sfxHit[Random.Range(0, sfxHit.Length)]);
                collision.attachedRigidbody.GetComponent<HelicopterInfo>().Damage(nDamage);
            }

            StartCoroutine(PlayHitSound());
        }
    }

    IEnumerator PlayHitSound()
    {
        audioSource.PlayOneShot(sfxExplosion[Random.Range(0, sfxExplosion.Length)]);

        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
