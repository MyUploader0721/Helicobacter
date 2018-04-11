using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
