using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: RocketBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: 로켓의 행동을 정의한 스크립트
 *     DEV LOG: 
 *  - 로켓이 발사되면서의 행동을 정의한 스크립트입니다. 
 *  - 소리를 내며 일정한 속도와 방향으로 이동하다가 일정 시간 이후에 소멸합니다. 
 */

public class RocketBehaviour : MonoBehaviour
{
    [Header("Rocket Behaviour")]
    [SerializeField] float fDuration = 7.5f;
    [SerializeField] float fSpeed = 0.1f;
    [SerializeField] int nDamage;
    [SerializeField] float fExpRadius;
    [Space]
    [Header("Splash Damage Setting")]
    [SerializeField] GameObject objExpRadPref;
    [Space]
    AudioSource audioSource;
    [Header("SFX: Rocket Engine")]
    [SerializeField] AudioClip[] sfxRocketEngine;
    [Space]
    [Header("SFX: Explosion")]
    [SerializeField] AudioClip[] sfxExplosion;
    [Space]
    [SerializeField] GameObject objExplosion;

    float fTimeToLaunched;
    bool bExploded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(sfxRocketEngine[Random.Range(0, sfxRocketEngine.Length)]);

        fTimeToLaunched = Time.time;
    }

    void FixedUpdate()
    {
        if (!bExploded)
            transform.Translate(Vector3.forward * fSpeed);

        if (Time.time - fTimeToLaunched > fDuration)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (!bExploded)
        {
            bExploded = true;
            transform.GetChild(0).gameObject.SetActive(false);

            Instantiate(objExplosion, transform.position, Quaternion.identity, null);

            if (collision.CompareTag("Enemy"))
            {
                //Explosion!
            }

            GameObject objExpRad = Instantiate(objExpRadPref, transform.position, Quaternion.identity, null);
            objExpRad.GetComponent<ExplosionRadius>().Init(nDamage, fExpRadius);
            Destroy(objExpRad, 0.1f);

            StartCoroutine(PlayExplosionSound());
        }
    }

    IEnumerator PlayExplosionSound()
    {
        audioSource.Stop();
        audioSource.spatialBlend = 0.5f;
        audioSource.volume = 1.0f;
        audioSource.maxDistance = 100.0f;
        audioSource.PlayOneShot(sfxExplosion[Random.Range(0, sfxExplosion.Length)]);

        while (audioSource.isPlaying)
        {
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}
