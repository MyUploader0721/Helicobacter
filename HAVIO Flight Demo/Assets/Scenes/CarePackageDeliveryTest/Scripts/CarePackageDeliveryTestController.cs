using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* =================================================================
   Editor      : 곽수환
   Date        : 2018. 4. 4.
   Description : CarePackageDelivery 테스트 게임모드의 초기설정
   Edit Log    : 
    - 테스트 게임모드인 CarePackageDelivery 게임 모드의 컨트롤을
      담당합니다. 
    - 레벨디자인 때 옵션을 수정하여 (과장하여)무한대의 게임모드를
      만들어 낼 수 있습니다. 
   ================================================================= */

public class CarePackageDeliveryTestController : MonoBehaviour
{
    [Header("Basic Setting")]
    public GameObject objPlayer;
    public float fMaxDurability;

    [Header("Helicopter Armament Status Setting")]
    public bool bActiveGunPod = false;
    public bool bActiveRocketPod = false;
    public bool bActiveSearchLight = false;

    [Header("Control Setting")]
    public bool bPlayWithJoystick = false;

    [Header("Care Package Delivery Settings")]
    [Header("* Target Positions")]
    public GameObject[] objTargetPositionCandidate;
    public List<GameObject> objlTargetPosition;
    [Header("* Random Target Number")]
    public int nRandomTargetPosition;

    public GameObject objGuide;

    HelicopterInfoController hic;

    public int nDeliveredPoint = 0;
    public bool bAccomplished = false;

    void Start ()
    {
        // Helicopter Setting
        hic = objPlayer.GetComponent<HelicopterInfoController>();

        hic.fMaxDurability = fMaxDurability;
        hic.fDurability = hic.fMaxDurability;

        hic.bActivateGunPod = bActiveGunPod;
        hic.bActivateRocketPod = bActiveRocketPod;
        hic.bActiveSearchLight = bActiveSearchLight;

        hic.bPlayWithJoystick = bPlayWithJoystick;

        hic.SetHelicopterStatus();

        objlTargetPosition = new List<GameObject>();

        // GameMode Setting
        if (nRandomTargetPosition > objTargetPositionCandidate.Length)
            nRandomTargetPosition = objTargetPositionCandidate.Length;
        int temp = objTargetPositionCandidate.Length;

        for (int i = 0; i < nRandomTargetPosition; i++)
        {
            int rand = Random.Range(0, temp);

            if (objTargetPositionCandidate[rand] != null)
            {
                objlTargetPosition.Add(objTargetPositionCandidate[rand]);
                objTargetPositionCandidate[rand] = null;
            }
            else
            {
                i--;
                continue;
            }
        }

        // Display the real target
        foreach (GameObject obj in objlTargetPosition)
        {
            Vector3 v3Pos = obj.transform.position;
            v3Pos.y += 5.0f;
            Instantiate(objGuide, v3Pos, Quaternion.identity, obj.transform);
        }
    }
	
	void FixedUpdate ()
    {
        if (nDeliveredPoint == nRandomTargetPosition)
            bAccomplished = true;
	}
}
