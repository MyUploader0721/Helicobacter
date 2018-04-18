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
    [SerializeField] float fDuration = 15.0f;
    [SerializeField] float fSpeed = 0.1f;
    public float fDamage = 0.0f;

    enum SFX_List { ROCKET_LAUNCH = 0 }
    AudioSource []audioSource;
    [Header("SFX: Rocket LAUNCHES")]
    [SerializeField] AudioClip[] sfxRocketLaunch;

    float fShotTime = 0.0f;

    void Start()
    {
        fShotTime = Time.time;

        audioSource = new AudioSource[1];
        audioSource[(int)SFX_List.ROCKET_LAUNCH] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.ROCKET_LAUNCH].volume = 0.5f;
        audioSource[(int)SFX_List.ROCKET_LAUNCH].maxDistance = 5.0f;
        audioSource[(int)SFX_List.ROCKET_LAUNCH].spatialBlend = 1.0f;
        int rand = Random.Range(0, sfxRocketLaunch.Length);
        audioSource[(int)SFX_List.ROCKET_LAUNCH].clip = sfxRocketLaunch[rand];
        audioSource[(int)SFX_List.ROCKET_LAUNCH].Play();
    }

    void FixedUpdate()
    {
        if (Time.time - fShotTime > fDuration)
        {
            Destroy(gameObject);
        }

        transform.Translate(Vector3.forward * fSpeed);
    }

    void OnTriggerEnter(Collider collision)
    {
        Destroy(gameObject);
    }
}
