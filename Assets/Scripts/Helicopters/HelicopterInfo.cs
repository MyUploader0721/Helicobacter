﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 *       TITLE: HelicopterInfo.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-06
 * DESCRIPTION: 헬리콥터의 기본 정보를 지니고 있는 클래스
 *     DEV LOG: 
 *  - 예전에 작성하던 스크립트가 완전히 재사용 불가능한 수준이었습니다. 
 *    (특정 헬리콥터에 종속적임) 따라서 재사용 가능한 스크립트 작성을 목표로 
 *    재작성하도록 하였습니다. 
 *  - 헬리콥터의 기본 정보를 가지고 있습니다. 비행가능상태 등..
 */

public class HelicopterInfo : MonoBehaviour
{
    [Header("Helicopter Status")]
    public bool bIsEngineStart     = false;
    public bool bIsFlyable         = true;
    public bool bIsPlayWithGamePad = false;
    [Space]
    [Header("Helicopter Durability")]
    public int nMaxDurability = 100;
    public int nCurrentDurability = 0;
    public int nArmor = 0;
    [Space]
    [Header("Available Attachments")]
    [SerializeField] GameObject objSearchLight;
    public bool bUseSearchLight = false;
    public GameObject objCargo;
    [Space]
    AudioSource[] audioSource;
    enum SFX_List{ HELICOPTER_STATUS, HELICOPTER_CRASH };
    [Header("SFX: Helicopter Status")]
    [SerializeField] AudioClip sfx30Sound;
    [SerializeField] AudioClip sfx20Sound;
    [SerializeField] AudioClip sfx10Sound;
    [SerializeField] AudioClip sfxCrashedSound;
    [Header("SFX: Helicopter Crash")]
    [SerializeField] AudioClip[] sfxCrash;
    [Space]
    [Header("UI Setting")]
    [SerializeField] Text txtAltitude;
    [SerializeField] Text txtVelocity;
    [SerializeField] Text txtDurability;

    FlightController flightController;

    const float fSafeVelocity = 3.0f;

    void Start ()
    {
        flightController = GetComponent<FlightController>();

        audioSource = new AudioSource[2];
        audioSource[(int)SFX_List.HELICOPTER_STATUS] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.HELICOPTER_STATUS].maxDistance = 3.0f;
        audioSource[(int)SFX_List.HELICOPTER_STATUS].volume = 0.5f;
        audioSource[(int)SFX_List.HELICOPTER_CRASH] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.HELICOPTER_CRASH].maxDistance = 3.0f;

        nCurrentDurability = nMaxDurability;
	}

	void Update ()
    {
		if (nCurrentDurability <= 0)
        {
            bIsFlyable = false;
        }

        SFX_HelicopterStatus();

        if (objSearchLight != null) objSearchLight.SetActive(bUseSearchLight);

        // UI
        if (txtAltitude) txtAltitude.text = "ALT: " + flightController.transform.position.y.ToString("0.00") + "m";
        if (txtVelocity) txtVelocity.text = "VEL: " + flightController.fVelocity.ToString("0.00") + "m/s";
        if (txtDurability) txtDurability.text = "HP: " + nCurrentDurability + "/" + nMaxDurability;
    }

    void OnCollisionEnter(Collision other)
    {
        float fDeltaVelocity = flightController.fVelocity - fSafeVelocity;

        if (fDeltaVelocity > 0.0f)
        {
            int rand = Random.Range(0, sfxCrash.Length);

            nCurrentDurability -= Mathf.FloorToInt(fDeltaVelocity);
            audioSource[(int)SFX_List.HELICOPTER_CRASH].PlayOneShot(sfxCrash[rand], (flightController.fVelocity - fSafeVelocity) / fSafeVelocity);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Bullet")) return;
        else if (collider.CompareTag("Package")) return;
        else if (collider.CompareTag("Flag")) return;
        else if (collider.CompareTag("Narr Block")) return;
        else if (collider.CompareTag("Ignorable")) return;
        else if (collider.CompareTag("Explosion Radius")) return;
        else
        {
            nCurrentDurability = 0;
        }
    }

    /// <summary>
    /// 효과음을 설정하고 재생하는 메소드입니다. 
    /// </summary>
    void SFX_HelicopterStatus()
    {
        if (bIsFlyable)
        {
            float fDurabilityPercent = (float)nCurrentDurability / nMaxDurability * 100.0f;

            if (fDurabilityPercent > 30.0f)
                audioSource[(int)SFX_List.HELICOPTER_STATUS].clip = null;
            else if ((20.0f < fDurabilityPercent && fDurabilityPercent <= 30.0f) &&
                     (audioSource[(int)SFX_List.HELICOPTER_STATUS].clip != sfx30Sound))
                audioSource[(int)SFX_List.HELICOPTER_STATUS].clip = sfx30Sound;
            else if ((10.0f < fDurabilityPercent && fDurabilityPercent <= 20.0f) &&
                     (audioSource[(int)SFX_List.HELICOPTER_STATUS].clip != sfx20Sound))
                audioSource[(int)SFX_List.HELICOPTER_STATUS].clip = sfx20Sound;
            else if ((0.0f < fDurabilityPercent && fDurabilityPercent <= 10.0f) &&
                     (audioSource[(int)SFX_List.HELICOPTER_STATUS].clip != sfx10Sound))
                audioSource[(int)SFX_List.HELICOPTER_STATUS].clip = sfx10Sound;
        }
        else
        {
            if (nCurrentDurability <= 0)
                audioSource[(int)SFX_List.HELICOPTER_STATUS].clip = sfxCrashedSound;
        }

        if (
            audioSource[(int)SFX_List.HELICOPTER_STATUS].clip != null &&
            !audioSource[(int)SFX_List.HELICOPTER_STATUS].isPlaying
           ) audioSource[(int)SFX_List.HELICOPTER_STATUS].Play();
    }

    public void Damage(int nDamage)
    {
        nCurrentDurability -= nDamage;
    }
}
