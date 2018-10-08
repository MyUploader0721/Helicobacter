using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: CarePackageDeliveryController.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: CarePackageDelivery 게임모드를 컨트롤하는 스크립트
 *     DEV LOG: 
 *  - Care Package를 배달하는 임무를 수행하는 게임모드를 설정합니다. 
 *  - 정해진 매개변수들을 조절하여 난이도를 변경할 수 있습니다. 
 */

public class CarePackageDeliveryController : MonoBehaviour
{
    HelicopterInfo helicopterInfo;

    [Header("Player Helicopter")]
    [SerializeField] GameObject objPlayer;

    [Header("Helicopter Setting")]
    [SerializeField] bool bIsPlayWithGamePad = false;

    [Header("Helicopter Armament Setting")]
    [SerializeField] bool bUseSearchLight = false;
    [SerializeField] bool bUseInnerPod = false;
    [SerializeField] bool bUseOuterPod = false;

    [Header("Game Mode Setting")]
    [SerializeField] int nNumReceiver;
    [SerializeField] GameObject[] objReceiver;
    public bool bAccomplished = false;

    void Start ()
    {
        if (objPlayer == null)
            objPlayer = GameObject.FindGameObjectWithTag("Player");

        helicopterInfo = objPlayer.GetComponent<HelicopterInfo>();
        helicopterInfo.bIsPlayWithGamePad = bIsPlayWithGamePad;
        helicopterInfo.bUseSearchLight    = bUseSearchLight;
        helicopterInfo.bUseInnerPod       = bUseInnerPod;
        helicopterInfo.bUseOuterPod       = bUseOuterPod;

        // 리시버(배달지)를 랜덤으로 바꿔줍니다. 
        // Fisher-Yates Shuffle 알고리즘을 적용하였습니다. 
        // REF: https://stackoverflow.com/questions/273313/randomize-a-listt
        int nCount = objReceiver.Length;
        while (nCount > 1)
        {
            nCount--;
            int k = Random.Range(0, nCount + 1);
            GameObject value = objReceiver[k];
            objReceiver[k] = objReceiver[nCount];
            objReceiver[nCount] = value;
        }

        for (int i = 0; i < objReceiver.Length; i++)
        {
            if (i >= nNumReceiver)
            {
                Destroy(objReceiver[i]);
                objReceiver[i] = null;
            }
        }
    }
	
	void Update ()
    {
        int nObjective = 0;
        for (int i = 0; i < nNumReceiver; i++)
        {
            if (objReceiver[i].GetComponent<PackageReceiverBehaviour>().bPackageDelivered)
                nObjective++;

            if (nObjective == nNumReceiver)
                bAccomplished = true;
        }
    }
}
