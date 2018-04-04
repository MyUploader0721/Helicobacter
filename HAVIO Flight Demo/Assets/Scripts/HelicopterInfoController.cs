using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 3. 28.
   Description : 헬리콥터의 초기 정보를 관리하는 스크립트
   Edit Log    : 
    - 헬리콥터의 초기 정보(무장량, 체력, 속도 등)를 설정합니다. 
   ================================================================= */

public class HelicopterInfoController : MonoBehaviour
{
    [Header("Basic Helicopter Info")]
    public float fMaxDurability = 1.0f;
    public float fDurability = 0.0f;
    public float fArmor = 0.0f;

    [Header("Helicopter Amrmament Setting")]
    public bool bActivateGunPod = false;
    public GameObject objGunPod;
    public bool bActivateRocketPod = false;
    public GameObject objRocketPod;
    public bool bActiveSearchLight = false;
    public Light lightSearchLight;

    [Header("Helicopter Stowage")]
    public GameObject objStowage = null;

    public bool bStatus = false;

    public bool bPlayWithJoystick = false;

    HelicopterFlightController hfc;

    [Header("SFX Controller")]
    AudioSource audioSource;
    public AudioClip sfx30Low;
    public AudioClip sfx20Low;
    public AudioClip sfx10Low;
    public AudioClip sfxDisabled;

    void Start ()
    {
        bStatus = true;
        fDurability = fMaxDurability;

        hfc = GetComponent<HelicopterFlightController>();
        hfc.bPlayWithJoystick = bPlayWithJoystick;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.maxDistance = 3.5f;
        audioSource.volume = 0.5f;
    }

    void Update()
    {
        if (audioSource.clip != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    void FixedUpdate ()
    {
        if (fDurability <= 0.0f)
        {
            bStatus = false;
            audioSource.clip = sfxDisabled;
        }

        if (bStatus == false)
        {
            hfc.CrashHelicopter();
        }
        else
        {
            float fDurabilityRatio = fDurability / fMaxDurability * 100.0f;
            if (fDurabilityRatio > 30.0f)
                audioSource.clip = null;
            if (20.0f < fDurabilityRatio && fDurabilityRatio <= 30.0f && audioSource.clip != sfx30Low)
                audioSource.clip = sfx30Low;
            else if (10.0f < fDurabilityRatio && fDurabilityRatio <= 20.0f && audioSource.clip != sfx20Low)
                audioSource.clip = sfx20Low;
            else if (0.0f < fDurabilityRatio && fDurabilityRatio <= 10.0f && audioSource.clip != sfx10Low)
                audioSource.clip = sfx10Low;
        }
    }

    /// <summary>
    /// 헬리콥터의 데미지를 입히는 함수입니다. 
    /// 옵션으로 헬리콥터의 방어력을 적용할 수 있습니다. 
    /// </summary>
    /// <param name="fDelta">헬리콥터가 받을 데미지를 양수로 입력합니다. </param>
    /// <param name="bRelative">헬리콥터의 방어력을 고려하려면 true로 설정하세요. </param>
    public void Damage(float fDelta, bool bRelative = false)
    {
        float temp = 0.0f;

        if (bRelative)
        {
            temp = fDelta - fArmor;

            if (temp > 0.0f)
            {
                fDurability -= temp;
            }
            else
            {
                fDurability -= 0.0f; // nothing happened
            }
        }
        else
        {
            fDurability -= fDelta;
        }
    }
    /// <summary>
    /// 헬리콥터를 수리하는 함수입니다. 
    /// </summary>
    /// <param name="fDelta">헬리콥터가 수리될 내구도를 양수로 입력합니다. </param>
    public void Repair(float fDelta)
    {
        fDurability += fDelta;
        if (fDurability > fMaxDurability)
            fDurability = fMaxDurability;
    }

    /// <summary>
    /// 현재 헬리콥터의 상태를 설정합니다. 
    /// HIC와 HFC 사이를 이어주는 느낌입니다. 
    /// </summary>
    public void SetHelicopterStatus()
    {
        objGunPod.SetActive(bActivateGunPod);
        objRocketPod.SetActive(bActivateRocketPod);
        lightSearchLight.gameObject.SetActive(bActiveSearchLight);
        if (hfc != null)
            hfc.bPlayWithJoystick = bPlayWithJoystick;
    }
}
