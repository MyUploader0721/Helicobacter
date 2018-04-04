using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 4.
   Description : 케어패키지 배달 테스트 게임모드의 보급기지 컨트롤러
   Edit Log    : 
    - 플레이어는 이 곳에서 케어패키지를 얻습니다. 
   ================================================================= */

public class SupplyPointBehaviour : MonoBehaviour
{
    GameObject objPlayer;

    [Header("Care Package Prefab & Setting")]
    public GameObject objCarePackage;
    public float fLoadTime = 3.0f;

    HelicopterInfoController hic;

    bool bIsLoading = false;

    AudioSource audioSource;

	void Start ()
    {
        objPlayer = GameObject.FindGameObjectWithTag("Player");
        hic = objPlayer.GetComponent<HelicopterInfoController>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Body") && hic.objStowage == null && !bIsLoading) 
        {
            StartCoroutine(SupplyCarePackage());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Body") && hic.objStowage == null && !bIsLoading)
        {
            Debug.Log("Failed to Load CarePackage!");
            StopAllCoroutines();
            bIsLoading = false;
        }
    }

    /// <summary>
    /// 지정된 위치에서 케어패키지를 얻어옵니다. 
    /// 해당 시간동안 가만히 있어야 합니다. 
    /// </summary>
    /// <returns>코루틴 함수입니다. </returns>
    IEnumerator SupplyCarePackage()
    {
        if (hic.objStowage == null)
        {
            bIsLoading = true;
            Debug.Log("CarePackage Loading...");
            yield return new WaitForSeconds(fLoadTime);

            audioSource.Play();
            hic.objStowage = Instantiate(objCarePackage, objPlayer.transform);
            hic.objStowage.transform.localPosition = new Vector3(0.0f, -1.7f, 0.0f);
            hic.objStowage.transform.localEulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
            Debug.Log("CarePackage Loaded!");
            bIsLoading = false;
        }
    }
}
