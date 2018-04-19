using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *       TITLE: PackageReceiverBehaviour.cs
 *      AUTHOR: 곽수환
 *        DATE: 2018-04-10
 * DESCRIPTION: 보급지에 제대로 배달이 되었는지 검사하는 스크립트
 *     DEV LOG: 
 *  - 보급지에 케어패키지가 제대로 배달이 되었는지를 검사합니다. 
 */

public class PackageReceiverBehaviour : MonoBehaviour
{
    [HideInInspector] public bool bPackageDelivered = false;
    [SerializeField] GameObject objInducementSign;

    void Update()
    {
        objInducementSign.GetComponent<Renderer>().enabled = !bPackageDelivered;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Package"))
        {
            bPackageDelivered = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Package"))
        {
            bPackageDelivered = false;
        }
    }
}
