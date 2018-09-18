using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Helicopter Durability")]
    public int nMaxDurability = 100;
    public int nCurrentDurability = 0;
    public int nArmor = 0;

    [Header("Available Attachments")]
    [SerializeField] GameObject objSearchLight;
    public bool bUseSearchLight = false;
    [SerializeField] GameObject objInnerPod;
    public bool bUseInnerPod = false;
    [SerializeField] GameObject objOuterPod;
    public bool bUseOuterPod = false;
    // enum Armament { NONE, ROCKET_POD, MACHINEGUN_POD }
    // [SerializeField] Armament armInnerPod = Armament.NONE;
    // [SerializeField] Armament armOuterPod = Armament.NONE;
    public GameObject objCargo;

    AudioSource[] audioSource;
    enum SFX_List{ HELICOPTER_STATUS, HELICOPTER_CRASH };
    [Header("SFX: Helicopter Status")]
    [SerializeField] AudioClip sfx30Sound;
    [SerializeField] AudioClip sfx20Sound;
    [SerializeField] AudioClip sfx10Sound;
    [SerializeField] AudioClip sfxCrashedSound;
    [Header("SFX: Helicopter Crash")]
    [SerializeField] AudioClip[] sfxCrash;

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

        if (objInnerPod != null) objInnerPod.SetActive(bUseInnerPod);
        if (objOuterPod != null) objOuterPod.SetActive(bUseOuterPod);
        if (objSearchLight != null) objSearchLight.SetActive(bUseSearchLight);
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
        if (collider.CompareTag("Terrain"))
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
}
