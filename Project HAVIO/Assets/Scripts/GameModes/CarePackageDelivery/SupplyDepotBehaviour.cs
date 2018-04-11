using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: SupplyDepotBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: 보급고의 행동에 대한 정의가 되어있는 스크립트
 *     DEV LOG: 
 *  - 보급고가 어떻게 행동하는지를 정의해 놓은 스크립트입니다. 
 *  - 보급 시간과 이에 대한 사운드 출력을 주로 작업합니다. 
 */

public class SupplyDepotBehaviour : MonoBehaviour
{
    [Header("Supply Depot Behaviour")]
    [SerializeField] GameObject objPlayer;
    [SerializeField] float fLoadingPackageTime = 5.0f;
    [SerializeField] GameObject objPackage;
    [SerializeField] CarePackageDeliveryController cpdController;

    AudioSource[] audioSource;
    enum SFX_List { PACKAGE_LOADING = 0 }
    [Header("SFX: Package Load")]
    [SerializeField] AudioClip sfxPackageStartLoading;
    [SerializeField] AudioClip sfxPackageLoading;
    [SerializeField] AudioClip sfxPackageStopLoading;
    [SerializeField] AudioClip sfxPackageFinishLoading;

    bool bIsLoading = false;

    void Start ()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        audioSource = new AudioSource[1];
        audioSource[(int)SFX_List.PACKAGE_LOADING] = gameObject.AddComponent<AudioSource>();
        audioSource[(int)SFX_List.PACKAGE_LOADING].volume = 0.5f;
        audioSource[(int)SFX_List.PACKAGE_LOADING].maxDistance = 3.0f;

    }
	
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objPlayer.GetComponent<HelicopterInfo>().objCargo == null && !bIsLoading)
        {
            StartCoroutine(LoadingPackage());
        }

        if (other.CompareTag("Player") && cpdController.bAccomplished)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && objPlayer.GetComponent<HelicopterInfo>().objCargo == null)
        {
            if (bIsLoading)
            {
                bIsLoading = false;
                StopAllCoroutines();

                audioSource[(int)SFX_List.PACKAGE_LOADING].clip = sfxPackageStopLoading;
                audioSource[(int)SFX_List.PACKAGE_LOADING].Play();
            }
        }
    }

    IEnumerator LoadingPackage()
    {
        bIsLoading = true;
        float fStartTime = Time.time;

        audioSource[(int)SFX_List.PACKAGE_LOADING].clip = sfxPackageStartLoading;
        audioSource[(int)SFX_List.PACKAGE_LOADING].Play();

        while (Time.time - fStartTime < fLoadingPackageTime)
        {
            if (!audioSource[(int)SFX_List.PACKAGE_LOADING].isPlaying)
            {
                audioSource[(int)SFX_List.PACKAGE_LOADING].clip = sfxPackageLoading;
                audioSource[(int)SFX_List.PACKAGE_LOADING].Play();
            }
            yield return new WaitForFixedUpdate();
        }

        audioSource[(int)SFX_List.PACKAGE_LOADING].clip = sfxPackageFinishLoading;
        audioSource[(int)SFX_List.PACKAGE_LOADING].Play();

        GameObject objInstantPackage = Instantiate(objPackage, objPlayer.transform);
        objInstantPackage.transform.localPosition = new Vector3(0.0f, -1.5f, 0.0f);
        objInstantPackage.transform.localRotation = Quaternion.identity;
        objPlayer.GetComponent<HelicopterInfo>().objCargo = objInstantPackage;

        bIsLoading = false;
    }
}
