using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: CarePackageBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: 배달품(CarePackage)의 행동을 정의하는 스크립트
 *     DEV LOG: 
 *  - 케어패키지가 어떻게 동작할지를 정의한 스크립트입니다. 
 *  - 헬리콥터 밑에 달려있을 때에는 충돌만 검사하기에 일반적인
 *    콜라이더로만 작용하게 됩니다. 
 *  - 하지만 헬리콥터로부터 떨어져 나가게 될 경우, 헬리콥터와는
 *    독립적으로 행동하게 됩니다. 
 */

public class CarePackageBehaviour : MonoBehaviour
{
    HelicopterInfo helicopterInfo;

    [Header("Care Package Info")]
    [SerializeField] GameObject objPlayer;

    AudioSource[] audioSource;
    enum SFX_List { CAREPACKAGE_CRASH, CAREPACKAGE_DETACH }
    [Header("SFX: Care Package")]
    [SerializeField] AudioClip[] sfxCrash;
    [SerializeField] AudioClip sfxDetach;

    void Start ()
    {
        if (objPlayer == null)
            objPlayer = transform.parent.gameObject;
        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();

        audioSource = new AudioSource[2];
        audioSource[(int)SFX_List.CAREPACKAGE_CRASH] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.CAREPACKAGE_CRASH].volume = 0.5f;
        audioSource[(int)SFX_List.CAREPACKAGE_CRASH].spatialBlend = 1.0f;

        audioSource[(int)SFX_List.CAREPACKAGE_DETACH] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.CAREPACKAGE_DETACH].volume = 0.5f;
        audioSource[(int)SFX_List.CAREPACKAGE_DETACH].clip = sfxDetach;
    }
	
	void Update ()
    {
		if (
            ((helicopterInfo.bIsPlayWithGamePad && Input.GetButtonDown("DropPackage")) ||
            (!helicopterInfo.bIsPlayWithGamePad && Input.GetKeyDown(KeyCode.V))) && transform.parent != null 
           )
        {
            audioSource[(int)SFX_List.CAREPACKAGE_DETACH].Play();
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = true;
            helicopterInfo.objCargo = null;
            transform.parent = null;
        }
	}
}
